using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static SatSys.SatUtils;

namespace SatSys
{
    public static class SatElements
    {
        /// <summary>
        /// Keplerian Elements of An Orbit
        /// </summary>
        [Serializable]
        public struct KeplerianElements
        {
            /// <summary>
            /// This value represents the average distance from the orbiting body to the center of mass.
            /// </summary>
            public double SemiMajorAxis;

            /// <summary>
            /// A number smaller to one would be a closed ellipse, while 0 would be a circle orbit.
            /// Values higher than one would be hyperbolic orbits, that are not closed, while a 1 would be a parabolic orbit. Negative values cannot exist.
            /// </summary>
            public double Eccentricity;

            /// <summary>
            /// This value represents the inclination of the plane where the object is orbiting with respect to the reference plane.
            /// </summary>
            public double Inclination;

            /// <summary>
            /// This value represents the angle in the orbit ellipse of the nearest point of the orbit to the center of mass of the system.
            /// </summary>
            public double Periapsis;

            /// <summary>
            /// This value represents the angle in the orbit ellipse of the point where the reference plane and the orbit plane cross when the orbiting body crosses the plane ascending in the orbit.
            /// </summary>
            public double AscendingNode;

            /// <summary>
            /// This value represents the angle in the orbit ellipse of the orbiting body at the given moment.
            /// </summary>
            public double MeanAnomaly;
        }

        public static double GetEccentricAnomaly(
            KeplerianElements elements,
            int maxIterations = 100
        )
        {
            double e = elements.Eccentricity;
            double M = elements.MeanAnomaly;

            return GetEccentricAnomaly(M, e, maxIterations);
        }

        public static double GetEccentricAnomaly(
            double meanAnomaly,
            double eccentricity,
            int maxIterations = 100
        )
        {
            double e = eccentricity;
            double M = meanAnomaly;

            double KeplerEquation(double E, double M, double e) => M - E + e * Math.Sin(E);

            const double h = 1e-4;
            const double error = 1e-8;
            double guess = meanAnomaly;

            for (int i = 0; i < maxIterations; i++)
            {
                double y = KeplerEquation(guess, M, e);
                if (Math.Abs(y) < error)
                    break;
                double slope = (KeplerEquation(guess + h, M, e) - y) / h;
                double step = y / slope;
                guess -= step;
            }
            return guess;
        }

        public static double GetTrueAnomaly(KeplerianElements elements)
        {
            double a = elements.SemiMajorAxis;
            double e = elements.Eccentricity;
            double M = elements.MeanAnomaly;

            double E = M + e * Math.Sin(M) * (1.0 + e * Math.Cos(M));
            double E0 = E;
            double E1 = E0 - (E0 - e * Math.Sin(E0) - M) / (1 - e * Math.Cos(E0));
            while (Math.Abs(E1 - E0) > 1e-8)
            {
                E0 = E1;
                E1 = E0 - (E0 - e * Math.Sin(E0) - M) / (1 - e * Math.Cos(E0));
            }

            double xv = a * (Math.Cos(E) - e);
            double yv = a * Math.Sqrt(1 - e * e) * Math.Sin(E);
            double v = Math.Atan2(yv, xv);
            return v;
        }

        public static double2 CalculatePointOnOrbit(double periapsis, double apoapsis, double t)
        {
            double semiMajorLength = (apoapsis + periapsis) / 2;
            double linearEccentricity = semiMajorLength - periapsis;
            double eccentricity = linearEccentricity / semiMajorLength;
            double semiMinorLength = Math.Sqrt(
                Math.Pow(semiMajorLength, 2) - Math.Pow(linearEccentricity, 2)
            );

            double meanAnomaly = t * Math.PI * 2;
            double eccentricAnomaly = GetEccentricAnomaly(meanAnomaly, eccentricity);

            double ellipseCenterX = -linearEccentricity;
            double pointX = Math.Cos(eccentricAnomaly) * semiMajorLength * ellipseCenterX;
            double pointY = Math.Sin(eccentricAnomaly) * semiMinorLength;
            return new double2(pointX, pointY);
        }

        /// <summary>
        /// Convert Kaplerian Orbit elements to Cartesian State Vector
        /// Convert Elements into position and velocity vectors at the given epoch
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static (double3, double3) KeplerianToCartesian(
            KeplerianElements elements,
            double julianDate
        )
        {
            double a = elements.SemiMajorAxis;
            double e = elements.Eccentricity;
            double i = elements.Inclination;
            double o = elements.AscendingNode;
            double w = elements.Periapsis;
            double M = elements.MeanAnomaly;

            double v = GetTrueAnomaly(elements);

            double r = a * (1 - e * e) / (1 + e * Math.Cos(v));
            double x =
                r * (Math.Cos(o) * Math.Cos(v + w) - Math.Sin(o) * Math.Sin(v + w) * Math.Cos(i));
            double y =
                r * (Math.Sin(o) * Math.Cos(v + w) + Math.Cos(o) * Math.Sin(v + w) * Math.Cos(i));
            double z = r * (Math.Sin(v + w) * Math.Sin(i));

            double p = Math.Sqrt(mu / a) / (1 + e * Math.Cos(v));
            double vx =
                p * (-Math.Cos(o) * Math.Sin(v + w) - Math.Sin(o) * Math.Cos(v + w) * Math.Cos(i));
            double vy =
                p * (-Math.Sin(o) * Math.Sin(v + w) + Math.Cos(o) * Math.Cos(v + w) * Math.Cos(i));
            double vz = p * (Math.Sin(i) * Math.Sin(v + w));

            return (new double3(x, y, z), new double3(vx, vy, vz));
        }
    }
}
