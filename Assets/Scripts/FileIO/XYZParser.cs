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
    /// Parser for XYZ file format.
    /// </summary>
    public static class XYZParser
    {
        /// <summary>
        /// Parses XYZ content into a Structure.
        /// </summary>
        public static Structure Parse(string content, string name = "Structure")
        {
            var structure = new Structure { Name = name };
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
                throw new FormatException("Invalid XYZ file: too few lines");

            // First line: number of atoms
            if (!int.TryParse(lines[0].Trim(), out int atomCount))
                throw new FormatException("Invalid XYZ file: first line must be atom count");

            // Second line: comment (may contain lattice info)
            string comment = lines.Length > 1 ? lines[1].Trim() : "";
            structure.Name = string.IsNullOrEmpty(comment) ? name : comment;

            // Parse lattice from extended XYZ format
            ParseLattice(comment, structure);

            // Parse atoms starting from line 3
            for (int i = 2; i < lines.Length && i < atomCount + 2; i++)
            {
                var atom = ParseAtomLine(lines[i]);
                if (atom != null)
                    structure.AddAtom(atom);
            }

            return structure;
        }

        private static Atom ParseAtomLine(string line)
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
                return null;

            Element element = PeriodicTable.ParseSymbol(parts[0]);

            if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                return null;
            if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                return null;
            if (!float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                return null;

            return new Atom(element, x, y, z)
            {
                Label = parts[0]
            };
        }

        private static void ParseLattice(string comment, Structure structure)
        {
            // Try to parse extended XYZ lattice format: Lattice="a1 a2 a3 b1 b2 b3 c1 c2 c3"
            if (comment.Contains("Lattice="))
            {
                int start = comment.IndexOf("Lattice=") + 9;
                int end = comment.IndexOf('"', start);
                if (end > start)
                {
                    string latticeStr = comment.Substring(start, end - start);
                    var values = latticeStr.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length >= 9)
                    {
                        float[] v = new float[9];
                        bool valid = true;
                        for (int i = 0; i < 9; i++)
                        {
                            if (!float.TryParse(values[i], NumberStyles.Float, CultureInfo.InvariantCulture, out v[i]))
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid)
                        {
                            Vector3 a = new Vector3(v[0], v[1], v[2]);
                            Vector3 b = new Vector3(v[3], v[4], v[5]);
                            Vector3 c = new Vector3(v[6], v[7], v[8]);

                            structure.UnitCell = VectorsToUnitCell(a, b, c);
                        }
                    }
                }
            }
        }

        private static UnitCell VectorsToUnitCell(Vector3 a, Vector3 b, Vector3 c)
        {
            float aLen = a.magnitude;
            float bLen = b.magnitude;
            float cLen = c.magnitude;

            float alpha = Mathf.Acos(Vector3.Dot(b.normalized, c.normalized)) * Mathf.Rad2Deg;
            float beta = Mathf.Acos(Vector3.Dot(a.normalized, c.normalized)) * Mathf.Rad2Deg;
            float gamma = Mathf.Acos(Vector3.Dot(a.normalized, b.normalized)) * Mathf.Rad2Deg;

            return new UnitCell(aLen, bLen, cLen, alpha, beta, gamma);
        }
    }
}
