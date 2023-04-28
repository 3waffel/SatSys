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
            public double longitude;
            public double latitude;

            public Station() { }

            public Station(StationSO so)
            {
                name = so.label;
                longitude = so.longitude;
                latitude = so.latitude;
            }

            public StationSO ToSO()
            {
                return new StationSO();
            }
        }

        public class Satellite
        {
            public string name;
            public string targetStationName;
            public string receiverStationName;
            public List<RecordGroup> records;

            public Satellite() { }

            public Satellite(
                SatelliteSO so,
                double startDate,
                double endDate,
                double timeStep = 0.01
            )
            {
                name = so.label;
                targetStationName = so.targetStationName;
                receiverStationName = so.receiverStationName;
                records = GenerateSatelliteRecord(so.satelliteData, startDate, endDate, timeStep);
            }

            public SatelliteSO ToSO()
            {
                return new SatelliteSO();
            }
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
            double startDate,
            double endDate,
            double timeStep = 0.01
        )
        {
            var timeSpan = endDate - startDate;
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
            public double startDate = SatDate.startJulianDate;
            public double endDate = SatDate.endJulianDate;
            public double timeStep = 0.01;
            public List<Station> stations;
            public List<Satellite> satellites;

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
                            startDate: startDate,
                            endDate: endDate
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
                            startDate: startDate,
                            endDate: endDate
                        ),
                    },
                };
            }

            public SatTask(
                List<Station> stations,
                List<Satellite> satellites,
                double startDate,
                double endDate,
                double timeStep = 0.01
            )
            {
                this.stations = stations;
                this.satellites = satellites;
                this.startDate = startDate;
                this.endDate = endDate;
                this.timeStep = timeStep;
            }
        }
    }
}
