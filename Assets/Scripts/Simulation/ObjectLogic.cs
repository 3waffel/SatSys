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
        EventManager.BrowserItemToggled += OnObjectToggled;
    }

    protected virtual void OnObjectToggled(Guid guid, bool flag)
    {
        if (_guid == guid)
        {
            this.gameObject.SetActive(flag);
            EventManager.OnObjectUpdated();
        }
    }
}
