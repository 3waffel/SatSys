using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static SatSys.SatElements;
using static SatSys.SatUtils;

namespace SatSys
{
    public static class SatData
    {
        [Serializable]
        public class SatelliteData
        {
            public double attractorMass = MassOfEarth; // (m^3 * kg^-1 * s^-2)
            public double gravConst = GravConst; // (kg)
            public double mu => attractorMass * 10e-12 * GravConst; // (km^3 / s^2)

            public KeplerianElements elements;
            public double currentMeanAnomaly;

            /// <summary>
            /// (degrees per second)
            /// </summary>
            /// <returns></returns>
            public double meanMotionPerSecond =>
                Math.Sqrt(mu / Math.Pow(elements.SemiMajorAxis, 3)) * (180 / Math.PI);

            // TODO fix mean motion
            public double orbitMaxMeanAnomaly = 10;
            public double orbitTimeStep = 0.001;
            public double orbitPeriod;

            [field: SerializeField]
            public double3 position { get; private set; }

            [field: SerializeField]
            public double3 velocity { get; private set; }

            public SatelliteData(KeplerianElements elements)
            {
                this.elements = elements;
                UpdateAnomaly(0);
            }

            public void UpdateInternalState()
            {
                (position, velocity) = Kep2Cart(elements, mu, currentMeanAnomaly);
            }

            /// <summary>
            /// update current mean anomaly based on elapsed time in seconds
            /// </summary>
            /// <param name="time"></param>
            public void UpdateAnomaly(double time)
            {
                var elapsedTimeInSeconds = SatDate.GetSeconds(time);
                currentMeanAnomaly =
                    (elements.MeanAnomaly + elapsedTimeInSeconds * meanMotionPerSecond) % 360;

                UpdateInternalState();
            }

            /// <summary>
            /// get a list of positions,
            /// used to draw orbit line
            /// </summary>
            /// <returns></returns>
            public List<Vector3> GetScaledOrbit()
            {
                var positions = new List<Vector3>();
                for (
                    double ma = 0;
                    ma <= orbitMaxMeanAnomaly;
                    ma += SatDate.GetSeconds(orbitTimeStep) * meanMotionPerSecond
                )
                {
                    var (position, _) = Kep2Cart(elements, mu, ma);
                    positions.Add(Vector3(position * Scale));
                }
                return positions;
            }
        }

        [Serializable]
        public class WalkerConstellation
        {
            public int satellitesCount;
            public int planesCount;
            public int phasing;
            public double inclination;
            public double height;

            public List<SatelliteData> satellites;
        }
    }
}
