// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System.Collections.Generic;
using AtomCoLab.Core;
using UnityEngine;

namespace AtomCoLab.Visualization
{
    /// <summary>
    /// Rendering modes for structure visualization.
    /// </summary>
    public enum RenderMode
    {
        BallAndStick,
        SpaceFilling,
        Wireframe,
        Polyhedral
    }

    /// <summary>
    /// Visualizes crystal structures in 3D.
    /// </summary>
    public class StructureVisualizer : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField]
        private GameObject _atomPrefab;

        [SerializeField]
        private GameObject _bondPrefab;

        [SerializeField]
        private GameObject _unitCellPrefab;

        [Header("Settings")]
        [SerializeField]
        private RenderMode _renderMode = RenderMode.BallAndStick;

        [SerializeField]
        private float _atomScale = 1f;

        [SerializeField]
        private float _bondRadius = 0.1f;

        [SerializeField]
        private bool _showUnitCell = true;

        [SerializeField]
        private bool _showBonds = true;

        private Structure _structure;
        private readonly List<GameObject> _atomObjects = new();
        private readonly List<GameObject> _bondObjects = new();
        private GameObject _unitCellObject;

        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        public RenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                _renderMode = value;
                if (_structure != null)
                    Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the atom scale multiplier.
        /// </summary>
        public float AtomScale
        {
            get => _atomScale;
            set
            {
                _atomScale = value;
                UpdateAtomScales();
            }
        }

        /// <summary>
        /// Gets or sets whether to show unit cell.
        /// </summary>
        public bool ShowUnitCell
        {
            get => _showUnitCell;
            set
            {
                _showUnitCell = value;
                if (_unitCellObject != null)
                    _unitCellObject.SetActive(value);
            }
        }

        /// <summary>
        /// Gets or sets whether to show bonds.
        /// </summary>
        public bool ShowBonds
        {
            get => _showBonds;
            set
            {
                _showBonds = value;
                foreach (var bond in _bondObjects)
                    bond.SetActive(value);
            }
        }

        /// <summary>
        /// Renders the given structure.
        /// </summary>
        public void Render(Structure structure)
        {
            Clear();
            _structure = structure;

            if (structure == null)
                return;

            RenderAtoms();

            if (_showBonds)
                RenderBonds();

            if (_showUnitCell && structure.UnitCell != null)
                RenderUnitCell();

            CenterStructure();
        }

        /// <summary>
        /// Clears the current visualization.
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _atomObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            _atomObjects.Clear();

            foreach (var obj in _bondObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            _bondObjects.Clear();

            if (_unitCellObject != null)
            {
                Destroy(_unitCellObject);
                _unitCellObject = null;
            }
        }

        /// <summary>
        /// Refreshes the visualization with current settings.
        /// </summary>
        public void Refresh()
        {
            if (_structure != null)
                Render(_structure);
        }

        private void RenderAtoms()
        {
            foreach (var atom in _structure.Atoms)
            {
                var obj = _atomPrefab != null
                    ? Instantiate(_atomPrefab, transform)
                    : GameObject.CreatePrimitive(PrimitiveType.Sphere);

                obj.transform.SetParent(transform);
                obj.transform.localPosition = atom.Position;
                obj.name = $"{atom.Element}_{atom.Index}";

                float radius = GetAtomRadius(atom);
                obj.transform.localScale = Vector3.one * radius * 2f * _atomScale;

                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = atom.CPKColor;
                }

                _atomObjects.Add(obj);
            }
        }

        private void RenderBonds()
        {
            foreach (var bond in _structure.Bonds)
            {
                var atom1 = _structure.Atoms[bond.AtomIndex1];
                var atom2 = _structure.Atoms[bond.AtomIndex2];

                var obj = _bondPrefab != null
                    ? Instantiate(_bondPrefab, transform)
                    : GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                obj.transform.SetParent(transform);
                obj.name = $"Bond_{bond.AtomIndex1}_{bond.AtomIndex2}";

                Vector3 start = atom1.Position;
                Vector3 end = atom2.Position;
                Vector3 direction = end - start;
                float length = direction.magnitude;

                obj.transform.localPosition = (start + end) / 2f;
                obj.transform.localRotation = Quaternion.FromToRotation(Vector3.up, direction);
                obj.transform.localScale = new Vector3(_bondRadius * 2f, length / 2f, _bondRadius * 2f);

                _bondObjects.Add(obj);
            }
        }

        private void RenderUnitCell()
        {
            if (_unitCellPrefab != null)
            {
                _unitCellObject = Instantiate(_unitCellPrefab, transform);
            }
            else
            {
                _unitCellObject = new GameObject("UnitCell");
                _unitCellObject.transform.SetParent(transform);

                var lineRenderer = _unitCellObject.AddComponent<LineRenderer>();
                lineRenderer.positionCount = 0;
            }

            _unitCellObject.SetActive(_showUnitCell);
        }

        private void CenterStructure()
        {
            if (_structure.AtomCount == 0)
                return;

            Vector3 center = _structure.GetCenterOfMass();
            transform.position -= center;
        }

        private float GetAtomRadius(Atom atom)
        {
            return _renderMode switch
            {
                RenderMode.SpaceFilling => atom.VanDerWaalsRadius,
                RenderMode.BallAndStick => atom.CovalentRadius * 0.5f,
                RenderMode.Wireframe => atom.CovalentRadius * 0.2f,
                RenderMode.Polyhedral => atom.CovalentRadius * 0.3f,
                _ => atom.CovalentRadius * 0.5f
            };
        }

        private void UpdateAtomScales()
        {
            for (int i = 0; i < _atomObjects.Count && i < _structure.AtomCount; i++)
            {
                var atom = _structure.Atoms[i];
                float radius = GetAtomRadius(atom);
                _atomObjects[i].transform.localScale = Vector3.one * radius * 2f * _atomScale;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
