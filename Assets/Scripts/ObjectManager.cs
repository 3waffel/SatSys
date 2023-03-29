using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ObjectManager : MonoBehaviour
{
    // Object Browser
    [SerializeField]
    private Transform _contentContainer;

    [SerializeField]
    private Transform _browserItemPrefab;

    // Inspector
    [SerializeField]
    private List<ObjectSO> _objectCollection;

    [SerializeField]
    private Transform _inspectorScene;

    [SerializeField]
    private Transform _targetPlanet;

    void Start()
    {
        EventManager.ObjectCreated += OnObjectCreated;

        SpawnFromCollection();
    }

    void SpawnFromCollection()
    {
        if (_objectCollection.Count > 0)
        {
            foreach (var os in _objectCollection)
            {
                Guid guid = Guid.NewGuid();
                CreateBrowserObject(guid, os);
                CreateInspectorObject(guid, os);
            }
        }
    }

    void BrowserSpawnTest()
    {
        int itemsToGenerate = 5;
        for (int i = 0; i < itemsToGenerate; i++)
        {
            var item = Instantiate(_browserItemPrefab);
            var label = "Test Item " + i;

            item.GetComponentInChildren<TMP_Text>().text = label;
            item.transform.SetParent(_contentContainer);
            item.transform.localScale = Vector2.one;
        }
    }

    void CreateBrowserObject(Guid guid, ObjectSO obj)
    {
        var item = Instantiate(_browserItemPrefab);
        var label = obj.label;

        item.GetComponentInChildren<TMP_Text>().text = label;
        item.GetComponentInChildren<ObjectBrowserItem>().Guid = guid;
        item.transform.SetParent(_contentContainer);
        item.transform.localScale = Vector2.one;
    }

    void CreateInspectorObject(Guid guid, ObjectSO obj)
    {
        var item = Instantiate(obj.itemPrefab);
        item.transform.SetParent(_inspectorScene);

        var logic = item.GetComponent<ObjectLogic>();
        if (logic != null)
        {
            logic.Guid = guid;
            logic.TargetPlanet = _targetPlanet;
        }
    }

    void OnObjectCreated(ObjectSO so)
    {
        Guid guid = Guid.NewGuid();
        CreateBrowserObject(guid, so);
        CreateInspectorObject(guid, so);
    }
}
