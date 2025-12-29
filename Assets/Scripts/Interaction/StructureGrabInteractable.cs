// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace AtomCoLab.Interaction
{
    /// <summary>
    /// Enables VR interaction (grab, rotate, scale) with crystal structures.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class StructureGrabInteractable : XRGrabInteractable
    {
        [Header("Structure Interaction")]
        [SerializeField]
        private float _rotationSpeed = 100f;

        [SerializeField]
        private float _scaleSpeed = 1f;

        [SerializeField]
        private float _minScale = 0.1f;

        [SerializeField]
        private float _maxScale = 10f;

        [SerializeField]
        private bool _allowScaling = true;

        private Vector3 _initialScale;
        private bool _isSelected;

        protected override void Awake()
        {
            base.Awake();
            _initialScale = transform.localScale;

            var rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _isSelected = true;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            _isSelected = false;
        }

        /// <summary>
        /// Resets the structure to its initial scale.
        /// </summary>
        public void ResetScale()
        {
            transform.localScale = _initialScale;
        }

        /// <summary>
        /// Scales the structure by a factor.
        /// </summary>
        public void Scale(float factor)
        {
            if (!_allowScaling)
                return;

            Vector3 newScale = transform.localScale * factor;
            float magnitude = newScale.magnitude / Vector3.one.magnitude;

            if (magnitude >= _minScale && magnitude <= _maxScale)
            {
                transform.localScale = newScale;
            }
        }

        /// <summary>
        /// Rotates the structure around an axis.
        /// </summary>
        public void Rotate(Vector3 axis, float angle)
        {
            transform.Rotate(axis, angle * _rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
