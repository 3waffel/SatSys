using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MenuButtonSO", menuName = "GeoSys/MenuButtonSO")]
public class MenuButtonSO : ScriptableObject
{
    public string label;

    public List<MenuButtonType> eventList;

    public enum MenuButtonType
    {
        Open,
        Save,
        Create,
        None,
    }
}
