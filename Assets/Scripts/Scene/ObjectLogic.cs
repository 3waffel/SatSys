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

    [SerializeField]
    protected Transform _targetPlanet;
    public Transform TargetPlanet
    {
        get => _targetPlanet;
        set => _targetPlanet = value;
    }

    protected virtual void Start()
    {
        EventManager.ObjectToggled += OnObjectToggled;
    }

    protected virtual void OnObjectToggled(Guid guid, bool flag)
    {
        if (_guid == guid)
        {
            Debug.Log(guid.ToString() + ", " + flag);
            this.gameObject.SetActive(flag);
        }
    }
}
