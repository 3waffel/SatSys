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
        public struct StationRecord
        {
            public string name;
            public double longitude;
            public double latitude;
        }

        public struct SatelliteRecord
        {
            public string name;
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
            public List<StationRecord> stationRecords;
            public List<SatelliteRecord> satelliteRecords;
            public double timeSpan = 1;

            public SatTask()
            {
                stationRecords = new List<StationRecord>
                {
                    new StationRecord
                    {
                        name = "Beijing",
                        longitude = 116.46,
                        latitude = 39.92,
                    },
                    new StationRecord
                    {
                        name = "Antarctic",
                        longitude = -62.21,
                        latitude = -58.96,
                    },
                };
                satelliteRecords = new List<SatelliteRecord>
                {
                    new SatelliteRecord
                    {
                        name = "GSAT0201",
                        records = GenerateSatelliteRecord(
                            new SatelliteData(
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
                            timeSpan
                        ),
                    },
                    new SatelliteRecord
                    {
                        name = "GSAT0202",
                        records = GenerateSatelliteRecord(
                            new SatelliteData(
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
                            timeSpan
                        ),
                    },
                };
            }

            public SatTask(
                List<StationRecord> stationRecords,
                List<SatelliteRecord> satelliteRecords,
                double timeSpan = 1
            )
            {
                this.stationRecords = stationRecords;
                this.satelliteRecords = satelliteRecords;
                this.timeSpan = timeSpan;
            }
        }
    }
}
