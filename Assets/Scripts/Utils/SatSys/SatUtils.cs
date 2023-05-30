using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Newtonsoft.Json;
using One_Sgp4;

namespace SatSys
{
    public static class SatUtils
    {
        /// <summary>
        /// Newtons Gravitational Constant (m^3 * kg^-1 * s^-2)
        /// </summary>
        public const double GravConst = 6.6743e-11F;

        /// <summary>
        /// Mass of Earth (kg)
        /// </summary>
        public const double MassOfEarth = 5.9721e24F;

        /// <summary>
        /// Standard Gravitational Parameter (km^3 * s^-2)
        /// </summary>
        // public static double mu => GravConst * 10e-12 * MassOfEarth;
        public const double StdGravParam = 398600;

        /// <summary>
        /// (km)
        /// </summary>
        public const float EarthRadius = 6757;

        public static float EarthScale = (float)(0.5 / EarthRadius);

        // public static float EarthScale =>
        //     MonoBehaviour.FindObjectOfType<PlanetLogic>().SphereRadius / EarthRadius;

        public const double AU = 1.495978707e11;

        public static Vector3 Vector3(double3 vector) =>
            new Vector3((float)vector.x, (float)vector.y, (float)vector.z);

        public static double SquareMagnitude(double3 vector) =>
            vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;

        public struct FixedDouble3
        {
            public double x;
            public double y;
            public double z;

            public static implicit operator FixedDouble3(double3 vector) =>
                new FixedDouble3
                {
                    x = vector.x,
                    y = vector.y,
                    z = vector.z
                };

            public static implicit operator FixedDouble3(Vector3 vector) =>
                new FixedDouble3
                {
                    x = vector.x,
                    y = vector.y,
                    z = vector.z
                };

            public static implicit operator FixedDouble3(Point3d vector) =>
                new FixedDouble3
                {
                    x = vector.x,
                    y = vector.y,
                    z = vector.z
                };

            public static implicit operator Vector3(FixedDouble3 vector) =>
                new Vector3((float)vector.x, (float)vector.y, (float)vector.z);

            public static implicit operator double3(FixedDouble3 vector) =>
                new double3(vector.x, vector.y, vector.z);
        }

        public struct FixedVector3
        {
            public float x;
            public float y;
            public float z;

            public static implicit operator FixedVector3(Vector3 vector) =>
                new FixedVector3
                {
                    x = vector.x,
                    y = vector.y,
                    z = vector.z
                };

            public static implicit operator FixedVector3(double3 vector) =>
                new FixedVector3
                {
                    x = (float)vector.x,
                    y = (float)vector.y,
                    z = (float)vector.z
                };

            public static implicit operator FixedVector3(Point3d vector) =>
                new FixedVector3
                {
                    x = (float)vector.x,
                    y = (float)vector.y,
                    z = (float)vector.z
                };

            public static implicit operator Vector3(FixedVector3 vector) =>
                new Vector3(vector.x, vector.y, vector.z);

            public static implicit operator double3(FixedVector3 vector) =>
                new double3(vector.x, vector.y, vector.z);
        }

        public static Vector3 FixInclination(Point3d p3d) =>
            Quaternion.AngleAxis(90, UnityEngine.Vector3.right) * (FixedVector3)p3d;
    }
}
