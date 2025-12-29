// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using UnityEngine;

namespace AtomCoLab.Core
{
    /// <summary>
    /// Represents a crystallographic unit cell with lattice parameters.
    /// </summary>
    [Serializable]
    public class UnitCell
    {
        [SerializeField]
        private float _a;

        [SerializeField]
        private float _b;

        [SerializeField]
        private float _c;

        [SerializeField]
        private float _alpha;

        [SerializeField]
        private float _beta;

        [SerializeField]
        private float _gamma;

        /// <summary>
        /// Length of the a-axis in angstroms.
        /// </summary>
        public float A
        {
            get => _a;
            set => _a = value;
        }

        /// <summary>
        /// Length of the b-axis in angstroms.
        /// </summary>
        public float B
        {
            get => _b;
            set => _b = value;
        }

        /// <summary>
        /// Length of the c-axis in angstroms.
        /// </summary>
        public float C
        {
            get => _c;
            set => _c = value;
        }

        /// <summary>
        /// Angle between b and c axes in degrees.
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set => _alpha = value;
        }

        /// <summary>
        /// Angle between a and c axes in degrees.
        /// </summary>
        public float Beta
        {
            get => _beta;
            set => _beta = value;
        }

        /// <summary>
        /// Angle between a and b axes in degrees.
        /// </summary>
        public float Gamma
        {
            get => _gamma;
            set => _gamma = value;
        }

        /// <summary>
        /// Gets the unit cell volume in cubic angstroms.
        /// </summary>
        public float Volume
        {
            get
            {
                float alphaRad = _alpha * Mathf.Deg2Rad;
                float betaRad = _beta * Mathf.Deg2Rad;
                float gammaRad = _gamma * Mathf.Deg2Rad;

                float cosAlpha = Mathf.Cos(alphaRad);
                float cosBeta = Mathf.Cos(betaRad);
                float cosGamma = Mathf.Cos(gammaRad);

                float term = 1 - cosAlpha * cosAlpha - cosBeta * cosBeta - cosGamma * cosGamma
                           + 2 * cosAlpha * cosBeta * cosGamma;

                return _a * _b * _c * Mathf.Sqrt(term);
            }
        }

        public UnitCell() : this(1, 1, 1, 90, 90, 90) { }

        public UnitCell(float a, float b, float c, float alpha, float beta, float gamma)
        {
            _a = a;
            _b = b;
            _c = c;
            _alpha = alpha;
            _beta = beta;
            _gamma = gamma;
        }

        /// <summary>
        /// Creates a cubic unit cell.
        /// </summary>
        public static UnitCell Cubic(float a)
        {
            return new UnitCell(a, a, a, 90, 90, 90);
        }

        /// <summary>
        /// Creates a tetragonal unit cell.
        /// </summary>
        public static UnitCell Tetragonal(float a, float c)
        {
            return new UnitCell(a, a, c, 90, 90, 90);
        }

        /// <summary>
        /// Creates an orthorhombic unit cell.
        /// </summary>
        public static UnitCell Orthorhombic(float a, float b, float c)
        {
            return new UnitCell(a, b, c, 90, 90, 90);
        }

        /// <summary>
        /// Creates a hexagonal unit cell.
        /// </summary>
        public static UnitCell Hexagonal(float a, float c)
        {
            return new UnitCell(a, a, c, 90, 90, 120);
        }

        /// <summary>
        /// Gets the transformation matrix from fractional to Cartesian coordinates.
        /// </summary>
        public Matrix4x4 GetTransformationMatrix()
        {
            float alphaRad = _alpha * Mathf.Deg2Rad;
            float betaRad = _beta * Mathf.Deg2Rad;
            float gammaRad = _gamma * Mathf.Deg2Rad;

            float cosAlpha = Mathf.Cos(alphaRad);
            float cosBeta = Mathf.Cos(betaRad);
            float cosGamma = Mathf.Cos(gammaRad);
            float sinGamma = Mathf.Sin(gammaRad);

            float v = Volume / (_a * _b * _c);

            var matrix = new Matrix4x4();
            matrix.SetColumn(0, new Vector4(_a, 0, 0, 0));
            matrix.SetColumn(1, new Vector4(_b * cosGamma, _b * sinGamma, 0, 0));
            matrix.SetColumn(2, new Vector4(
                _c * cosBeta,
                _c * (cosAlpha - cosBeta * cosGamma) / sinGamma,
                _c * v / sinGamma,
                0
            ));
            matrix.SetColumn(3, new Vector4(0, 0, 0, 1));

            return matrix;
        }

        /// <summary>
        /// Converts fractional coordinates to Cartesian coordinates.
        /// </summary>
        public Vector3 FractionalToCartesian(Vector3 fractional)
        {
            var matrix = GetTransformationMatrix();
            return matrix.MultiplyPoint3x4(fractional);
        }

        /// <summary>
        /// Converts Cartesian coordinates to fractional coordinates.
        /// </summary>
        public Vector3 CartesianToFractional(Vector3 cartesian)
        {
            var matrix = GetTransformationMatrix().inverse;
            return matrix.MultiplyPoint3x4(cartesian);
        }

        /// <summary>
        /// Gets the lattice vectors (a, b, c) as Cartesian vectors.
        /// </summary>
        public (Vector3 a, Vector3 b, Vector3 c) GetLatticeVectors()
        {
            var matrix = GetTransformationMatrix();
            return (
                matrix.MultiplyPoint3x4(Vector3.right),
                matrix.MultiplyPoint3x4(Vector3.up),
                matrix.MultiplyPoint3x4(Vector3.forward)
            );
        }

        public override string ToString()
        {
            return $"UnitCell(a={_a:F3}, b={_b:F3}, c={_c:F3}, " +
                   $"alpha={_alpha:F1}, beta={_beta:F1}, gamma={_gamma:F1})";
        }
    }
}
