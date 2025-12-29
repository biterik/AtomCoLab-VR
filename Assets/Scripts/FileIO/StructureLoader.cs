// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.IO;
using AtomCoLab.Core;
using UnityEngine;

namespace AtomCoLab.FileIO
{
    /// <summary>
    /// Supported file formats for structure import.
    /// </summary>
    public enum FileFormat
    {
        CIF,
        XYZ,
        POSCAR,
        XSF,
        Unknown
    }

    /// <summary>
    /// Loads crystal structures from various file formats.
    /// </summary>
    public static class StructureLoader
    {
        /// <summary>
        /// Loads a structure from a file, auto-detecting the format.
        /// </summary>
        public static Structure Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Structure file not found: {filePath}");

            var format = DetectFormat(filePath);
            return Load(filePath, format);
        }

        /// <summary>
        /// Loads a structure from a file with specified format.
        /// </summary>
        public static Structure Load(string filePath, FileFormat format)
        {
            string content = File.ReadAllText(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            return format switch
            {
                FileFormat.CIF => CIFParser.Parse(content, fileName),
                FileFormat.XYZ => XYZParser.Parse(content, fileName),
                FileFormat.POSCAR => POSCARParser.Parse(content, fileName),
                FileFormat.XSF => XSFParser.Parse(content, fileName),
                _ => throw new NotSupportedException($"Unsupported file format: {format}")
            };
        }

        /// <summary>
        /// Loads a structure from CIF file.
        /// </summary>
        public static Structure LoadFromCIF(string filePath)
        {
            return Load(filePath, FileFormat.CIF);
        }

        /// <summary>
        /// Loads a structure from XYZ file.
        /// </summary>
        public static Structure LoadFromXYZ(string filePath)
        {
            return Load(filePath, FileFormat.XYZ);
        }

        /// <summary>
        /// Loads a structure from VASP POSCAR/CONTCAR file.
        /// </summary>
        public static Structure LoadFromPOSCAR(string filePath)
        {
            return Load(filePath, FileFormat.POSCAR);
        }

        /// <summary>
        /// Loads a structure from XSF file.
        /// </summary>
        public static Structure LoadFromXSF(string filePath)
        {
            return Load(filePath, FileFormat.XSF);
        }

        /// <summary>
        /// Parses a structure from string content.
        /// </summary>
        public static Structure ParseFromString(string content, FileFormat format, string name = "Structure")
        {
            return format switch
            {
                FileFormat.CIF => CIFParser.Parse(content, name),
                FileFormat.XYZ => XYZParser.Parse(content, name),
                FileFormat.POSCAR => POSCARParser.Parse(content, name),
                FileFormat.XSF => XSFParser.Parse(content, name),
                _ => throw new NotSupportedException($"Unsupported file format: {format}")
            };
        }

        /// <summary>
        /// Detects the file format from file extension.
        /// </summary>
        public static FileFormat DetectFormat(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string fileName = Path.GetFileName(filePath).ToUpperInvariant();

            if (fileName == "POSCAR" || fileName == "CONTCAR")
                return FileFormat.POSCAR;

            return extension switch
            {
                ".cif" => FileFormat.CIF,
                ".xyz" => FileFormat.XYZ,
                ".vasp" => FileFormat.POSCAR,
                ".xsf" => FileFormat.XSF,
                _ => FileFormat.Unknown
            };
        }
    }
}
