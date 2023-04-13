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
            public double attractorMass = M;
            public double gravConst = G;

            public KeplerianElements elements;
            public double currentMeanAnomaly;
            public double meanMotion =>
                Math.Sqrt((attractorMass * gravConst) / Math.Pow(elements.SemiMajorAxis, 3));

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
                    // Parameters of Moon
                    SemiMajorAxis = 0.0025,
                    Eccentricity = 0.0450,
                    Inclination = 5.2522,
                    Periapsis = 130.6531,
                    AscendingNode = 88.9708,
                    MeanAnomaly = 328.5176,
                };
            }

            public void UpdateInternalState()
            {
                (position, velocity) = KeplerianToCartesian(elements, currentMeanAnomaly);
            }

            public void UpdateAnomaly(double time)
            {
                var elapsedTime = SatDate.GetSeconds(time);
                currentMeanAnomaly = (elements.MeanAnomaly + elapsedTime * meanMotion) % 360;

                UpdateInternalState();
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

            public WalkerConstellation() {
                
            }
        }
    }
}
