// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AtomCoLab.Core
{
    /// <summary>
    /// Represents a crystal structure with atoms, bonds, and unit cell.
    /// </summary>
    [Serializable]
    public class Structure
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private List<Atom> _atoms = new();

        [SerializeField]
        private List<Bond> _bonds = new();

        [SerializeField]
        private UnitCell _unitCell;

        [SerializeField]
        private string _spaceGroup;

        /// <summary>
        /// Gets or sets the structure name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Gets the list of atoms in this structure.
        /// </summary>
        public IReadOnlyList<Atom> Atoms => _atoms;

        /// <summary>
        /// Gets the list of bonds in this structure.
        /// </summary>
        public IReadOnlyList<Bond> Bonds => _bonds;

        /// <summary>
        /// Gets or sets the unit cell parameters.
        /// </summary>
        public UnitCell UnitCell
        {
            get => _unitCell;
            set => _unitCell = value;
        }

        /// <summary>
        /// Gets or sets the space group (e.g., "Fm-3m", "P21/c").
        /// </summary>
        public string SpaceGroup
        {
            get => _spaceGroup;
            set => _spaceGroup = value;
        }

        /// <summary>
        /// Gets the total number of atoms.
        /// </summary>
        public int AtomCount => _atoms.Count;

        /// <summary>
        /// Gets the total number of bonds.
        /// </summary>
        public int BondCount => _bonds.Count;

        /// <summary>
        /// Adds an atom to the structure.
        /// </summary>
        public void AddAtom(Atom atom)
        {
            if (atom == null)
                throw new ArgumentNullException(nameof(atom));

            atom.Index = _atoms.Count;
            _atoms.Add(atom);
        }

        /// <summary>
        /// Adds a bond between two atoms.
        /// </summary>
        public void AddBond(int atomIndex1, int atomIndex2, BondOrder order = BondOrder.Single)
        {
            if (atomIndex1 < 0 || atomIndex1 >= _atoms.Count)
                throw new ArgumentOutOfRangeException(nameof(atomIndex1));
            if (atomIndex2 < 0 || atomIndex2 >= _atoms.Count)
                throw new ArgumentOutOfRangeException(nameof(atomIndex2));

            var bond = new Bond(atomIndex1, atomIndex2, order);
            _bonds.Add(bond);
        }

        /// <summary>
        /// Calculates the center of mass of the structure.
        /// </summary>
        public Vector3 GetCenterOfMass()
        {
            if (_atoms.Count == 0)
                return Vector3.zero;

            Vector3 center = Vector3.zero;
            float totalMass = 0f;

            foreach (var atom in _atoms)
            {
                float mass = PeriodicTable.GetAtomicMass(atom.Element);
                center += atom.Position * mass;
                totalMass += mass;
            }

            return totalMass > 0 ? center / totalMass : Vector3.zero;
        }

        /// <summary>
        /// Calculates the bounding box of the structure.
        /// </summary>
        public Bounds GetBounds()
        {
            if (_atoms.Count == 0)
                return new Bounds();

            var bounds = new Bounds(_atoms[0].Position, Vector3.zero);
            foreach (var atom in _atoms)
            {
                bounds.Encapsulate(atom.Position);
            }

            return bounds;
        }

        /// <summary>
        /// Generates a supercell by replicating the unit cell.
        /// </summary>
        /// <param name="nx">Replications along a-axis</param>
        /// <param name="ny">Replications along b-axis</param>
        /// <param name="nz">Replications along c-axis</param>
        public Structure GenerateSupercell(int nx, int ny, int nz)
        {
            if (_unitCell == null)
                throw new InvalidOperationException("Unit cell not defined");

            var supercell = new Structure
            {
                Name = $"{_name}_{nx}x{ny}x{nz}",
                SpaceGroup = _spaceGroup
            };

            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    for (int k = 0; k < nz; k++)
                    {
                        Vector3 offset = _unitCell.FractionalToCartesian(new Vector3(i, j, k));

                        foreach (var atom in _atoms)
                        {
                            var newAtom = new Atom(atom.Element, atom.Position + offset)
                            {
                                Label = atom.Label
                            };
                            supercell.AddAtom(newAtom);
                        }
                    }
                }
            }

            return supercell;
        }

        /// <summary>
        /// Clears all atoms and bonds.
        /// </summary>
        public void Clear()
        {
            _atoms.Clear();
            _bonds.Clear();
        }
    }
}
