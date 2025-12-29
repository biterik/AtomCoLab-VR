// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.Collections.Generic;
using AtomCoLab.Core;
using UnityEngine;

namespace AtomCoLab.Interaction
{
    /// <summary>
    /// Event data for atom selection.
    /// </summary>
    public class AtomSelectionEventArgs : EventArgs
    {
        public Atom Atom { get; }
        public GameObject AtomObject { get; }
        public int AtomIndex { get; }

        public AtomSelectionEventArgs(Atom atom, GameObject atomObject, int atomIndex)
        {
            Atom = atom;
            AtomObject = atomObject;
            AtomIndex = atomIndex;
        }
    }

    /// <summary>
    /// Handles selection of individual atoms for measurement and inspection.
    /// </summary>
    public class AtomSelector : MonoBehaviour
    {
        [Header("Selection Settings")]
        [SerializeField]
        private int _maxSelections = 4;

        [SerializeField]
        private Color _selectionColor = Color.yellow;

        [SerializeField]
        private float _highlightScale = 1.2f;

        private readonly List<(GameObject obj, Atom atom, int index)> _selectedAtoms = new();
        private readonly Dictionary<GameObject, Color> _originalColors = new();
        private readonly Dictionary<GameObject, Vector3> _originalScales = new();

        public event EventHandler<AtomSelectionEventArgs> AtomSelected;
        public event EventHandler<AtomSelectionEventArgs> AtomDeselected;
        public event EventHandler SelectionCleared;

        /// <summary>
        /// Gets the currently selected atoms.
        /// </summary>
        public IReadOnlyList<(GameObject obj, Atom atom, int index)> SelectedAtoms => _selectedAtoms;

        /// <summary>
        /// Gets the number of selected atoms.
        /// </summary>
        public int SelectionCount => _selectedAtoms.Count;

        /// <summary>
        /// Selects or deselects an atom.
        /// </summary>
        public void ToggleSelection(GameObject atomObject, Atom atom, int atomIndex)
        {
            int existingIndex = _selectedAtoms.FindIndex(s => s.obj == atomObject);

            if (existingIndex >= 0)
            {
                Deselect(existingIndex);
            }
            else
            {
                Select(atomObject, atom, atomIndex);
            }
        }

        /// <summary>
        /// Selects an atom.
        /// </summary>
        public void Select(GameObject atomObject, Atom atom, int atomIndex)
        {
            if (_selectedAtoms.Count >= _maxSelections)
            {
                Deselect(0);
            }

            _selectedAtoms.Add((atomObject, atom, atomIndex));
            HighlightAtom(atomObject, true);

            AtomSelected?.Invoke(this, new AtomSelectionEventArgs(atom, atomObject, atomIndex));
        }

        /// <summary>
        /// Deselects an atom by index.
        /// </summary>
        public void Deselect(int selectionIndex)
        {
            if (selectionIndex < 0 || selectionIndex >= _selectedAtoms.Count)
                return;

            var (obj, atom, index) = _selectedAtoms[selectionIndex];
            _selectedAtoms.RemoveAt(selectionIndex);
            HighlightAtom(obj, false);

            AtomDeselected?.Invoke(this, new AtomSelectionEventArgs(atom, obj, index));
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        public void ClearSelection()
        {
            foreach (var (obj, _, _) in _selectedAtoms)
            {
                HighlightAtom(obj, false);
            }
            _selectedAtoms.Clear();

            SelectionCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calculates distance between two selected atoms.
        /// </summary>
        public float? GetDistance()
        {
            if (_selectedAtoms.Count < 2)
                return null;

            return Vector3.Distance(
                _selectedAtoms[0].atom.Position,
                _selectedAtoms[1].atom.Position
            );
        }

        /// <summary>
        /// Calculates angle between three selected atoms.
        /// </summary>
        public float? GetAngle()
        {
            if (_selectedAtoms.Count < 3)
                return null;

            Vector3 v1 = _selectedAtoms[0].atom.Position - _selectedAtoms[1].atom.Position;
            Vector3 v2 = _selectedAtoms[2].atom.Position - _selectedAtoms[1].atom.Position;

            return Vector3.Angle(v1, v2);
        }

        /// <summary>
        /// Calculates dihedral angle between four selected atoms.
        /// </summary>
        public float? GetDihedralAngle()
        {
            if (_selectedAtoms.Count < 4)
                return null;

            Vector3 b1 = _selectedAtoms[1].atom.Position - _selectedAtoms[0].atom.Position;
            Vector3 b2 = _selectedAtoms[2].atom.Position - _selectedAtoms[1].atom.Position;
            Vector3 b3 = _selectedAtoms[3].atom.Position - _selectedAtoms[2].atom.Position;

            Vector3 n1 = Vector3.Cross(b1, b2).normalized;
            Vector3 n2 = Vector3.Cross(b2, b3).normalized;

            float x = Vector3.Dot(n1, n2);
            float y = Vector3.Dot(Vector3.Cross(n1, b2.normalized), n2);

            return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        }

        private void HighlightAtom(GameObject atomObject, bool highlight)
        {
            if (atomObject == null)
                return;

            var renderer = atomObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (highlight)
                {
                    _originalColors[atomObject] = renderer.material.color;
                    renderer.material.color = _selectionColor;
                }
                else if (_originalColors.TryGetValue(atomObject, out Color originalColor))
                {
                    renderer.material.color = originalColor;
                    _originalColors.Remove(atomObject);
                }
            }

            if (highlight)
            {
                _originalScales[atomObject] = atomObject.transform.localScale;
                atomObject.transform.localScale *= _highlightScale;
            }
            else if (_originalScales.TryGetValue(atomObject, out Vector3 originalScale))
            {
                atomObject.transform.localScale = originalScale;
                _originalScales.Remove(atomObject);
            }
        }
    }
}
