using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Newtonsoft.Json;
using static SatSys.SatData;
using static SatSys.SatUtils;
using static SatSys.SatElements;

namespace SatSys
{
    public static class SatRecord
    {
        public class Station
        {
            public string name;
            public float longitude;
            public float latitude;

            public Station() { }

            public Station(StationSO so)
            {
                name = so.name;
                longitude = so.longitude;
                latitude = so.latitude;
            }

            public StationSO ToStationSO()
            {
                var so = ScriptableObject.CreateInstance<StationSO>();
                so.name = name;
                so.longitude = longitude;
                so.latitude = latitude;
                return so;
            }
        }

        public class Satellite
        {
            public string name;
            public string targetStationName;
            public string receiverStationName;
            public List<TimedPosition> records;

            public Satellite() { }

            public Satellite(
                SatelliteSO so,
                double startDate,
                double endDate,
                double timeStep = 0.01
            )
            {
                name = so.name;
                targetStationName = so.targetStationName;
                receiverStationName = so.receiverStationName;
                records = GenerateSatelliteRecord(so.satelliteData, startDate, endDate, timeStep);
            }
        }

        public struct TimedPosition
        {
            public double elapsedTime;
            public FixedVector3 position;
        }

        /// <summary>
        /// Generate orbit info in a given time span
        /// </summary>
        /// <param name="data"></param>
        public static List<TimedPosition> GenerateSatelliteRecord(
            SatelliteData data,
            double startDate,
            double endDate,
            double timeStep = 0.01
        )
        {
            var timeSpan = endDate - startDate;
            var result = new List<TimedPosition>();
            for (double t = 0; t < timeSpan; t += timeStep)
            {
                data.UpdateState(t);
                result.Add(new TimedPosition { elapsedTime = t, position = data.position, });
            }
            return result;
        }

        public class SatTask
        {
            public double startDate = SatDate.startJulianDate;
            public double endDate = SatDate.endJulianDate;
            public float timeStep = 0.01f;
            public List<Station> stations;
            public List<Satellite> satellites;

            public SatTask() { }

            public SatTask(
                List<Station> stations,
                List<Satellite> satellites,
                double startDate,
                double endDate,
                float timeStep = 0.01f
            )
            {
                this.stations = stations;
                this.satellites = satellites;
                this.startDate = startDate;
                this.endDate = endDate;
                this.timeStep = timeStep;
            }

            public static SatTask GetDefaultTask()
            {
                double startDate = SatDate.startJulianDate;
                double endDate = SatDate.endJulianDate;
                float timeStep = 0.01f;
                var stations = new List<Station>
                {
                    new Station
                    {
                        name = "StationBeijing",
                        longitude = 116.46f,
                        latitude = 39.92f,
                    },
                    new Station
                    {
                        name = "StationAntarctic",
                        longitude = -62.21f,
                        latitude = -58.96f,
                    },
                };
                var satellites = new List<Satellite>
                {
                    new Satellite
                    {
                        name = "GSAT0201",
                        records = GenerateSatelliteRecord(
                            data: new SatelliteData(
                                new KeplerianElements
                                {
                                    SemiMajorAxis = 27977.6,
                                    Eccentricity = 0.162,
                                    Inclination = 49.850,
                                    Periapsis = 56.198,
                                    AscendingNode = 52.521,
                                    MeanAnomaly = 316.069,
                                }
                            ),
                            startDate: startDate,
                            endDate: endDate
                        ),
                        targetStationName = "StationAntarctic",
                        receiverStationName = "StationBeijing",
                    },
                    new Satellite
                    {
                        name = "GSAT0202",
                        records = GenerateSatelliteRecord(
                            data: new SatelliteData(
                                new KeplerianElements
                                {
                                    SemiMajorAxis = 27977.6,
                                    Eccentricity = 0.162,
                                    Inclination = 49.850,
                                    Periapsis = 56.198,
                                    AscendingNode = 52.521,
                                    MeanAnomaly = 136.069,
                                }
                            ),
                            startDate: startDate,
                            endDate: endDate
                        ),
                    },
                };
                return new SatTask(stations, satellites, startDate, endDate, timeStep);
            }
        }
    }
}
