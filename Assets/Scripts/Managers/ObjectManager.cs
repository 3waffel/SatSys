using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;
using SatSys;
using static SatSys.SatData;
using static SatSys.SatRecord;

[DisallowMultipleComponent]
public class ObjectManager : MonoBehaviour
{
    // Object Browser
    public Transform contentContainer;
    public Transform browserItemPrefab;

    // ObjectSO Collection
    public List<ObjectSO> objectCollection;

    // will be assigned to newly created objects
    public Transform targetScene;
    public Transform targetPlanet;

    public Transform defaultSatellitePrefab;
    public Transform defaultStationPrefab;

    public static ObjectManager Instance;

    // random spawn index
    static int sttIndex = 0;
    static int satIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        EventManager.ObjectCreated += CreateObjectFromSO;
        EventManager.ObjectDeleted += DeferredObjectUpdate;

        SpawnFromCollection();
    }

    void SpawnFromCollection()
    {
        if (objectCollection.Count > 0)
        {
            foreach (var so in objectCollection)
            {
                if (so == null)
                    continue;

                Guid guid = Guid.NewGuid();
                CreateBrowserObject(guid, so.name);
                CreateInspectorObject(guid, so);
            }
        }
        EventManager.OnObjectUpdated();
    }

    void BrowserSpawnTest(int itemsToGenerate = 5)
    {
        for (int i = 0; i < itemsToGenerate; i++)
        {
            var item = Instantiate(browserItemPrefab);
            var label = "Test Item " + i;

            item.GetComponentInChildren<TMP_Text>().text = label;
            item.transform.SetParent(contentContainer);
            item.transform.localScale = Vector2.one;
        }
    }

    public void RandomSpawnTest(int satCount = 10, int sttCount = 3)
    {
        var rnd = new System.Random();
        var sttNameList = new List<string>();

        for (int i = 0; i < sttCount; i++)
        {
            var so = ScriptableObject.CreateInstance<StationSO>();
            so.name = "TestStt" + sttIndex++;
            so.longitude = rnd.Next(-180, 180);
            so.latitude = rnd.Next(-90, 90);
            CreateObjectFromSO(so);
            sttNameList.Add(so.name);
        }

        for (int i = 0; i < satCount; i++)
        {
            var data = new SatelliteData(
                new SatElements.KeplerianElements
                {
                    SemiMajorAxis = SatUtils.EarthRadius + rnd.Next(500, 20000),
                    Eccentricity = rnd.NextDouble() % 0.1,
                    Inclination = rnd.Next(360),
                    Periapsis = rnd.Next(360),
                    AscendingNode = rnd.Next(360),
                    MeanAnomaly = rnd.Next(360),
                }
            );
            var so = ScriptableObject.CreateInstance<SatelliteSO>();
            so.name = "TestSat" + satIndex++;
            so.satelliteData = data;
            so.targetStationName = sttNameList[rnd.Next(sttCount)];
            so.receiverStationName = sttNameList[rnd.Next(sttCount)];
            CreateObjectFromSO(so);
        }
        EventManager.OnObjectUpdated();
    }

    void CreateBrowserObject(Guid guid, string label)
    {
        var item = Instantiate(browserItemPrefab);

        item.GetComponentInChildren<TMP_Text>().text = label;
        item.GetComponentInChildren<ObjectBrowserItem>().Guid = guid;
        item.transform.SetParent(contentContainer);
        item.transform.localScale = Vector2.one;
    }

    void CreateInspectorObject(Guid guid, ObjectSO obj)
    {
        obj.targetScene = targetScene;
        obj.targetPlanet = targetPlanet;
        if (obj.itemPrefab == null)
        {
            obj.itemPrefab = obj is SatelliteSO ? defaultSatellitePrefab : defaultStationPrefab;
        }
        obj.Spawn(guid);
    }

    void CreateObjectFromSO(ObjectSO so)
    {
        Guid guid = Guid.NewGuid();
        CreateBrowserObject(guid, so.name);
        CreateInspectorObject(guid, so);
        objectCollection.Add(so);
        EventManager.OnObjectUpdated();
    }

    // using task to avoid finding destroyed objects
    // TODO update collection when deleted
    async void DeferredObjectUpdate()
    {
        await Task.Run(() => new WaitForSeconds(0.1f))
            .ContinueWith(
                (_) =>
                {
                    EventManager.OnObjectUpdated();
                    EventManager.SelectedGuid = default;
                },
                TaskScheduler.FromCurrentSynchronizationContext()
            );
    }

    void CreateSatelliteFromData(SatelliteData data, string label)
    {
        Guid guid = Guid.NewGuid();
        CreateBrowserObject(guid, label);

        var sat = Instantiate(defaultSatellitePrefab);
        sat.name = label;
        sat.transform.SetParent(targetScene);
        var logic = sat.GetComponent<SatelliteLogic>();
        logic.Guid = guid;
        logic.targetPlanet = targetPlanet;
        logic.satelliteData = data;
        logic.InitializeDirectMovement();
    }

    void CreateSatelliteFromRecord(Satellite satellite)
    {
        Guid guid = Guid.NewGuid();
        CreateBrowserObject(guid, satellite.name);

        var sat = Instantiate(defaultSatellitePrefab);
        sat.name = satellite.name;
        sat.transform.SetParent(targetScene);
        var logic = sat.GetComponent<SatelliteLogic>();
        logic.Guid = guid;
        logic.targetPlanet = targetPlanet;
        logic.targetStationName = satellite.targetStationName;
        logic.receiverStationName = satellite.receiverStationName;
        logic.orbitRecord = satellite.records;
        logic.InitializeRecordMovement();
    }

    public void ClearScene()
    {
        objectCollection.Clear();
        var objects = FindObjectsOfType<ObjectLogic>();
        foreach (var obj in objects)
        {
            obj.gameObject.SetActive(false);
            GameObject.Destroy(obj.gameObject);
        }

        foreach (Transform item in contentContainer)
        {
            GameObject.Destroy(item.gameObject);
        }
        EventManager.OnObjectUpdated();
    }

