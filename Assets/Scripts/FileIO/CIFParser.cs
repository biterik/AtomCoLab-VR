// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using AtomCoLab.Core;
using UnityEngine;

namespace AtomCoLab.FileIO
{
    /// <summary>
    /// Parser for Crystallographic Information File (CIF) format.
    /// </summary>
    public static class CIFParser
    {
        private static readonly Regex NumberRegex = new(@"^-?\d+\.?\d*", RegexOptions.Compiled);

        /// <summary>
        /// Parses CIF content into a Structure.
        /// </summary>
        public static Structure Parse(string content, string name = "Structure")
        {
            var structure = new Structure { Name = name };
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var cifData = ParseCIFData(lines);

            // Parse unit cell parameters
            structure.UnitCell = ParseUnitCell(cifData);

            // Parse space group
            if (cifData.TryGetValue("_symmetry_space_group_name_h-m", out string spaceGroup))
                structure.SpaceGroup = spaceGroup.Trim('\'', '"');
            else if (cifData.TryGetValue("_space_group_name_h-m_alt", out spaceGroup))
                structure.SpaceGroup = spaceGroup.Trim('\'', '"');

            // Parse atoms
            ParseAtoms(cifData, structure);

            return structure;
        }

        private static Dictionary<string, string> ParseCIFData(string[] lines)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var loopColumns = new List<string>();
            var loopData = new Dictionary<string, List<string>>();
            bool inLoop = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                if (line.StartsWith("loop_"))
                {
                    inLoop = true;
                    loopColumns.Clear();
                    continue;
                }

                if (inLoop && line.StartsWith("_"))
                {
                    loopColumns.Add(line);
                    if (!loopData.ContainsKey(line))
                        loopData[line] = new List<string>();
                }
                else if (inLoop && loopColumns.Count > 0 && !line.StartsWith("_"))
                {
                    var values = SplitCIFLine(line);
                    if (values.Count >= loopColumns.Count)
                    {
                        for (int j = 0; j < loopColumns.Count; j++)
                        {
                            loopData[loopColumns[j]].Add(values[j]);
                        }
                    }
                    else if (line.StartsWith("data_") || line.StartsWith("loop_"))
                    {
                        inLoop = false;
                    }
                }
                else if (line.StartsWith("_"))
                {
                    inLoop = false;
                    var parts = SplitCIFLine(line);
                    if (parts.Count >= 2)
                    {
                        data[parts[0]] = parts[1];
                    }
                }
            }

            // Merge loop data into main dictionary
            foreach (var kvp in loopData)
            {
                data[kvp.Key] = string.Join("|", kvp.Value);
            }

            return data;
        }

        private static List<string> SplitCIFLine(string line)
        {
            var result = new List<string>();
            bool inQuote = false;
            char quoteChar = '"';
            string current = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if ((c == '"' || c == '\'') && !inQuote)
                {
                    inQuote = true;
                    quoteChar = c;
                }
                else if (c == quoteChar && inQuote)
                {
                    inQuote = false;
                    if (!string.IsNullOrEmpty(current))
                    {
                        result.Add(current);
                        current = "";
                    }
                }
                else if (char.IsWhiteSpace(c) && !inQuote)
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        result.Add(current);
                        current = "";
                    }
                }
                else
                {
                    current += c;
                }
            }

            if (!string.IsNullOrEmpty(current))
                result.Add(current);

            return result;
        }

        private static UnitCell ParseUnitCell(Dictionary<string, string> data)
        {
            float a = ParseCellParameter(data, "_cell_length_a", 1f);
            float b = ParseCellParameter(data, "_cell_length_b", 1f);
            float c = ParseCellParameter(data, "_cell_length_c", 1f);
            float alpha = ParseCellParameter(data, "_cell_angle_alpha", 90f);
            float beta = ParseCellParameter(data, "_cell_angle_beta", 90f);
            float gamma = ParseCellParameter(data, "_cell_angle_gamma", 90f);

            return new UnitCell(a, b, c, alpha, beta, gamma);
        }

        private static float ParseCellParameter(Dictionary<string, string> data, string key, float defaultValue)
        {
            if (!data.TryGetValue(key, out string value))
                return defaultValue;

            var match = NumberRegex.Match(value);
            if (match.Success && float.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                return result;

            return defaultValue;
        }

        private static void ParseAtoms(Dictionary<string, string> data, Structure structure)
        {
            // Try standard atom site labels
            string[] labelKeys = { "_atom_site_type_symbol", "_atom_site_label" };
            string[] xKeys = { "_atom_site_fract_x", "_atom_site_cartn_x" };
            string[] yKeys = { "_atom_site_fract_y", "_atom_site_cartn_y" };
            string[] zKeys = { "_atom_site_fract_z", "_atom_site_cartn_z" };

            string labelKey = null;
            string xKey = null, yKey = null, zKey = null;
            bool fractional = true;

            foreach (var key in labelKeys)
            {
                if (data.ContainsKey(key))
                {
                    labelKey = key;
                    break;
                }
            }

            for (int i = 0; i < xKeys.Length; i++)
            {
                if (data.ContainsKey(xKeys[i]))
                {
                    xKey = xKeys[i];
                    yKey = yKeys[i];
                    zKey = zKeys[i];
                    fractional = xKeys[i].Contains("fract");
                    break;
                }
            }

            if (labelKey == null || xKey == null)
                return;

            var labels = data[labelKey].Split('|');
            var xs = data[xKey].Split('|');
            var ys = data[yKey].Split('|');
            var zs = data[zKey].Split('|');

            for (int i = 0; i < labels.Length; i++)
            {
                if (i >= xs.Length || i >= ys.Length || i >= zs.Length)
                    break;

                float x = ParseCoordinate(xs[i]);
                float y = ParseCoordinate(ys[i]);
                float z = ParseCoordinate(zs[i]);

                Vector3 position = new Vector3(x, y, z);
                if (fractional && structure.UnitCell != null)
                {
                    position = structure.UnitCell.FractionalToCartesian(position);
                }

                Element element = PeriodicTable.ParseSymbol(labels[i]);
                var atom = new Atom(element, position)
                {
                    Label = labels[i]
                };

                structure.AddAtom(atom);
            }
        }

        private static float ParseCoordinate(string value)
        {
            var match = NumberRegex.Match(value);
            if (match.Success && float.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                return result;
            return 0f;
        }
    }
}
