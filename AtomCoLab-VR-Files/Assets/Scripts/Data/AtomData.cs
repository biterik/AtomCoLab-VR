// AtomCoLab-VR - Large-Scale Atomistic Visualization
// Copyright (c) 2024-2025 AtomCoLab-VR Contributors
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

using System.Runtime.InteropServices;
using UnityEngine;

namespace AtomCoLab.Data
{
    /// <summary>
    /// Atom data structure matching the .aclvr binary format.
    /// 32 bytes per atom, GPU-compatible layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AtomData
    {
        public Vector3 Position;    // 12 bytes
        public int AtomType;        // 4 bytes
        public Vector3 Velocity;    // 12 bytes
        public float Property;      // 4 bytes
        // Total: 32 bytes

        public AtomData(Vector3 position, int atomType = 0)
        {
            Position = position;
            AtomType = atomType;
            Velocity = Vector3.zero;
            Property = 0f;
        }

        public AtomData(float x, float y, float z, int atomType = 0)
        {
            Position = new Vector3(x, y, z);
            AtomType = atomType;
            Velocity = Vector3.zero;
            Property = 0f;
        }

        /// <summary>
        /// Size in bytes (must be 32 for GPU buffer alignment).
        /// </summary>
        public static int SizeInBytes => 32;
    }

    /// <summary>
    /// Atom type definition (element, color, radius).
    /// </summary>
    [System.Serializable]
    public struct AtomTypeInfo
    {
        public string Name;
        public Color Color;
        public float Radius;

        public AtomTypeInfo(string name, Color color, float radius)
        {
            Name = name;
            Color = color;
            Radius = radius;
        }

        // Common elements
        public static AtomTypeInfo Hydrogen => new("H", Color.white, 0.31f);
        public static AtomTypeInfo Carbon => new("C", new Color(0.56f, 0.56f, 0.56f), 0.77f);
        public static AtomTypeInfo Nitrogen => new("N", new Color(0.19f, 0.31f, 0.97f), 0.71f);
        public static AtomTypeInfo Oxygen => new("O", Color.red, 0.66f);
        public static AtomTypeInfo Magnesium => new("Mg", new Color(0.54f, 1f, 0f), 1.41f);
        public static AtomTypeInfo Aluminum => new("Al", new Color(0.75f, 0.65f, 0.65f), 1.21f);
        public static AtomTypeInfo Iron => new("Fe", new Color(0.88f, 0.4f, 0.2f), 1.32f);
    }
}
