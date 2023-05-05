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
        EventManager.SelectedGuid = _guid;
        EventManager.OnBrowserItemSelected(_guid);
    }

    void Start()
    {
        GetComponentInChildren<Toggle>().onValueChanged.AddListener(
            (flag) => EventManager.OnBrowserItemToggled(_guid, flag)
        );
        EventManager.ObjectDeleted += () =>
        {
            if (EventManager.SelectedGuid == _guid)
            {
                GameObject.Destroy(gameObject);
            }
        };
    }
}
