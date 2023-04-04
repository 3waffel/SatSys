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

            public double3 position { get; private set; }
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

            public void UpdateSatelliteState(double epoch)
            {
                (position, velocity) = KeplerianToCartesian(elements, epoch);
            }

            public SimpleKeplerOrbits.KeplerOrbitData keplerOrbitData;

            public void CalculateStateExternal()
            {
                keplerOrbitData = new SimpleKeplerOrbits.KeplerOrbitData(
                    eccentricity: elements.Eccentricity,
                    semiMajorAxis: elements.SemiMajorAxis,
                    meanAnomalyDeg: elements.MeanAnomaly,
                    inclinationDeg: elements.Inclination,
                    argOfPerifocusDeg: elements.Periapsis,
                    ascendingNodeDeg: elements.AscendingNode,
                    attractorMass: attractorMass,
                    gConst: gravConst
                );

                double3 double3(SimpleKeplerOrbits.Vector3d vector3) =>
                    new double3(vector3.x, vector3.y, vector3.z);
                velocity = double3(keplerOrbitData.Velocity);
                position = double3(keplerOrbitData.Position);
            }

            public void UpdateStateExternal(double deltaTime)
            {
                keplerOrbitData.UpdateOrbitDataByTime(deltaTime);

                double3 double3(SimpleKeplerOrbits.Vector3d vector3) =>
                    new double3(vector3.x, vector3.y, vector3.z);
                velocity = double3(keplerOrbitData.Velocity);
                position = double3(keplerOrbitData.Position);
            }
        }
    }
}
