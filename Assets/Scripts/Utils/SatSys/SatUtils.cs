using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SatSys
{
    public static class SatUtils
    {
        /// <summary>
        /// Newtons Gravitational Constant (m^3 * kg^-1 * s^-2)
        /// </summary>
        public const double G = 6.6743e-11F;

        /// <summary>
        /// Mass of Earth (kg)
        /// </summary>
        public const double M = 5.9721e24F;

        /// <summary>
        /// Standard Gravitational Parameter
        /// </summary>
        public static double mu => G * M;

        public const double AU = 1.495978707e11;

        public static Vector3 D2F(double3 vector) =>
            new Vector3((float)vector.x, (float)vector.y, (float)vector.z);

        public static double SquareMagnitude(double3 vector) =>
            vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
    }
}
