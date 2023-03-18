using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private EventManager _eventManager;

    // Object Browser
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private Transform _browserItemPrefab;
    [SerializeField] private int _itemsToGenerate = 5;

    // Inspector
    [SerializeField] private List<ObjectSO> _objectCollection;
    [SerializeField] private Transform _inspectorScene;

    [SerializeField] private List<Transform> _objectRecord;

    void Start()
    {
        SpawnFromCollection();
    }

    void SpawnFromCollection()
    {
        if (_objectCollection.Count > 0)
        {
            foreach (var item in _objectCollection)
            {
                CreateBrowserObject(item);
                CreateInspectorObject(item);
            }
        }
    }

    void BrowserSpawnTest()
    {
        for (int i = 0; i < _itemsToGenerate; i++)
        {
            var item = Instantiate(_browserItemPrefab);
            var label = "Test Item" + i;

            item.GetComponentInChildren<TMP_Text>().text = label;
            item.transform.SetParent(_contentContainer);
            item.transform.localScale = Vector2.one;
        }
    }

    void CreateBrowserObject(ObjectSO obj)
    {
        var item = Instantiate(_browserItemPrefab);
        var label = obj.label;

        item.GetComponentInChildren<TMP_Text>().text = label;
        item.transform.SetParent(_contentContainer);
        item.transform.localScale = Vector2.one;
    }

    void CreateInspectorObject(ObjectSO obj)
    {
        var item = Instantiate(obj.itemPrefab);
        var label = obj.label;
        _objectRecord.Add(item);

        item.transform.SetParent(_inspectorScene);

        var logic = item.GetComponent<ObjectLogic>();
        if (logic != null) {
            logic.TargetPlanet = _inspectorScene.GetChild(0);
        }
    }
}
