using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteUtils : MonoBehaviour
{
    /// <summary>
    /// 轨道六根数
    /// </summary>
    public record OrbitalElements
    {
        /// <value>半长轴 Semi-Major Axis</value>
        public double a;

        /// <value>离心率 Eccentricity</value>
        public double e;

        /// <value>轨道倾角 Inclination</value>
        public double i;

        /// <value>近心点辐角 Arguments of Periapsis</value>
        public double argp;

        /// <value>升交点经度 Longitude of Ascending Node</value>
        public double Omega;

        /// <value>真近点角 True Anomaly</value>
        public double nu;
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

    /// <summary>
    /// 创建卫星
    /// </summary>
    /// <param name="elements">轨道六根数</param>
    /// <returns></returns>
    public static Satellite CreateSatellite(OrbitalElements elements)
    {
        return new Satellite(elements);
    }
}
