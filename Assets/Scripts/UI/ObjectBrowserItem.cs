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
        GetComponent<Toggle>().onValueChanged.AddListener(OnToggled);
    }

    private void OnToggled(bool flag)
    {
        EventManager.OnObjectToggled(_guid, flag);
    }
}
