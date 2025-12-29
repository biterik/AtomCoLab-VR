# AtomCoLab-VR

**Immersive Crystal Structure Visualization for Meta Quest**

AtomCoLab-VR is a scientific visualization platform that brings crystal structures and atomic arrangements to life in virtual reality. Built for Meta Quest headsets using Unity, it enables researchers, educators, and students to explore crystallographic structures in an intuitive, immersive environment.

## Features

- **3D Crystal Visualization** - Render crystal structures with atomic-level detail
- **Interactive Manipulation** - Grab, rotate, and scale structures using hand tracking or controllers
- **Multiple Rendering Modes** - Ball-and-stick, space-filling, polyhedral, and wireframe representations
- **Unit Cell Display** - Visualize unit cells with lattice parameters and symmetry
- **Supercell Generation** - Build and explore supercells in real-time
- **File Format Support** - Import CIF, VASP (POSCAR/CONTCAR), XYZ, and XSF files
- **Collaborative Sessions** - Multi-user VR workspace for team research
- **Measurement Tools** - Distance, angle, and coordination analysis
- **Annotation System** - Add labels and notes to structures

## Requirements

- **Hardware:** Meta Quest 2, Quest 3, or Quest Pro
- **Unity:** 2022.3 LTS or newer
- **Packages:**
  - Meta XR SDK
  - XR Interaction Toolkit
  - TextMeshPro

## Installation

### For Users

1. Download the latest APK from [Releases](https://github.com/your-org/AtomCoLab-VR/releases)
2. Install via SideQuest or ADB:
   ```bash
   adb install AtomCoLab-VR.apk
   ```

### For Developers

1. Clone the repository:
   ```bash
   git clone https://github.com/your-org/AtomCoLab-VR.git
   ```

2. Open the project in Unity Hub (Unity 2022.3 LTS)

3. Install required packages via Package Manager

4. Configure Meta Quest build settings:
   - Switch to Android platform
   - Set texture compression to ASTC
   - Enable Meta XR features

5. Build and deploy to your Quest device

## Project Structure

```
AtomCoLab-VR/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # Core systems and utilities
│   │   ├── Visualization/  # Structure rendering
│   │   ├── Interaction/    # VR interaction handlers
│   │   ├── FileIO/         # File format parsers
│   │   ├── Networking/     # Multiplayer support
│   │   └── UI/             # User interface
│   ├── Prefabs/            # Reusable prefabs
│   ├── Materials/          # Shaders and materials
│   ├── Scenes/             # Unity scenes
│   └── Resources/          # Runtime-loaded assets
├── Packages/               # Unity packages
├── Documentation/          # Project documentation
└── Tests/                  # Unit and integration tests
```

## Quick Start

```csharp
using AtomCoLab.Core;
using AtomCoLab.Visualization;

// Load a structure from CIF file
var structure = StructureLoader.LoadFromCIF("path/to/structure.cif");

// Create visualization
var visualizer = new StructureVisualizer();
visualizer.SetRenderMode(RenderMode.BallAndStick);
visualizer.Render(structure);

// Generate a 2x2x2 supercell
var supercell = structure.GenerateSupercell(2, 2, 2);
```

## Documentation

- [User Guide](Documentation/UserGuide.md)
- [Developer Guide](Documentation/DeveloperGuide.md)
- [API Reference](Documentation/API.md)
- [File Formats](Documentation/FileFormats.md)

## License

AtomCoLab-VR uses a **dual licensing** model:

### Noncommercial Use (Free)

This project is licensed under the [PolyForm Noncommercial License 1.0.0](LICENSE) for noncommercial use. This includes:

- Academic research
- Education and teaching
- Personal learning and experimentation
- Non-profit organizations

### Commercial Use (Paid)

For commercial use, including use in commercial products, services, or any revenue-generating activity, a commercial license is required. See [LICENSE-COMMERCIAL.md](LICENSE-COMMERCIAL.md) for details.

**Contact:** licensing@atomcolab-vr.com

## Contributing

We welcome contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) and [Code of Conduct](CODE_OF_CONDUCT.md) before submitting pull requests.

## Acknowledgments

- Meta XR SDK Team
- Crystallography Open Database
- Materials Project

## Contact

- **Issues:** [GitHub Issues](https://github.com/your-org/AtomCoLab-VR/issues)
- **Discussions:** [GitHub Discussions](https://github.com/your-org/AtomCoLab-VR/discussions)
- **Email:** contact@atomcolab-vr.com
