using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectBrowserItem : MonoBehaviour
{
    [SerializeField]
    private Guid _guid;
    public Guid Guid
    {
        get => _guid;
        set => _guid = value;
    }

    void Start()
    {
        GetComponentInChildren<Toggle>().onValueChanged.AddListener(
            delegate(bool flag)
            {
                EventManager.OnObjectToggled(_guid, flag);
            }
        );
    }
}
