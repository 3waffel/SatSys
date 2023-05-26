using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static SatSys.SatElements;
using static SatSys.SatUtils;
using One_Sgp4;

namespace SatSys
{
    public static class SatData
    {
        [Serializable]
        public class SatelliteData
        {
            public double attractorMass = MassOfEarth; // (m^3 * kg^-1 * s^-2)
            public double gravConst = GravConst; // (kg)
            public double mu => attractorMass * gravConst;
            public double muInKm => mu * 10e-12; // (km^3 / s^2)

            public KeplerianElements elements;
            public double currentMeanAnomaly;

            public Tle tle;

            /// <summary>
            /// (degrees per second)
            /// </summary>
            /// <returns></returns>
            public double meanMotionPerSecond =>
                Math.Sqrt(muInKm / Math.Pow(elements.SemiMajorAxis, 3))
                * (180 / Math.PI)
                * (180 / Math.PI); // TODO

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

            public SatelliteData(Tle tle)
            {
                this.tle = tle;
                this.elements = Tle2Kep(tle, mu);
                UpdateInternalStateByTle(0);
            }

            void UpdateInternalState()
            {
                (position, velocity) = Kep2Cart(elements, muInKm, currentMeanAnomaly);
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

                if (tle != null)
                    UpdateInternalStateByTle(time);
                else
                    UpdateInternalState();
            }

            public void UpdateInternalStateByTle(double timeEpoch)
            {
                if (tle == null)
                    return;

                var data = SatFunctions.getSatPositionAtTime(
                    tle,
                    new EpochTime(SatDate.GetDateTime(Timeline.startDate + timeEpoch)),
                    Sgp4.wgsConstant.WGS_84
                );
                var pos = data.getPositionData();
                var vel = data.getVelocityData();
                position = new double3(pos.x, pos.y, pos.z);
                velocity = new double3(vel.x, vel.y, vel.z);
            }

            /// <summary>
            /// get a list of positions,
            /// used to draw orbit line
            /// </summary>
            /// <returns></returns>
            public List<Vector3> GetScaledOrbit()
            {
                var positions = GetOrbit();
                for (int i = 0; i < positions.Count; i++)
                {
                    positions[i] *= EarthScale;
                }
                return positions;
            }

            public List<Vector3> GetOrbit()
            {
                var positions = new List<Vector3>();
                if (tle != null)
                {
                    for (double t = Timeline.startDate; t < Timeline.endDate; t += 0.01)
                    {
                        var data = SatFunctions.getSatPositionAtTime(
                            tle,
                            new EpochTime(SatDate.GetDateTime(t)),
                            Sgp4.wgsConstant.WGS_84
                        );
                        var pos = data.getPositionData();
                        var pos_d3 = new double3(pos.x, pos.y, pos.z);
                        positions.Add(Vector3(pos_d3));
                    }
                }
                else
                {
                    double halfMA = 0d;
                    void semiCircle(ref double ma)
                    {
                        double originMA = ma;
                        float maxAngle = 0;
                        for (; ma < 360; ma += meanMotionPerSecond * 10)
                        {
                            var (pos1, _) = Kep2Cart(elements, muInKm, originMA);
                            var (pos2, _) = Kep2Cart(elements, muInKm, ma);
                            var a = UnityEngine.Vector3.Angle(Vector3(pos1), Vector3(pos2));
                            if (a < maxAngle)
                                break;
                            maxAngle = a;
                            positions.Add(Vector3(pos2));
                        }
                    }
                    semiCircle(ref halfMA);
                    semiCircle(ref halfMA);

                    var (finalPos, _) = Kep2Cart(elements, muInKm, 0);
                    positions.Add(Vector3(finalPos));
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
