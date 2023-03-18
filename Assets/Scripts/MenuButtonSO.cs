using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuButtonSO", menuName = "GeoSys/MenuButtonSO")]
public class MenuButtonSO : ScriptableObject
{
    public string label;
    public Transform prefab;
}
