// AtomCoLab-VR - Large-Scale Atomistic Visualization
// Copyright (c) 2024-2025 AtomCoLab-VR Contributors
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

using UnityEngine;
using AtomCoLab.Data;

namespace AtomCoLab.Rendering
{
    /// <summary>
    /// GPU-instanced atom renderer using DrawMeshInstancedIndirect.
    /// Capable of rendering millions of atoms in a single draw call.
    /// </summary>
    public class GPUAtomRenderer : MonoBehaviour
    {
        [Header("Rendering")]
        [SerializeField] private Mesh _sphereMesh;
        [SerializeField] private Material _atomMaterial;
        [SerializeField] private float _atomScale = 1f;

        [Header("Debug")]
        [SerializeField] private bool _showBounds = true;

        // GPU Buffers
        private ComputeBuffer _atomBuffer;
        private ComputeBuffer _atomTypeBuffer;
        private ComputeBuffer _argsBuffer;

        // State
        private int _atomCount;
        private Bounds _bounds;
        private bool _isInitialized;

        // Shader property IDs (cached for performance)
        private static readonly int AtomBufferID = Shader.PropertyToID("_AtomBuffer");
        private static readonly int AtomTypesID = Shader.PropertyToID("_AtomTypes");
        private static readonly int AtomScaleID = Shader.PropertyToID("_AtomScale");

        /// <summary>
        /// Number of atoms currently loaded.
        /// </summary>
        public int AtomCount => _atomCount;

        /// <summary>
        /// Whether the renderer is initialized and ready.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        private void Awake()
        {
            // Create default sphere mesh if not assigned
            if (_sphereMesh == null)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _sphereMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
                Destroy(sphere);
            }
        }

        private void OnDestroy()
        {
            ReleaseBuffers();
        }

        /// <summary>
        /// Initialize renderer with atom data.
        /// </summary>
        /// <param name="atoms">Array of atom data (positions, types, etc.)</param>
        /// <param name="atomTypes">Atom type definitions (colors, radii)</param>
        public void Initialize(AtomData[] atoms, AtomTypeInfo[] atomTypes)
        {
            if (atoms == null || atoms.Length == 0)
            {
                Debug.LogError("GPUAtomRenderer: No atoms provided");
                return;
            }

            if (_atomMaterial == null)
            {
                Debug.LogError("GPUAtomRenderer: Material not assigned");
                return;
            }

            ReleaseBuffers();

            _atomCount = atoms.Length;

            // Calculate bounds
            _bounds = CalculateBounds(atoms);

            // Create atom buffer
            _atomBuffer = new ComputeBuffer(_atomCount, AtomData.SizeInBytes);
            _atomBuffer.SetData(atoms);

            // Create atom type buffer (colors and radii)
            Vector4[] typeData = new Vector4[atomTypes.Length];
            for (int i = 0; i < atomTypes.Length; i++)
            {
                // Pack color (rgb) and radius (a) into Vector4
                typeData[i] = new Vector4(
                    atomTypes[i].Color.r,
                    atomTypes[i].Color.g,
                    atomTypes[i].Color.b,
                    atomTypes[i].Radius
                );
            }
            _atomTypeBuffer = new ComputeBuffer(atomTypes.Length, sizeof(float) * 4);
            _atomTypeBuffer.SetData(typeData);

            // Create indirect args buffer
            // Args: [indexCount, instanceCount, startIndex, baseVertex, startInstance]
            uint[] args = new uint[5];
            args[0] = _sphereMesh.GetIndexCount(0);
            args[1] = (uint)_atomCount;
            args[2] = _sphereMesh.GetIndexStart(0);
            args[3] = _sphereMesh.GetBaseVertex(0);
            args[4] = 0;

            _argsBuffer = new ComputeBuffer(1, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(args);

            // Set material properties
            _atomMaterial.SetBuffer(AtomBufferID, _atomBuffer);
            _atomMaterial.SetBuffer(AtomTypesID, _atomTypeBuffer);
            _atomMaterial.SetFloat(AtomScaleID, _atomScale);

            _isInitialized = true;

            Debug.Log($"GPUAtomRenderer: Initialized with {_atomCount} atoms");
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            // Update scale if changed
            _atomMaterial.SetFloat(AtomScaleID, _atomScale);

            // Render all atoms in a single draw call
            Graphics.DrawMeshInstancedIndirect(
                _sphereMesh,
                0,
                _atomMaterial,
                _bounds,
                _argsBuffer
            );
        }

        /// <summary>
        /// Clear all atom data and release GPU buffers.
        /// </summary>
        public void Clear()
        {
            ReleaseBuffers();
            _atomCount = 0;
            _isInitialized = false;
        }

        private void ReleaseBuffers()
        {
            _atomBuffer?.Release();
            _atomBuffer = null;

            _atomTypeBuffer?.Release();
            _atomTypeBuffer = null;

            _argsBuffer?.Release();
            _argsBuffer = null;
        }

        private Bounds CalculateBounds(AtomData[] atoms)
        {
            if (atoms.Length == 0)
                return new Bounds(Vector3.zero, Vector3.one);

            Vector3 min = atoms[0].Position;
            Vector3 max = atoms[0].Position;

            for (int i = 1; i < atoms.Length; i++)
            {
                min = Vector3.Min(min, atoms[i].Position);
                max = Vector3.Max(max, atoms[i].Position);
            }

            // Add padding for atom radii
            Vector3 padding = Vector3.one * 5f;
            Vector3 center = (min + max) / 2f;
            Vector3 size = (max - min) + padding;

            return new Bounds(center, size);
        }

        private void OnDrawGizmos()
        {
            if (_showBounds && _isInitialized)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
        }
    }
}
