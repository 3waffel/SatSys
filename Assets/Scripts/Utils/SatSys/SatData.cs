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
            private double attractorMass = MassOfEarth; // (m^3 * kg^-1 * s^-2)
            private double gravConst = GravConst; // (kg)

            // private double muInM => attractorMass * gravConst; // (m^3 / s^2)
            // private double muInKm => muInM * 1e-9; // (km^3 / s^2)
            private double muInM = StdGravParam * 1e9;
            private double muInKm = StdGravParam;

            // different movement types
            public KeplerianElements elements;

            public Tle tle;

            public List<SatRecord.TimedPosition> orbitRecord;
            private int recordIndex = 0;

            /// <summary>
            /// (degrees per second)
            /// </summary>
            /// <returns></returns>
            public double meanMotionPerSecond =>
                Math.Sqrt(muInKm / Math.Pow(elements.SemiMajorAxis, 3)) * (180 / Math.PI); // TODO

            // TODO fix mean motion
            public double currentMeanAnomaly;

            [field: SerializeField]
            public double3 position { get; private set; }

            [field: SerializeField]
            public double3 velocity { get; private set; }

            public SatelliteData(KeplerianElements elements)
            {
                this.elements = elements;
                this.tle = Kep2Tle(elements, muInKm);
                UpdateState(0);
            }

            public SatelliteData(Tle tle)
            {
                this.tle = tle;
                this.elements = Tle2Kep(tle, muInKm);
                UpdateState(0);
            }

            public SatelliteData(SatelliteData data)
            {
                this.attractorMass = data.attractorMass;
                this.gravConst = data.gravConst;
                this.elements = data.elements;
                this.tle = data.tle ?? Kep2Tle(elements, muInKm);
                this.orbitRecord = data.orbitRecord;
            }

            private void UpdateInternalStateByKep(double timeEpoch)
            {
                var elapsedTimeInSeconds = SatDate.GetSeconds(timeEpoch);
                currentMeanAnomaly =
                    (elements.MeanAnomaly + elapsedTimeInSeconds * meanMotionPerSecond) % 360;
                (position, velocity) = Kep2Cart(elements, muInKm, currentMeanAnomaly);
            }

            private void UpdateInternalStateByTle(double timeEpoch)
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

                // position = new double3(pos.x, pos.y, pos.z);
                // velocity = new double3(vel.x, vel.y, vel.z);

                // TODO fix inclination
                position = (FixedDouble3)FixInclination(pos);
                velocity = (FixedDouble3)FixInclination(vel);
            }

            public void UpdateInternalStateByRecord(double timeEpoch)
            {
                if (orbitRecord == null || orbitRecord.Count == 0)
                    return;

                float timeStep = (float)(orbitRecord[1].elapsedTime - orbitRecord[0].elapsedTime);
                int idx = (int)(timeEpoch / timeStep);
                idx = orbitRecord.FindIndex(idx, (item) => item.elapsedTime >= timeEpoch);
                recordIndex = idx < 0 ? orbitRecord.Count - 1 : idx;
                position = orbitRecord[recordIndex].position;
            }

            /// <summary>
            /// update current mean anomaly based on elapsed time in seconds
            /// </summary>
            /// <param name="time"></param>
            public void UpdateState(double timeEpoch)
            {
                if (tle != null)
                    UpdateInternalStateByTle(timeEpoch);
                else
                    UpdateInternalStateByKep(timeEpoch);
            }

            public List<Vector3> GetOrbit()
            {
                var positions = new List<Vector3>();
                if (tle != null)
                {
                    // period in julian date
                    var timeStep = 0.01 / tle.getMeanMotion();
                    var period = 1 / tle.getMeanMotion();

                    for (
                        double t = Timeline.startDate;
                        t < Timeline.startDate + period;
                        t += timeStep
                    )
                    {
                        var data = SatFunctions.getSatPositionAtTime(
                            tle,
                            new EpochTime(SatDate.GetDateTime(t)),
                            Sgp4.wgsConstant.WGS_84
                        );
                        var pos = data.getPositionData();
                        // var pos_d3 = new double3(pos.x, pos.y, pos.z);
                        // positions.Add(Vector3(pos_d3));

                        // TODO fix inclination
                        positions.Add(FixInclination(pos));
                    }
                    positions.Add(positions[0]);
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

                    positions.Add(positions[0]);
                }
                return positions;
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
