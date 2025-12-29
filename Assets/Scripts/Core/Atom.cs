// AtomCoLab-VR - Molecular Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using UnityEngine;

namespace AtomCoLab.Core
{
    /// <summary>
    /// Represents an atom in a molecular structure.
    /// </summary>
    [Serializable]
    public class Atom
    {
        [SerializeField]
        private Element _element;

        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private int _index;

        [SerializeField]
        private string _label;

        [SerializeField]
        private int _residueId;

        [SerializeField]
        private string _residueName;

        [SerializeField]
        private string _chainId;

        /// <summary>
        /// Gets or sets the chemical element.
        /// </summary>
        public Element Element
        {
            get => _element;
            set => _element = value;
        }

        /// <summary>
        /// Gets or sets the 3D position in angstroms.
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Gets or sets the atom index within the molecule.
        /// </summary>
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        /// <summary>
        /// Gets or sets the atom label (e.g., "CA" for alpha carbon).
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value;
        }

        /// <summary>
        /// Gets or sets the residue ID (for proteins/nucleic acids).
        /// </summary>
        public int ResidueId
        {
            get => _residueId;
            set => _residueId = value;
        }

        /// <summary>
        /// Gets or sets the residue name (e.g., "ALA", "GLY").
        /// </summary>
        public string ResidueName
        {
            get => _residueName;
            set => _residueName = value;
        }

        /// <summary>
        /// Gets or sets the chain identifier.
        /// </summary>
        public string ChainId
        {
            get => _chainId;
            set => _chainId = value;
        }

        /// <summary>
        /// Gets the van der Waals radius for this atom.
        /// </summary>
        public float VanDerWaalsRadius => PeriodicTable.GetVanDerWaalsRadius(_element);

        /// <summary>
        /// Gets the covalent radius for this atom.
        /// </summary>
        public float CovalentRadius => PeriodicTable.GetCovalentRadius(_element);

        /// <summary>
        /// Gets the CPK color for this atom.
        /// </summary>
        public Color CPKColor => PeriodicTable.GetCPKColor(_element);

        public Atom() { }

        public Atom(Element element, Vector3 position)
        {
            _element = element;
            _position = position;
        }

        public Atom(Element element, float x, float y, float z)
        {
            _element = element;
            _position = new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"{_element}[{_index}] at {_position}";
        }
    }
}
