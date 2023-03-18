using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectSO", menuName = "GeoSys/ObjectSO")]
public class ObjectSO : ScriptableObject
{
    [SerializeField] public string label;
    [SerializeField] public Transform itemPrefab;
}
