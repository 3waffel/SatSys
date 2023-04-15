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

            // degrees per second
            public double meanMotion =>
                Math.Sqrt(mu / Math.Pow(elements.SemiMajorAxis, 3)) * (180 / Math.PI);

            public double orbitTimeStep = 0.001;
            public double orbitMaxMeanAnomaly = 10;

            [field: SerializeField]
            public double3 position { get; private set; }

            [field: SerializeField]
            public double3 velocity { get; private set; }

            public SatelliteData(KeplerianElements elements)
            {
                this.elements = elements;
            }

            public SatelliteData()
            {
                this.elements = new KeplerianElements
                {
                    // Parameters of GSAT0201
                    SemiMajorAxis = 27977.6, // (km)
                    Eccentricity = 0.162,
                    Inclination = 49.850,
                    Periapsis = 56.198,
                    AscendingNode = 52.521,
                    MeanAnomaly = 316.069,
                };
            }

            public void UpdateInternalState()
            {
                (position, velocity) = Kep2Cart(elements, mu, currentMeanAnomaly);
            }

            // Update current mean anomaly based on elapsed time in seconds
            public void UpdateAnomaly(double time)
            {
                var elapsedTimeInSeconds = SatDate.GetSeconds(time);
                currentMeanAnomaly =
                    (elements.MeanAnomaly + elapsedTimeInSeconds * meanMotion) % 360;

                UpdateInternalState();
            }

            public List<Vector3> GetScaledOrbit()
            {
                var positions = new List<Vector3>();
                for (
                    double ma = 0;
                    ma <= orbitMaxMeanAnomaly;
                    ma += SatDate.GetSeconds(orbitTimeStep) * meanMotion
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

            public WalkerConstellation() { }
        }
    }
}
