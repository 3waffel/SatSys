using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteUtils : MonoBehaviour
{
    /// <summary>
    /// Newtons Gravitational Constant
    /// </summary>
    public const float G = 6.6743e-11F;

    /// <summary>
    /// Mass of Earth
    /// </summary>
    public static float M => 5.972e24F;

    /// <summary>
    /// Standard Gravitational Parameter
    /// </summary>
    public static float mu => G * M;


    /// <summary>
    /// Keplerian Orbit Elements
    /// </summary>
    public record OrbitalElements
    {
        /// <value>半长轴 Semi-Major Axis</value>
        public float a;

        /// <value>离心率 Eccentricity</value>
        public float e;

        /// <value>轨道倾角 Inclination</value>
        public float i;

        /// <value>近心点辐角 Arguments of Periapsis</value>
        public float argp;

        /// <value>升交点经度 Longitude of Ascending Node</value>
        public float Omega;

        /// <value>真近点角 True Anomaly</value>
        public float nu;
    }

    public float GetTrueAnomaly()
    {
        return 0;
    }

    /// <summary>
    /// Convert Kaplerian Orbit elements to Cartesian State Vector
    /// </summary>
    /// <param name="elements"></param>
    /// <returns></returns>
    public Vector3 KeplerianToCartesian(OrbitalElements elements)
    {
        var E = 0f;
        var rc = elements.a * (1 - elements.e * Mathf.Cos(E));
        var o = new Vector3(rc * Mathf.Cos(elements.nu), rc * Mathf.Sin(elements.nu), 0f);

        var odot = new Vector3(Mathf.Sin(E),
                               Mathf.Sqrt(1 - elements.e * elements.e) * Mathf.Cos(E),
                               0f);
        odot = (Mathf.Sqrt(mu * elements.a) / rc) * odot;

        var rx = (
            o.x * (Mathf.Cos(elements.argp)
                    * Mathf.Cos(elements.Omega)
                    - Mathf.Sin(elements.argp)
                    * Mathf.Cos(elements.i)
                    * Mathf.Sin(elements.Omega)) -
            o.y * (Mathf.Sin(elements.argp)
                    * Mathf.Cos(elements.Omega)
                    + Mathf.Cos(elements.argp)
                    * Mathf.Cos(elements.i)
                    * Mathf.Sin(elements.Omega))
        );
        var ry = (
            o.x * (Mathf.Cos(elements.argp)
                   * Mathf.Cos(elements.Omega)
                   + Mathf.Sin(elements.argp)
                   * Mathf.Cos(elements.i)
                   * Mathf.Sin(elements.Omega)) -
            o.y * (Mathf.Sin(elements.argp)
                   * Mathf.Cos(elements.i)
                   - Mathf.Cos(elements.Omega)
                   * Mathf.Cos(elements.argp)
                   * Mathf.Sin(elements.Omega))
        );
        var rz = (
            o.x * (Mathf.Sin(elements.argp) * Mathf.Sin(elements.i)) +
            o.y * (Mathf.Cos(elements.argp) * Mathf.Sin(elements.i))
        );


        var r = new Vector3(rx, ry, rz);
        return new Vector3();
    }

    public record SatelliteState
    {
        private readonly double _timestamp;

        /// <value>位置矢量</value>
        public Vector3 Position { get; set; }

        /// <value>速度矢量</value>
        public Vector3 Velocity { get; set; }
    }

    public class Satellite
    {
        private OrbitalElements _elements;

        public Satellite(OrbitalElements elements)
        {
            this._elements = elements;
        }

        /// <summary>
        /// 根据轨道六根数计算在某一时刻的卫星数据
        /// </summary>
        /// <param name="timestamp">指定时刻</param>
        /// <returns></returns>
        public SatelliteState GetSatelliteState(double timestamp)
        {
            return new SatelliteState();
        }
    }
}
