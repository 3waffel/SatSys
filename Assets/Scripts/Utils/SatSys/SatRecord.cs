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
        public struct Station
        {
            public string name;
            public double longitude;
            public double latitude;
        }

        public struct Satellite
        {
            public string name;
            public string targetStationName;
            public string receiverStationName;
            public List<RecordGroup> records;
        }

        public struct RecordGroup
        {
            public double elapsedTime;
            public FixedVector3 position;
        }

        /// <summary>
        /// Generate orbit info in a given time span
        /// </summary>
        /// <param name="data"></param>
        public static List<RecordGroup> GenerateSatelliteRecord(
            SatelliteData data,
            double timeSpan = 1,
            double timeStep = 0.01
        )
        {
            var result = new List<RecordGroup>();
            for (double t = 0; t < timeSpan; t += timeStep)
            {
                data.UpdateAnomaly(t);
                result.Add(
                    new RecordGroup { elapsedTime = t, position = new FixedVector3(data.position), }
                );
            }
            return result;
        }

        public class SatTask
        {
            public List<Station> stations;
            public List<Satellite> satellites;
            public double timeSpan = 1;

            public SatTask()
            {
                stations = new List<Station>
                {
                    new Station
                    {
                        name = "Station Beijing",
                        longitude = 116.46,
                        latitude = 39.92,
                    },
                    new Station
                    {
                        name = "Station Antarctic",
                        longitude = -62.21,
                        latitude = -58.96,
                    },
                };
                satellites = new List<Satellite>
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
                            timeSpan: timeSpan
                        ),
                        targetStationName = "Station Antarctic",
                        receiverStationName = "Station Beijing",
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
                            timeSpan: timeSpan
                        ),
                    },
                };
            }

            public SatTask(List<Station> stations, List<Satellite> satellites, double timeSpan = 1)
            {
                this.stations = stations;
                this.satellites = satellites;
                this.timeSpan = timeSpan;
            }
        }
    }
}
