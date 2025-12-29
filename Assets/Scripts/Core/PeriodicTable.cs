// AtomCoLab-VR - Molecular Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System.Collections.Generic;
using UnityEngine;

namespace AtomCoLab.Core
{
    /// <summary>
    /// Provides element data from the periodic table.
    /// </summary>
    public static class PeriodicTable
    {
        private static readonly Dictionary<Element, ElementData> _elements = new()
        {
            { Element.H,  new ElementData("Hydrogen",   1.008f,   0.31f, 1.20f, new Color(1.00f, 1.00f, 1.00f)) },
            { Element.He, new ElementData("Helium",     4.003f,   0.28f, 1.40f, new Color(0.85f, 1.00f, 1.00f)) },
            { Element.Li, new ElementData("Lithium",    6.941f,   1.28f, 1.82f, new Color(0.80f, 0.50f, 1.00f)) },
            { Element.Be, new ElementData("Beryllium",  9.012f,   0.96f, 1.53f, new Color(0.76f, 1.00f, 0.00f)) },
            { Element.B,  new ElementData("Boron",     10.811f,   0.84f, 1.92f, new Color(1.00f, 0.71f, 0.71f)) },
            { Element.C,  new ElementData("Carbon",    12.011f,   0.76f, 1.70f, new Color(0.56f, 0.56f, 0.56f)) },
            { Element.N,  new ElementData("Nitrogen",  14.007f,   0.71f, 1.55f, new Color(0.19f, 0.31f, 0.97f)) },
            { Element.O,  new ElementData("Oxygen",    15.999f,   0.66f, 1.52f, new Color(1.00f, 0.05f, 0.05f)) },
            { Element.F,  new ElementData("Fluorine",  18.998f,   0.57f, 1.47f, new Color(0.56f, 0.88f, 0.31f)) },
            { Element.Ne, new ElementData("Neon",      20.180f,   0.58f, 1.54f, new Color(0.70f, 0.89f, 0.96f)) },
            { Element.Na, new ElementData("Sodium",    22.990f,   1.66f, 2.27f, new Color(0.67f, 0.36f, 0.95f)) },
            { Element.Mg, new ElementData("Magnesium", 24.305f,   1.41f, 1.73f, new Color(0.54f, 1.00f, 0.00f)) },
            { Element.Al, new ElementData("Aluminum",  26.982f,   1.21f, 1.84f, new Color(0.75f, 0.65f, 0.65f)) },
            { Element.Si, new ElementData("Silicon",   28.086f,   1.11f, 2.10f, new Color(0.94f, 0.78f, 0.63f)) },
            { Element.P,  new ElementData("Phosphorus",30.974f,   1.07f, 1.80f, new Color(1.00f, 0.50f, 0.00f)) },
            { Element.S,  new ElementData("Sulfur",    32.065f,   1.05f, 1.80f, new Color(1.00f, 1.00f, 0.19f)) },
            { Element.Cl, new ElementData("Chlorine",  35.453f,   1.02f, 1.75f, new Color(0.12f, 0.94f, 0.12f)) },
            { Element.Ar, new ElementData("Argon",     39.948f,   1.06f, 1.88f, new Color(0.50f, 0.82f, 0.89f)) },
            { Element.K,  new ElementData("Potassium", 39.098f,   2.03f, 2.75f, new Color(0.56f, 0.25f, 0.83f)) },
            { Element.Ca, new ElementData("Calcium",   40.078f,   1.76f, 2.31f, new Color(0.24f, 1.00f, 0.00f)) },
            { Element.Fe, new ElementData("Iron",      55.845f,   1.32f, 2.00f, new Color(0.88f, 0.40f, 0.20f)) },
            { Element.Cu, new ElementData("Copper",    63.546f,   1.32f, 1.40f, new Color(0.78f, 0.50f, 0.20f)) },
            { Element.Zn, new ElementData("Zinc",      65.380f,   1.22f, 1.39f, new Color(0.49f, 0.50f, 0.69f)) },
            { Element.Br, new ElementData("Bromine",   79.904f,   1.20f, 1.85f, new Color(0.65f, 0.16f, 0.16f)) },
            { Element.Ag, new ElementData("Silver",   107.868f,   1.45f, 1.72f, new Color(0.75f, 0.75f, 0.75f)) },
            { Element.I,  new ElementData("Iodine",   126.904f,   1.39f, 1.98f, new Color(0.58f, 0.00f, 0.58f)) },
            { Element.Au, new ElementData("Gold",     196.967f,   1.36f, 1.66f, new Color(1.00f, 0.82f, 0.14f)) },
            { Element.Pt, new ElementData("Platinum", 195.078f,   1.28f, 1.75f, new Color(0.82f, 0.82f, 0.88f)) },
            { Element.Pb, new ElementData("Lead",     207.200f,   1.46f, 2.02f, new Color(0.34f, 0.35f, 0.38f)) },
            { Element.U,  new ElementData("Uranium",  238.029f,   1.96f, 1.86f, new Color(0.00f, 0.56f, 0.00f)) },
        };

        private static readonly ElementData _unknown = new("Unknown", 0f, 1.5f, 1.5f, new Color(1f, 0.08f, 0.58f));

        /// <summary>
        /// Gets the element name.
        /// </summary>
        public static string GetName(Element element)
        {
            return _elements.TryGetValue(element, out var data) ? data.Name : _unknown.Name;
        }

        /// <summary>
        /// Gets the atomic mass in daltons.
        /// </summary>
        public static float GetAtomicMass(Element element)
        {
            return _elements.TryGetValue(element, out var data) ? data.AtomicMass : _unknown.AtomicMass;
        }

        /// <summary>
        /// Gets the covalent radius in angstroms.
        /// </summary>
        public static float GetCovalentRadius(Element element)
        {
            return _elements.TryGetValue(element, out var data) ? data.CovalentRadius : _unknown.CovalentRadius;
        }

        /// <summary>
        /// Gets the van der Waals radius in angstroms.
        /// </summary>
        public static float GetVanDerWaalsRadius(Element element)
        {
            return _elements.TryGetValue(element, out var data) ? data.VdWRadius : _unknown.VdWRadius;
        }

        /// <summary>
        /// Gets the CPK coloring scheme color.
        /// </summary>
        public static Color GetCPKColor(Element element)
        {
            return _elements.TryGetValue(element, out var data) ? data.CPKColor : _unknown.CPKColor;
        }

        /// <summary>
        /// Parses element symbol string to Element enum.
        /// </summary>
        public static Element ParseSymbol(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return Element.Unknown;

            symbol = symbol.Trim();

            if (symbol.Length > 2)
                symbol = symbol.Substring(0, 2);

            if (symbol.Length == 2)
                symbol = char.ToUpper(symbol[0]) + char.ToLower(symbol[1]).ToString();
            else
                symbol = symbol.ToUpper();

            return System.Enum.TryParse<Element>(symbol, out var element) ? element : Element.Unknown;
        }

        private readonly struct ElementData
        {
            public string Name { get; }
            public float AtomicMass { get; }
            public float CovalentRadius { get; }
            public float VdWRadius { get; }
            public Color CPKColor { get; }

            public ElementData(string name, float mass, float covalent, float vdw, Color color)
            {
                Name = name;
                AtomicMass = mass;
                CovalentRadius = covalent;
                VdWRadius = vdw;
                CPKColor = color;
            }
        }
    }
}