#nullable enable
    public SatTask? SaveCollection()
    {
        if (objectCollection.Count == 0)
            return null;

        var stations = new List<Station>();
        var satellites = new List<Satellite>();
        foreach (var so in objectCollection)
        {
            switch (so)
            {
                case (StationSO stt):
                    stations.Add(new Station(stt));
                    break;
                case (SatelliteSO sat):
                    satellites.Add(
                        new Satellite(sat, Timeline.startDate, Timeline.endDate, Timeline.timeStep)
                    );
                    break;
            }
        }

        SatTask task = new SatTask(
            stations,
            satellites,
            Timeline.startDate,
            Timeline.endDate,
            Timeline.timeStep
        );
        return task;
    }

    public SatTask? SaveScene()
    {
        var logics = FindObjectsOfType<ObjectLogic>();
        if (logics.Length == 0)
            return null;

        var stations = new List<Station>();
        var satellites = new List<Satellite>();
        foreach (var logic in logics)
        {
            switch (logic)
            {
                case (StationLogic stt):
                    stations.Add(
                        new Station
                        {
                            name = stt.name,
                            longitude = stt.longitude,
                            latitude = stt.latitude
                        }
                    );
                    break;
                case (SatelliteLogic sat):
                    Satellite satellite;
                    if (sat.satelliteData != null)
                    {
                        satellite = new Satellite
                        {
                            name = sat.name,
                            targetStationName = sat.targetStationName,
                            receiverStationName = sat.receiverStationName,
                            records = GenerateSatelliteRecord(
                                sat.satelliteData,
                                Timeline.startDate,
                                Timeline.endDate,
                                Timeline.timeStep
                            )
                        };
                    }
                    else if (sat.orbitRecord != null)
                    {
                        satellite = new Satellite
                        {
                            name = sat.name,
                            targetStationName = sat.targetStationName,
                            receiverStationName = sat.receiverStationName,
                            records = sat.orbitRecord
                        };
                    }
                    else
                    {
                        satellite = new Satellite();
                    }

                    satellites.Add(satellite);
                    break;
            }
        }

        SatTask task = new SatTask(
            stations,
            satellites,
            Timeline.startDate,
            Timeline.endDate,
            Timeline.timeStep
        );
        return task;
    }

#nullable restore

    public void LoadTask(SatTask task)
    {
        foreach (var station in task.stations)
        {
            var so = station.ToStationSO();
            Guid guid = Guid.NewGuid();
            CreateBrowserObject(guid, so.name);
            CreateInspectorObject(guid, so);
        }
        foreach (var satellite in task.satellites)
        {
            CreateSatelliteFromRecord(satellite);
        }
        Timeline.OnChangeTimeProperties(task.timeStep, task.startDate, task.endDate);
        EventManager.OnObjectUpdated();
    }
}
