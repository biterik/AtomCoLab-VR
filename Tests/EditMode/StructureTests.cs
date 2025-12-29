// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using AtomCoLab.Core;
using NUnit.Framework;
using UnityEngine;

namespace AtomCoLab.Tests
{
    [TestFixture]
    public class StructureTests
    {
        [Test]
        public void Structure_AddAtom_IncreasesAtomCount()
        {
            var structure = new Structure();

            structure.AddAtom(new Atom(Element.C, Vector3.zero));

            Assert.AreEqual(1, structure.AtomCount);
        }

        [Test]
        public void Structure_AddBond_IncreasesBondCount()
        {
            var structure = new Structure();
            structure.AddAtom(new Atom(Element.C, Vector3.zero));
            structure.AddAtom(new Atom(Element.O, Vector3.one));

            structure.AddBond(0, 1);

            Assert.AreEqual(1, structure.BondCount);
        }

        [Test]
        public void Structure_Clear_RemovesAllAtomsAndBonds()
        {
            var structure = new Structure();
            structure.AddAtom(new Atom(Element.C, Vector3.zero));
            structure.AddAtom(new Atom(Element.O, Vector3.one));
            structure.AddBond(0, 1);

            structure.Clear();

            Assert.AreEqual(0, structure.AtomCount);
            Assert.AreEqual(0, structure.BondCount);
        }

        [Test]
        public void Structure_GetCenterOfMass_ReturnsCorrectCenter()
        {
            var structure = new Structure();
            structure.AddAtom(new Atom(Element.C, new Vector3(0, 0, 0)));
            structure.AddAtom(new Atom(Element.C, new Vector3(2, 0, 0)));

            var center = structure.GetCenterOfMass();

            Assert.AreEqual(1f, center.x, 0.001f);
            Assert.AreEqual(0f, center.y, 0.001f);
            Assert.AreEqual(0f, center.z, 0.001f);
        }

        [Test]
        public void Structure_GetBounds_EncapsulatesAllAtoms()
        {
            var structure = new Structure();
            structure.AddAtom(new Atom(Element.C, new Vector3(-1, -1, -1)));
            structure.AddAtom(new Atom(Element.C, new Vector3(1, 1, 1)));

            var bounds = structure.GetBounds();

            Assert.AreEqual(Vector3.zero, bounds.center);
            Assert.AreEqual(new Vector3(2, 2, 2), bounds.size);
        }
    }

    [TestFixture]
    public class UnitCellTests
    {
        [Test]
        public void UnitCell_Cubic_CreatesCorrectParameters()
        {
            var cell = UnitCell.Cubic(5f);

            Assert.AreEqual(5f, cell.A);
            Assert.AreEqual(5f, cell.B);
            Assert.AreEqual(5f, cell.C);
            Assert.AreEqual(90f, cell.Alpha);
            Assert.AreEqual(90f, cell.Beta);
            Assert.AreEqual(90f, cell.Gamma);
        }

        [Test]
        public void UnitCell_Volume_CalculatesCorrectly()
        {
            var cell = UnitCell.Cubic(2f);

            Assert.AreEqual(8f, cell.Volume, 0.001f);
        }

        [Test]
        public void UnitCell_FractionalToCartesian_ConvertsCorrectly()
        {
            var cell = UnitCell.Cubic(10f);

            var cartesian = cell.FractionalToCartesian(new Vector3(0.5f, 0.5f, 0.5f));

            Assert.AreEqual(5f, cartesian.x, 0.001f);
            Assert.AreEqual(5f, cartesian.y, 0.001f);
            Assert.AreEqual(5f, cartesian.z, 0.001f);
        }
    }

    [TestFixture]
    public class PeriodicTableTests
    {
        [Test]
        public void PeriodicTable_ParseSymbol_RecognizesCommonElements()
        {
            Assert.AreEqual(Element.C, PeriodicTable.ParseSymbol("C"));
            Assert.AreEqual(Element.O, PeriodicTable.ParseSymbol("O"));
            Assert.AreEqual(Element.Fe, PeriodicTable.ParseSymbol("Fe"));
            Assert.AreEqual(Element.Au, PeriodicTable.ParseSymbol("Au"));
        }

        [Test]
        public void PeriodicTable_ParseSymbol_HandlesLowercase()
        {
            Assert.AreEqual(Element.Fe, PeriodicTable.ParseSymbol("fe"));
            Assert.AreEqual(Element.Au, PeriodicTable.ParseSymbol("au"));
        }

        [Test]
        public void PeriodicTable_ParseSymbol_ReturnsUnknownForInvalid()
        {
            Assert.AreEqual(Element.Unknown, PeriodicTable.ParseSymbol("Xx"));
            Assert.AreEqual(Element.Unknown, PeriodicTable.ParseSymbol(""));
        }

        [Test]
        public void PeriodicTable_GetAtomicMass_ReturnsCorrectValues()
        {
            Assert.AreEqual(12.011f, PeriodicTable.GetAtomicMass(Element.C), 0.001f);
            Assert.AreEqual(15.999f, PeriodicTable.GetAtomicMass(Element.O), 0.001f);
        }
    }
}
