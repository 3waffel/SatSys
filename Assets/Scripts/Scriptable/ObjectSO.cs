using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ObjectSO", menuName = "GeoSys/ObjectSO")]
public class ObjectSO : ScriptableObject
{
    public string label;
    public Transform itemPrefab;
    public Transform targetScene { get; set; }
    public Transform targetPlanet { get; set; }

    public virtual Transform Spawn(Guid guid)
    {
        Debug.Assert(targetScene != null && targetPlanet != null);

        var item = Instantiate(itemPrefab);
        item.transform.SetParent(targetScene);

        var logic = item.GetComponent<ObjectLogic>();
        logic.Guid = guid;
        logic.targetPlanet = targetPlanet;

        return item;
    }
}
