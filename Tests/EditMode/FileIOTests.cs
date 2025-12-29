// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using AtomCoLab.Core;
using AtomCoLab.FileIO;
using NUnit.Framework;

namespace AtomCoLab.Tests
{
    [TestFixture]
    public class XYZParserTests
    {
        [Test]
        public void XYZParser_Parse_BasicFile()
        {
            string content = @"3
Water molecule
O     0.000000     0.000000     0.117369
H     0.756950     0.000000    -0.469476
H    -0.756950     0.000000    -0.469476";

            var structure = XYZParser.Parse(content, "test");

            Assert.AreEqual(3, structure.AtomCount);
            Assert.AreEqual(Element.O, structure.Atoms[0].Element);
            Assert.AreEqual(Element.H, structure.Atoms[1].Element);
            Assert.AreEqual(Element.H, structure.Atoms[2].Element);
        }

        [Test]
        public void XYZParser_Parse_SetsStructureName()
        {
            string content = @"1
Test Structure Name
C     0.0     0.0     0.0";

            var structure = XYZParser.Parse(content);

            Assert.AreEqual("Test Structure Name", structure.Name);
        }
    }

    [TestFixture]
    public class CIFParserTests
    {
        [Test]
        public void CIFParser_Parse_ExtractsUnitCell()
        {
            string content = @"
data_test
_cell_length_a 5.431
_cell_length_b 5.431
_cell_length_c 5.431
_cell_angle_alpha 90.0
_cell_angle_beta 90.0
_cell_angle_gamma 90.0
_symmetry_space_group_name_H-M 'Fd-3m'
";

            var structure = CIFParser.Parse(content, "Silicon");

            Assert.IsNotNull(structure.UnitCell);
            Assert.AreEqual(5.431f, structure.UnitCell.A, 0.001f);
            Assert.AreEqual(90f, structure.UnitCell.Alpha, 0.001f);
            Assert.AreEqual("Fd-3m", structure.SpaceGroup);
        }
    }

    [TestFixture]
    public class StructureLoaderTests
    {
        [Test]
        public void StructureLoader_DetectFormat_RecognizesCIF()
        {
            Assert.AreEqual(FileFormat.CIF, StructureLoader.DetectFormat("test.cif"));
            Assert.AreEqual(FileFormat.CIF, StructureLoader.DetectFormat("path/to/structure.CIF"));
        }

        [Test]
        public void StructureLoader_DetectFormat_RecognizesXYZ()
        {
            Assert.AreEqual(FileFormat.XYZ, StructureLoader.DetectFormat("test.xyz"));
        }

        [Test]
        public void StructureLoader_DetectFormat_RecognizesPOSCAR()
        {
            Assert.AreEqual(FileFormat.POSCAR, StructureLoader.DetectFormat("POSCAR"));
            Assert.AreEqual(FileFormat.POSCAR, StructureLoader.DetectFormat("CONTCAR"));
            Assert.AreEqual(FileFormat.POSCAR, StructureLoader.DetectFormat("test.vasp"));
        }

        [Test]
        public void StructureLoader_DetectFormat_ReturnsUnknownForUnrecognized()
        {
            Assert.AreEqual(FileFormat.Unknown, StructureLoader.DetectFormat("test.txt"));
            Assert.AreEqual(FileFormat.Unknown, StructureLoader.DetectFormat("test.pdf"));
        }
    }
}
