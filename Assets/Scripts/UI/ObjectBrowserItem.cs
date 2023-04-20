using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectBrowserItem : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private Guid _guid;
    public Guid Guid
    {
        get => _guid;
        set => _guid = value;
    }

    public void OnSelect(BaseEventData eventData)
    {
        EventManager.OnBrowserItemSelected(_guid);
    }

    void Start()
    {
        GetComponentInChildren<Toggle>().onValueChanged.AddListener(
            delegate(bool flag)
            {
                EventManager.OnBrowserItemToggled(_guid, flag);
            }
        );
    }
}
