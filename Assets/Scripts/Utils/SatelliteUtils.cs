using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteUtils : MonoBehaviour
{
    /// <summary>
    /// Newtons Gravitational Constant
    /// </summary>
    public const double G = 6.6743e-11F;

    /// <summary>
    /// Mass of Earth
    /// </summary>
    public const double M = 5.972e24F;

    /// <summary>
    /// Standard Gravitational Parameter
    /// </summary>
    public static double mu => G * M;

    public static double GetJulianDate(int year, int month, int day, int hour, int minute, int second)
    {
        DateTime date = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        double julianDate = date.ToOADate() + 2415018.5;
        return julianDate;
    }

    public static double GetJulianDate(DateTime date)
    {
        double julianDate = date.ToOADate() + 2415018.5;
        return julianDate;
    }

    /// <summary>
    /// Keplerian Elements of An Orbit
    /// </summary>
    public record KeplerianElements
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

    public static double GetTrueAnomaly(KeplerianElements elements, double julianDate)
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

    /// <summary>
    /// Convert Kaplerian Orbit elements to Cartesian State Vector
    /// Convert Elements into position and velocity vectors at the given epoch
    /// </summary>
    /// <param name="elements"></param>
    /// <returns></returns>
    public static (Vector3, Vector3) KeplerianToCartesian(KeplerianElements elements, double julianDate)
    {
        double a = elements.SemiMajorAxis;
        double e = elements.Eccentricity;
        double i = elements.Inclination;
        double o = elements.AscendingNode;
        double w = elements.Periapsis;
        double M = elements.MeanAnomaly;

        double v = GetTrueAnomaly(elements, julianDate);

        double r = a * (1 - e * e) / (1 + e * Math.Cos(v));
        double x = r * (Math.Cos(o) * Math.Cos(v + w) - Math.Sin(o) * Math.Sin(v + w) * Math.Cos(i));
        double y = r * (Math.Sin(o) * Math.Cos(v + w) + Math.Cos(o) * Math.Sin(v + w) * Math.Cos(i));
        double z = r * (Math.Sin(v + w) * Math.Sin(i));

        double p = Math.Sqrt(mu / a) / (1 + e * Math.Cos(v));
        double vx = p * (-Math.Cos(o) * Math.Sin(v + w) - Math.Sin(o) * Math.Cos(v + w) * Math.Cos(i));
        double vy = p * (-Math.Sin(o) * Math.Sin(v + w) + Math.Cos(o) * Math.Cos(v + w) * Math.Cos(i));
        double vz = p * (Math.Sin(i) * Math.Sin(v + w));

        return (new Vector3((float)x, (float)y, (float)z), new Vector3((float)vx, (float)vy, (float)vz));
    }

    public record SatelliteState
    {
        private readonly double _julianDate;

        /// <value>位置矢量</value>
        public Vector3 Position { get; set; }

        /// <value>速度矢量</value>
        public Vector3 Velocity { get; set; }
    }

    public class Satellite
    {
        private KeplerianElements _elements;

        public Satellite(KeplerianElements elements)
        {
            this._elements = elements;
        }

        public Satellite()
        {
            this._elements = new KeplerianElements
            {
                SemiMajorAxis = 1.3844, // 384400
                Eccentricity = 0.0554,
                Inclination = 5.16,
                Periapsis = 318.15,
                AscendingNode = 125.08,
                MeanAnomaly = 135.27,
            };
        }

        /// <summary>
        /// 根据轨道六根数计算在某一时刻的卫星数据
        /// </summary>
        /// <param name="julianDate">指定时刻</param>
        /// <returns></returns>
        public SatelliteState GetSatelliteState(double julianDate)
        {
            var (position, velocity) = KeplerianToCartesian(_elements, julianDate);
            return new SatelliteState { Position = position, Velocity = velocity };
        }
    }
}
