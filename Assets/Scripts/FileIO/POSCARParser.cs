// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.Collections.Generic;
using System.Globalization;
using AtomCoLab.Core;
using UnityEngine;

namespace AtomCoLab.FileIO
{
    /// <summary>
    /// Parser for VASP POSCAR/CONTCAR file format.
    /// </summary>
    public static class POSCARParser
    {
        /// <summary>
        /// Parses POSCAR content into a Structure.
        /// </summary>
        public static Structure Parse(string content, string name = "Structure")
        {
            var structure = new Structure();
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 8)
                throw new FormatException("Invalid POSCAR file: too few lines");

            int lineIndex = 0;

            // Line 1: Comment/name
            structure.Name = lines[lineIndex++].Trim();
            if (string.IsNullOrEmpty(structure.Name))
                structure.Name = name;

            // Line 2: Scaling factor
            float scale = ParseFloat(lines[lineIndex++]);

            // Lines 3-5: Lattice vectors
            Vector3 a = ParseVector(lines[lineIndex++]) * scale;
            Vector3 b = ParseVector(lines[lineIndex++]) * scale;
            Vector3 c = ParseVector(lines[lineIndex++]) * scale;

            structure.UnitCell = VectorsToUnitCell(a, b, c);

            // Line 6: Element symbols (VASP 5+) or atom counts (VASP 4)
            string line6 = lines[lineIndex++];
            var parts6 = line6.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> elementSymbols = new();
            List<int> atomCounts = new();

            // Check if line 6 contains element symbols or numbers
            bool hasSymbols = false;
            foreach (var part in parts6)
            {
                if (!int.TryParse(part, out _))
                {
                    hasSymbols = true;
                    break;
                }
            }

            if (hasSymbols)
            {
                // VASP 5+ format: line 6 is elements, line 7 is counts
                elementSymbols.AddRange(parts6);
                var parts7 = lines[lineIndex++].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts7)
                {
                    if (int.TryParse(part, out int count))
                        atomCounts.Add(count);
                }
            }
            else
            {
                // VASP 4 format: line 6 is counts, no element symbols
                foreach (var part in parts6)
                {
                    if (int.TryParse(part, out int count))
                        atomCounts.Add(count);
                }
                // Try to get elements from comment line
                var commentParts = structure.Name.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                elementSymbols.AddRange(commentParts);
            }

            // Check for Selective dynamics
            string coordLine = lines[lineIndex++];
            if (coordLine.Trim().StartsWith("S", StringComparison.OrdinalIgnoreCase))
            {
                coordLine = lines[lineIndex++];
            }

            // Coordinate type: Direct (fractional) or Cartesian
            bool fractional = coordLine.Trim().StartsWith("D", StringComparison.OrdinalIgnoreCase);

            // Parse atom positions
            int totalAtoms = 0;
            foreach (int count in atomCounts)
                totalAtoms += count;

            int elementIndex = 0;
            int atomInElement = 0;
            int currentElementCount = atomCounts.Count > 0 ? atomCounts[0] : 0;

            for (int i = 0; i < totalAtoms && lineIndex < lines.Length; i++)
            {
                Vector3 position = ParseVector(lines[lineIndex++]);

                if (fractional && structure.UnitCell != null)
                {
                    position = structure.UnitCell.FractionalToCartesian(position);
                }
                else if (!fractional)
                {
                    position *= scale;
                }

                string symbol = elementIndex < elementSymbols.Count ? elementSymbols[elementIndex] : "X";
                Element element = PeriodicTable.ParseSymbol(symbol);

                var atom = new Atom(element, position)
                {
                    Label = symbol
                };
                structure.AddAtom(atom);

                atomInElement++;
                if (atomInElement >= currentElementCount && elementIndex < atomCounts.Count - 1)
                {
                    elementIndex++;
                    atomInElement = 0;
                    currentElementCount = atomCounts[elementIndex];
                }
            }

            return structure;
        }

        private static float ParseFloat(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                return value;
            return 1f;
        }

        private static Vector3 ParseVector(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            float x = 0, y = 0, z = 0;
            if (parts.Length >= 1)
                float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x);
            if (parts.Length >= 2)
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y);
            if (parts.Length >= 3)
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z);

            return new Vector3(x, y, z);
        }

        private static UnitCell VectorsToUnitCell(Vector3 a, Vector3 b, Vector3 c)
        {
            float aLen = a.magnitude;
            float bLen = b.magnitude;
            float cLen = c.magnitude;

            float alpha = Mathf.Acos(Mathf.Clamp(Vector3.Dot(b.normalized, c.normalized), -1f, 1f)) * Mathf.Rad2Deg;
            float beta = Mathf.Acos(Mathf.Clamp(Vector3.Dot(a.normalized, c.normalized), -1f, 1f)) * Mathf.Rad2Deg;
            float gamma = Mathf.Acos(Mathf.Clamp(Vector3.Dot(a.normalized, b.normalized), -1f, 1f)) * Mathf.Rad2Deg;

            return new UnitCell(aLen, bLen, cLen, alpha, beta, gamma);
        }
    }
}
