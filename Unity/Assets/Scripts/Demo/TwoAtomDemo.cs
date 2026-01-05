// AtomCoLab-VR - Large-Scale Atomistic Visualization
// Copyright (c) 2024-2025 AtomCoLab-VR Contributors
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

using UnityEngine;
using AtomCoLab.Data;
using AtomCoLab.Rendering;

namespace AtomCoLab.Demo
{
    /// <summary>
    /// Simple demo that displays a few atoms for testing.
    /// Attach this to any GameObject in your scene.
    /// </summary>
    public class TwoAtomDemo : MonoBehaviour
    {
        [Header("Renderer Reference")]
        [SerializeField] private GPUAtomRenderer _renderer;

        [Header("Demo Settings")]
        [SerializeField] private float _atomSeparation = 3f;
        [SerializeField] private int _atomType1 = 0; // Magnesium
        [SerializeField] private int _atomType2 = 1; // Aluminum

        private void Start()
        {
            if (_renderer == null)
            {
                _renderer = FindObjectOfType<GPUAtomRenderer>();
            }

            if (_renderer == null)
            {
                Debug.LogError("TwoAtomDemo: GPUAtomRenderer not found! Please add one to the scene.");
                return;
            }

            CreateTwoAtoms();
        }

        private void CreateTwoAtoms()
        {
            // Create two atoms
            AtomData[] atoms = new AtomData[]
            {
                new AtomData(-_atomSeparation / 2f, 0f, 0f, _atomType1),
                new AtomData(_atomSeparation / 2f, 0f, 0f, _atomType2)
            };

            // Define atom types (colors and radii)
            AtomTypeInfo[] atomTypes = new AtomTypeInfo[]
            {
                AtomTypeInfo.Magnesium,  // Type 0: Green
                AtomTypeInfo.Aluminum,   // Type 1: Gray
                AtomTypeInfo.Iron,       // Type 2: Orange (for future use)
                AtomTypeInfo.Oxygen      // Type 3: Red (for future use)
            };

            // Initialize renderer
            _renderer.Initialize(atoms, atomTypes);

            Debug.Log($"TwoAtomDemo: Created {atoms.Length} atoms");
        }

        /// <summary>
        /// Call this to add more atoms dynamically (for testing scale).
        /// </summary>
        [ContextMenu("Create 1000 Random Atoms")]
        public void Create1000Atoms()
        {
            int count = 1000;
            AtomData[] atoms = new AtomData[count];

            for (int i = 0; i < count; i++)
            {
                atoms[i] = new AtomData(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    Random.Range(0, 4)
                );
            }

            AtomTypeInfo[] atomTypes = new AtomTypeInfo[]
            {
                AtomTypeInfo.Magnesium,
                AtomTypeInfo.Aluminum,
                AtomTypeInfo.Iron,
                AtomTypeInfo.Oxygen
            };

            _renderer.Initialize(atoms, atomTypes);
            Debug.Log($"Created {count} random atoms");
        }

        /// <summary>
        /// Create a stress test with many atoms.
        /// </summary>
        [ContextMenu("Create 100,000 Atoms")]
        public void Create100KAtoms()
        {
            int count = 100000;
            AtomData[] atoms = new AtomData[count];

            // Create a cubic lattice
            int side = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
            float spacing = 2.5f;

            int index = 0;
            for (int x = 0; x < side && index < count; x++)
            {
                for (int y = 0; y < side && index < count; y++)
                {
                    for (int z = 0; z < side && index < count; z++)
                    {
                        atoms[index] = new AtomData(
                            x * spacing - side * spacing / 2f,
                            y * spacing - side * spacing / 2f,
                            z * spacing - side * spacing / 2f,
                            (x + y + z) % 4
                        );
                        index++;
                    }
                }
            }

            AtomTypeInfo[] atomTypes = new AtomTypeInfo[]
            {
                AtomTypeInfo.Magnesium,
                AtomTypeInfo.Aluminum,
                AtomTypeInfo.Iron,
                AtomTypeInfo.Oxygen
            };

            _renderer.Initialize(atoms, atomTypes);
            Debug.Log($"Created {count} atoms in cubic lattice");
        }
    }
}
