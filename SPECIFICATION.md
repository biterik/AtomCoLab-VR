# AtomCoLab-VR Technical Specification

> **IMPORTANT FOR AI ASSISTANTS**: This is the authoritative specification. Do NOT deviate from the architecture, file formats, or technology choices defined here. If something contradicts this document, this document wins.

## Project Mission

AtomCoLab-VR is a **collaborative virtual reality environment for visualizing large-scale atomistic simulations** (molecular dynamics, Monte Carlo, etc.) on Meta Quest headsets. The primary use case is:

- Visualizing LAMMPS/OVITO output with **millions of atoms**
- Collaborative research discussions with **up to 12 users**
- Presenting defects, dislocations, and materials phenomena in VR

This is NOT a crystallography teaching tool. This is NOT for small molecules or proteins.

---

## Non-Negotiable Requirements

### 1. Scale Target
- **Minimum**: 1 million atoms at 72fps on Quest 3
- **Goal**: 5+ million atoms
- This REQUIRES GPU instancing. GameObject-per-atom approaches are forbidden.

### 2. File Format: `.aclvr` Binary + JSON Sidecar
- Custom binary format optimized for VR streaming
- NOT CIF, NOT POSCAR, NOT XYZ (these are for small structures)

### 3. Data Source: OVITO/LAMMPS Pipeline
- Export from OVITO using Python scripts
- Input: LAMMPS dump files, typically millions of atoms

### 4. Networking: Photon Fusion 2 + Photon Voice 2
- NOT generic Unity networking
- Specific collaboration modes required

---

## Technology Stack (MANDATORY)

| Component | Technology | Version |
|-----------|------------|---------|
| Engine | Unity | 2022.3 LTS |
| VR SDK | Meta XR All-in-One SDK | Latest |
| Rendering | GPU Instancing via `Graphics.DrawMeshInstancedIndirect` | - |
| Compute | Unity Compute Shaders | - |
| Networking | Photon Fusion 2 | Latest |
| Voice Chat | Photon Voice 2 | Latest |
| Data Export | OVITO Python API | 3.9+ |

---

## File Format Specification

### Binary Format: `.aclvr`

```
HEADER (64 bytes):
├── Magic number: "ACLVR001" (8 bytes)
├── Version: uint32 (4 bytes)
├── Atom count: uint64 (8 bytes)
├── Frame count: uint32 (4 bytes)
├── Flags: uint32 (4 bytes)
│   ├── Bit 0: Has velocities
│   ├── Bit 1: Has properties
│   ├── Bit 2: Has atom types
│   └── Bits 3-31: Reserved
├── Bounding box min: float32 × 3 (12 bytes)
├── Bounding box max: float32 × 3 (12 bytes)
└── Reserved (12 bytes)

ATOM DATA (32 bytes per atom):
├── Position X: float32 (4 bytes)
├── Position Y: float32 (4 bytes)
├── Position Z: float32 (4 bytes)
├── Atom type: int32 (4 bytes)
├── Velocity X: float32 (4 bytes)
├── Velocity Y: float32 (4 bytes)
├── Velocity Z: float32 (4 bytes)
└── Property value: float32 (4 bytes)
```

### Metadata Format: `.aclvr.json`

