using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectLogic : MonoBehaviour
{
    [SerializeField]
    protected Guid _guid;
    public Guid Guid
    {
        get => _guid;
        set => _guid = value;
    }

    public Transform targetPlanet;

    protected virtual void Start()
    {
        EventManager.BrowserItemToggled += SwitchActive;
        EventManager.BrowserItemSelected += ShowInfo;
    }

    protected virtual void SwitchActive(Guid guid, bool flag)
    {
        if (_guid == guid)
        {
            this.gameObject.SetActive(flag);
            EventManager.OnObjectUpdated();
        }
    }

    protected virtual void ShowInfo(Guid guid)
    {
        if (_guid == guid)
        {
            EventManager.OnObjectInfoSent(this);
        }
    }
}
