// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.Globalization;
using AtomCoLab.Core;
using UnityEngine;

namespace AtomCoLab.FileIO
{
    /// <summary>
    /// Parser for XCrySDen Structure File (XSF) format.
    /// </summary>
    public static class XSFParser
    {
        /// <summary>
        /// Parses XSF content into a Structure.
        /// </summary>
        public static Structure Parse(string content, string name = "Structure")
        {
            var structure = new Structure { Name = name };
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            bool isPeriodic = false;
            Vector3 a = Vector3.zero, b = Vector3.zero, c = Vector3.zero;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.StartsWith("CRYSTAL") || line.StartsWith("SLAB") || line.StartsWith("POLYMER"))
                {
                    isPeriodic = true;
                }
                else if (line.StartsWith("PRIMVEC") || line.StartsWith("CONVVEC"))
                {
                    // Parse lattice vectors (next 3 lines)
                    if (i + 3 < lines.Length)
                    {
                        a = ParseVector(lines[++i]);
                        b = ParseVector(lines[++i]);
                        c = ParseVector(lines[++i]);
                        structure.UnitCell = VectorsToUnitCell(a, b, c);
                    }
                }
                else if (line.StartsWith("PRIMCOORD") || line.StartsWith("ATOMS"))
                {
                    // Next line contains atom count (and optionally step number)
                    if (i + 1 < lines.Length)
                    {
                        var parts = lines[++i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (int.TryParse(parts[0], out int atomCount))
                        {
                            // Parse atoms
                            for (int j = 0; j < atomCount && i + 1 < lines.Length; j++)
                            {
                                var atom = ParseAtomLine(lines[++i]);
                                if (atom != null)
                                    structure.AddAtom(atom);
                            }
                        }
                    }
                }
            }

            return structure;
        }

        private static Atom ParseAtomLine(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
                return null;

            // First column can be atomic number or element symbol
            Element element;
            if (int.TryParse(parts[0], out int atomicNumber))
            {
                element = (Element)atomicNumber;
            }
            else
            {
                element = PeriodicTable.ParseSymbol(parts[0]);
            }

            if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                return null;
            if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                return null;
            if (!float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                return null;

            return new Atom(element, x, y, z)
            {
                Label = element.ToString()
            };
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