```json
{
  "version": "1.0",
  "name": "Mg_edge_dislocation_300K",
  "description": "Edge dislocation in Mg at 300K after 100ps relaxation",
  
  "source": {
    "software": "LAMMPS",
    "version": "23Jun2022",
    "potential": "Zhou EAM Mg",
    "potential_file": "Mg_zhou.eam.alloy"
  },
  
  "simulation": {
    "timestep_fs": 1.0,
    "temperature_K": 300,
    "pressure_GPa": 0.0,
    "total_time_ps": 100.0
  },
  
  "structure": {
    "crystal_structure": "hcp",
    "lattice_a": 3.209,
    "lattice_c": 5.211,
    "orientation": {
      "x": "[11-20]",
      "y": "[0001]", 
      "z": "[1-100]"
    },
    "supercell": [50, 30, 40]
  },
  
  "atom_types": {
    "1": {"element": "Mg", "color": [0.54, 1.0, 0.0], "radius": 1.6}
  },
  
  "coloring": {
    "default": "atom_type",
    "property_name": "centro_symmetry",
    "colormap": "viridis",
    "range": [0.0, 15.0]
  },
  
  "bibliography": {
    "title": "Dislocation dynamics in Mg alloys",
    "authors": ["E. Bitzek", "..."],
    "doi": "10.xxxx/xxxxx",
    "date": "2024-12-29"
  },
  
  "view_presets": [
    {
      "name": "Overview",
      "camera_position": [0, 0, 100],
      "camera_target": [0, 0, 0],
      "cutting_planes": []
    },
    {
      "name": "Dislocation Core",
      "camera_position": [25, 15, 20],
      "camera_target": [25, 15, 0],
      "cutting_planes": [
        {"normal": [0, 0, 1], "distance": 0, "enabled": true}
      ]
    }
  ],
  
  "annotations": [
    {
      "type": "point",
      "position": [25.3, 15.1, 0],
      "label": "Dislocation core",
      "color": [1, 0, 0]
    }
  ],
  
  "selection_groups": [
    {
      "name": "Stacking fault atoms",
      "atom_indices": [1234, 1235, 1236],
      "color": [1, 0.5, 0]
    }
  ]
}
```

---

## Rendering Architecture (CRITICAL)

### ❌ FORBIDDEN Approach (what was incorrectly built)
```csharp
// DO NOT DO THIS - doesn't scale beyond ~10K atoms
foreach (var atom in atoms)
{
    Instantiate(atomPrefab, position, rotation);
}
```

### ✅ REQUIRED Approach
```csharp
public class AtomRenderer : MonoBehaviour
{
    private ComputeBuffer _positionBuffer;
    private ComputeBuffer _colorBuffer;
    private ComputeBuffer _argsBuffer;
    private ComputeShader _cullingShader;
    
    public void Initialize(NativeArray<AtomData> atoms)
    {
        // Upload all atom data to GPU once
        _positionBuffer = new ComputeBuffer(atoms.Length, sizeof(float) * 4);
        _positionBuffer.SetData(atoms);
        
        // Indirect args for instanced rendering
        _argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        
        _material.SetBuffer("_Positions", _positionBuffer);
    }
    
    void Update()
    {
        // Optional: GPU frustum culling via compute shader
        _cullingShader.Dispatch(_cullingKernel, threadGroups, 1, 1);
        
        // Single draw call for ALL atoms
        Graphics.DrawMeshInstancedIndirect(
            _sphereMesh,
            0,
            _material,
            _bounds,
            _argsBuffer
        );
    }
}
```

### Required Shader Structure
```hlsl
// AtomInstanced.shader
StructuredBuffer<float4> _Positions;  // xyz = position, w = type
StructuredBuffer<float4> _Colors;

void vert(uint instanceID : SV_InstanceID, ...)
{
    float4 atomData = _Positions[instanceID];
    float3 worldPos = atomData.xyz;
    int atomType = (int)atomData.w;
    
    // Transform vertex by instance position
    output.pos = mul(UNITY_MATRIX_VP, float4(input.vertex.xyz * radius + worldPos, 1));
    output.color = _Colors[atomType];
}
```

---

## Directory Structure

