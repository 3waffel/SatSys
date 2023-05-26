using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static SatSys.SatUtils;
using One_Sgp4;

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
            /// (km)
            /// </summary>
            public double SemiMajorAxis;

            /// <summary>
            /// A number smaller to one would be a closed ellipse, while 0 would be a circle orbit.
            /// Values higher than one would be hyperbolic orbits, that are not closed, while a 1 would be a parabolic orbit. Negative values cannot exist.
            /// </summary>
            public double Eccentricity;

            /// <summary>
            /// This value represents the inclination of the plane where the object is orbiting with respect to the reference plane.
            /// (degree)
            /// </summary>
            public double Inclination;

            /// <summary>
            /// This value represents the angle in the orbit ellipse of the nearest point of the orbit to the center of mass of the system.
            /// (degree)
            /// </summary>
            public double Periapsis;

            /// <summary>
            /// This value represents the angle in the orbit ellipse of the point where the reference plane and the orbit plane cross when the orbiting body crosses the plane ascending in the orbit.
            /// (degree)
            /// </summary>
            public double AscendingNode;

            /// <summary>
            /// This value represents the angle in the orbit ellipse of the orbiting body at the given moment.
            /// (degree)
            /// </summary>
            public double MeanAnomaly;
        }

        /// <summary>
        /// Get eccentric anomaly from mean anomaly
        /// </summary>
        /// <param name="meanAnomaly">Mean anomaly of the orbit</param>
        /// <param name="eccentricity">Eccentricity of the orbit</param>
        /// <param name="maxIterations">Maximum number of iterations to perform</param>
        /// <returns>Eccentric anomaly of the orbit</returns>
        public static double GetEccentricAnomaly(
            double meanAnomaly,
            double eccentricity,
            double epsilon = 1e-6
        )
        {
            // TODO
            double M = meanAnomaly * Math.PI / 180 * Math.PI / 180;
            double E = M;

            double delta;
            do
            {
                delta = E - eccentricity * Math.Sin(E) - M;
                E -= delta / (1 - eccentricity * Math.Cos(E));
            } while (Math.Abs(delta) > epsilon);

            return E * 180 / Math.PI;
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

        /// <summary>
        /// Get true anomaly from eccentric anomaly
        /// </summary>
        /// <param name="eccentricAnomaly">Eccentric anomaly of the orbit</param>
        /// <param name="eccentricity">Eccentricity of the orbit</param>
        /// <returns>True anomaly of the orbit</returns>
        public static double GetTrueAnomaly(double eccentricAnomaly, double eccentricity)
        {
            double E = eccentricAnomaly * Math.PI / 180;
            double trueAnomaly =
                2
                * Math.Atan(
                    Math.Sqrt((1 + eccentricity) / (1 - eccentricity))
                        * Math.Tan(eccentricAnomaly / 2)
                );
            return trueAnomaly * 180 / Math.PI;
        }

        public static double GetTrueAnomaly(KeplerianElements elements)
        {
            double a = elements.SemiMajorAxis;
            double e = elements.Eccentricity;
            double M = elements.MeanAnomaly;

            double E = GetEccentricAnomaly(M, e);
            double v = GetTrueAnomaly(E, e);
            return v;
        }

        /// <summary>
        /// Convert Elements into position and velocity vectors in the current mean anomaly
        /// </summary>
        /// <param name="elements">Keplerian elements of the orbit</param>
        /// <param name="mu">(km^3 * s^-2)</param>
        /// <param name="currentMeanAnomaly">Current mean anomaly of the orbit</param>
        /// <returns>A tuple containing position and velocity vectors</returns>
        [Obsolete]
        public static (double3, double3) KeplerianToCartesian(
            KeplerianElements elements,
            double mu,
            double currentMeanAnomaly
        )
        {
            double a = elements.SemiMajorAxis;
            double e = elements.Eccentricity;
            double i = elements.Inclination;
            double o = elements.AscendingNode;
            double w = elements.Periapsis;
            double M = currentMeanAnomaly;

            double E = GetEccentricAnomaly(M, e);
            double v = GetTrueAnomaly(E, e);
            v *= Math.PI / 180;

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

        public static double3 GetPerifocalPositionVector(
            double semiMajorAxis,
            double eccentricity,
            double trueAnomaly
        )
        {
            double v = trueAnomaly * Math.PI / 180;
            double r =
                semiMajorAxis
                * (1 - eccentricity * eccentricity)
                / (1 + eccentricity * Math.Cos(v));
            return new double3(r * Math.Cos(v), r * Math.Sin(v), 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="semiMajorAxis">(km)</param>
        /// <param name="eccentricity"></param>
        /// <param name="trueAnomaly">(degree)</param>
        /// <param name="mu">(km^3 * s^-2)</param>
        /// <returns></returns>
        public static double3 GetPerifocalVelocityVector(
            double semiMajorAxis,
            double eccentricity,
            double trueAnomaly,
            double mu
        )
        {
            double a = semiMajorAxis;
            double v = trueAnomaly * Math.PI / 180;

            double r = a * (1 - eccentricity * eccentricity) / (1 + eccentricity * Math.Cos(v));
            double h = Math.Sqrt(mu * a * (1 - eccentricity * eccentricity));
            return new double3(-mu / h * Math.Sin(v), mu / h * (eccentricity + Math.Cos(v)), 0);
        }

        public static double3 PerifocalIntoInertial(
            double3 vector,
            double inclination,
            double ascendingNode,
            double periapsis
        )
        {
            double i = inclination * Math.PI / 180; // Convert inclination from degrees to radians
            double Omega = ascendingNode * Math.PI / 180; // Convert longitude of ascending node from degrees to radians
            double omega = periapsis * Math.PI / 180; // Convert argument of periapsis from degrees to radians

            double3 result;
            result.x =
                (
                    Math.Cos(Omega) * Math.Cos(omega)
                    - Math.Sin(Omega) * Math.Sin(omega) * Math.Cos(i)
                ) * vector.x
                + (
                    -Math.Cos(Omega) * Math.Sin(omega)
                    - Math.Sin(Omega) * Math.Cos(omega) * Math.Cos(i)
                ) * vector.y;
            result.y =
                (
                    Math.Sin(Omega) * Math.Cos(omega)
                    + Math.Cos(Omega) * Math.Sin(omega) * Math.Cos(i)
                ) * vector.x
                + (
                    -Math.Sin(Omega) * Math.Sin(omega)
                    + Math.Cos(Omega) * Math.Cos(omega) * Math.Cos(i)
                ) * vector.y;
            result.z =
                (Math.Sin(omega) * Math.Sin(i)) * vector.x
                + (Math.Cos(omega) * Math.Sin(i)) * vector.y;
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="mu">(km^3 * s^-2)</param>
        /// <param name="currentMeanAnomaly"></param>
        /// <returns></returns>
        public static (double3, double3) Kep2Cart(
            KeplerianElements elements,
            double mu,
            double currentMeanAnomaly
        )
        {
            double a = elements.SemiMajorAxis;
            double e = elements.Eccentricity;
            double i = elements.Inclination;
            double o = elements.AscendingNode;
            double w = elements.Periapsis;
            double M = currentMeanAnomaly;

            double E = GetEccentricAnomaly(M, e);
            double v = GetTrueAnomaly(E, e);

            double3 position = GetPerifocalPositionVector(a, e, v);
            double3 velocity = GetPerifocalVelocityVector(a, e, v, mu);
            position = PerifocalIntoInertial(position, i, o, w);
            velocity = PerifocalIntoInertial(velocity, i, o, w);

            return (position, velocity);
        }

        // TODO
        public static Tle Kep2Tle(KeplerianElements elements)
        {
            string line1 = new String('0', 68);
            string line2 = new String('0', 68);

            int getSum(string str)
            {
                int sum = 0;
                for (int i = 0; i < str.Length - 1; i++)
                {
                    if (char.IsNumber(str[i]))
                        sum += (int)Char.GetNumericValue(str[i]);
                    else
                    {
                        if (str[i] == '-')
                            sum++;
                    }
                }
                return sum;
            }
            line1 += (getSum(line1) % 10);
            line2 += (getSum(line2) % 10);
            return ParserTLE.parseTle(line1, line2);
        }

        public static KeplerianElements Tle2Kep(Tle tle, double mu)
        {
            return new KeplerianElements
            {
                SemiMajorAxis =
                    Math.Pow(mu, 1 / 3)
                    / (2 * tle.getMeanMotion() * Math.Pow(Math.PI, 1 / 3) / 86400),
                Eccentricity = tle.getEccentriciy(),
                Inclination = tle.getInclination(),
                Periapsis = tle.getPerigee(),
                AscendingNode = tle.getRightAscendingNode(),
                MeanAnomaly = tle.getMeanAnomoly(),
            };
        }
    }
}
