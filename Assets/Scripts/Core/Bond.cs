// AtomCoLab-VR - Molecular Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using UnityEngine;

namespace AtomCoLab.Core
{
    /// <summary>
    /// Types of chemical bonds.
    /// </summary>
    public enum BondOrder
    {
        Single = 1,
        Double = 2,
        Triple = 3,
        Aromatic = 4,
        Unknown = 0
    }

    /// <summary>
    /// Represents a chemical bond between two atoms.
    /// </summary>
    [Serializable]
    public class Bond
    {
        [SerializeField]
        private int _atomIndex1;

        [SerializeField]
        private int _atomIndex2;

        [SerializeField]
        private BondOrder _order;

        /// <summary>
        /// Gets the index of the first atom.
        /// </summary>
        public int AtomIndex1 => _atomIndex1;

        /// <summary>
        /// Gets the index of the second atom.
        /// </summary>
        public int AtomIndex2 => _atomIndex2;

        /// <summary>
        /// Gets or sets the bond order.
        /// </summary>
        public BondOrder Order
        {
            get => _order;
            set => _order = value;
        }

        public Bond() { }

        public Bond(int atomIndex1, int atomIndex2, BondOrder order = BondOrder.Single)
        {
            _atomIndex1 = atomIndex1;
            _atomIndex2 = atomIndex2;
            _order = order;
        }

        /// <summary>
        /// Checks if this bond connects the specified atom.
        /// </summary>
        public bool ConnectsAtom(int atomIndex)
        {
            return _atomIndex1 == atomIndex || _atomIndex2 == atomIndex;
        }

        /// <summary>
        /// Gets the other atom connected by this bond.
        /// </summary>
        public int GetOtherAtom(int atomIndex)
        {
            if (_atomIndex1 == atomIndex)
                return _atomIndex2;
            if (_atomIndex2 == atomIndex)
                return _atomIndex1;

            throw new ArgumentException("Atom is not part of this bond", nameof(atomIndex));
        }

        public override string ToString()
        {
            return $"Bond({_atomIndex1}-{_atomIndex2}, {_order})";
        }
    }
}
