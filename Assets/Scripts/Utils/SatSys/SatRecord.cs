using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Newtonsoft.Json;
using static SatSys.SatData;
using static SatSys.SatUtils;

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
            public Vector3 position;
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
                result.Add(new RecordGroup { elapsedTime = t, position = Vector3(data.position), });
            }
            return result;
        }

        public class SatTask
        {
            public List<StationRecord> stations;
            public List<SatelliteRecord> satellites;

            public SatTask()
            {
                stations = new List<StationRecord>
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
                satellites = new List<SatelliteRecord>
                {
                    new SatelliteRecord
                    {
                        name = "Satellite 1",
                        records = GenerateSatelliteRecord(new SatelliteData()),
                    },
                };
            }
        }
    }
}