```
AtomCoLab-VR/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── AtomData.cs           # Struct matching binary format
│   │   │   ├── ACLVRFile.cs          # Binary file reader
│   │   │   ├── MetadataLoader.cs     # JSON sidecar parser
│   │   │   └── AtomTypeDatabase.cs   # Element colors/radii
│   │   ├── Rendering/
│   │   │   ├── GPUAtomRenderer.cs    # DrawMeshInstancedIndirect
│   │   │   ├── AtomCulling.compute   # Frustum culling shader
│   │   │   ├── CuttingPlane.cs       # Slice visualization
│   │   │   └── LODController.cs      # Level of detail
│   │   ├── Interaction/
│   │   │   ├── StructureManipulator.cs
│   │   │   ├── AtomPicker.cs         # Raycast atom selection
│   │   │   └── MeasurementTool.cs
│   │   ├── Networking/
│   │   │   ├── PhotonManager.cs      # Photon Fusion setup
│   │   │   ├── SharedStateSync.cs    # Transform/settings sync
│   │   │   ├── CollaborationMode.cs  # Shared/Follow modes
│   │   │   └── VoiceChatManager.cs   # Photon Voice 2
│   │   └── UI/
│   │       ├── MainMenu.cs
│   │       ├── FileLoader.cs
│   │       └── SettingsPanel.cs
│   ├── Shaders/
│   │   ├── AtomInstanced.shader
│   │   ├── BondInstanced.shader
│   │   └── CuttingPlane.shader
│   ├── Prefabs/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   └── Viewer.unity
│   └── Resources/
├── Tools/
│   ├── ovito_export/
│   │   ├── export_aclvr.py           # OVITO Python modifier
│   │   ├── batch_export.py
│   │   └── README.md
│   └── converters/
│       ├── lammps_to_aclvr.py
│       └── xyz_to_aclvr.py
├── Schemas/
│   └── v1/
│       └── metadata.schema.json
├── SampleData/
│   ├── test_10k.aclvr
│   ├── test_10k.aclvr.json
│   └── test_1M.aclvr
├── Docs/
│   ├── UserGuide.md
│   ├── DeveloperGuide.md
│   └── FileFormat.md
├── SPECIFICATION.md                   # THIS FILE
├── README.md
├── LICENSE
└── LICENSE-COMMERCIAL.md
```

---

## Implementation Phases

### Phase 1: Core Rendering (CURRENT)
- [ ] Implement `AtomData` struct matching binary format
- [ ] Implement `.aclvr` binary file reader
- [ ] Implement GPU instanced rendering with ComputeBuffers
- [ ] Achieve 1M atoms at 72fps on Quest 3
- [ ] Basic VR interaction (grab, rotate, scale)

### Phase 2: Features
- [ ] Cutting planes
- [ ] Atom coloring by property
- [ ] JSON metadata loading
- [ ] View presets

### Phase 3: Collaboration
- [ ] Photon Fusion 2 integration
- [ ] State synchronization
- [ ] "Shared Object" mode
- [ ] "Follow Lead" mode
- [ ] Photon Voice 2 integration

### Phase 4: Tools
- [ ] OVITO export script
- [ ] LAMMPS converter
- [ ] Batch processing

---

## Performance Targets

| Metric | Target | Method |
|--------|--------|--------|
| Atom count | 5M+ | GPU instancing |
| Frame rate | 72fps | Compute culling |
| Load time (1M atoms) | <3s | Binary format |
| Memory (1M atoms) | <128MB | Packed structs |

---

## What NOT to Build

1. ❌ CIF/POSCAR/XYZ parsers (wrong scale)
2. ❌ GameObject-per-atom rendering (doesn't scale)
3. ❌ Protein/molecule features (residues, chains)
4. ❌ Chemical bond detection (too slow for MD)
5. ❌ Generic Unity networking (use Photon)

---

## Session Log

| Date | Session | Accomplishments |
|------|---------|-----------------|
| 2024-12-29 | 1 | Initial planning, file format design, repo created |
| 2025-01-05 | 2 | Discovered implementation drift, created this spec |

---

## Instructions for Claude Code

When working on this project:

1. **READ THIS FILE FIRST** before making any changes
2. **DO NOT** create CIF/XYZ/POSCAR parsers
3. **DO NOT** use `Instantiate()` for atoms
4. **ALWAYS** use GPU instancing for rendering
5. **FOLLOW** the directory structure exactly
6. **ASK** if something contradicts this specification

If you need to deviate from this spec, STOP and ask the user first.
