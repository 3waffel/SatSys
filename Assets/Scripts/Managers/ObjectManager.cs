using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static SatSys.SatData;

public class ObjectManager : MonoBehaviour
{
    // Object Browser
    public Transform contentContainer;
    public Transform browserItemPrefab;

    // ObjectSO Collection
    public List<ObjectSO> objectCollection;

    public Transform targetScene;
    public Transform targetPlanet;

    public Transform satellitePrefab;

    void Start()
    {
        EventManager.ObjectCreated += OnObjectCreated;

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
                CreateBrowserObject(guid, so.label);
                CreateInspectorObject(guid, so);
            }
        }
        EventManager.OnObjectUpdated();
    }

    void BrowserSpawnTest()
    {
        int itemsToGenerate = 5;
        for (int i = 0; i < itemsToGenerate; i++)
        {
            var item = Instantiate(browserItemPrefab);
            var label = "Test Item " + i;

            item.GetComponentInChildren<TMP_Text>().text = label;
            item.transform.SetParent(contentContainer);
            item.transform.localScale = Vector2.one;
        }
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
        obj.Spawn(guid);
    }

    void OnObjectCreated(ObjectSO so)
    {
        Guid guid = Guid.NewGuid();
        CreateBrowserObject(guid, so.label);
        CreateInspectorObject(guid, so);
    }

    void CreateSatelliteFromData(SatelliteData data, string label)
    {
        Guid guid = Guid.NewGuid();

        CreateBrowserObject(guid, label);

        var sat = Instantiate(satellitePrefab);
        sat.name = label;
        sat.transform.SetParent(targetScene);
        var logic = sat.GetComponent<SatelliteLogic>();
        logic.Guid = guid;
        logic.targetPlanet = targetPlanet;
        logic.satelliteData = data;
    }

    void ClearScene() { }

    public void LoadTask() { }
}
