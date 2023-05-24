using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[DisallowMultipleComponent]
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

    public static Transform uiLabelCanvas;
    public static TMP_Text uiLabel => uiLabelCanvas.GetComponentInChildren<TMP_Text>();

    protected virtual void Start()
    {
        EventManager.BrowserItemToggled += SwitchActive;
        EventManager.BrowserItemSelected += OnItemSelected;
        EventManager.ObjectDeleted += () =>
        {
            if (EventManager.SelectedGuid == _guid)
            {
                uiLabelCanvas.gameObject.SetActive(false);
                GameObject.Destroy(gameObject);
            }
        };

        if (uiLabelCanvas == null)
        {
            uiLabelCanvas = Instantiate(Loader.Instance.objectLabelCanvas);
            uiLabelCanvas.gameObject.SetActive(false);
        }
    }

    protected virtual void SwitchActive(Guid guid, bool flag)
    {
        if (_guid == guid)
        {
            uiLabelCanvas.gameObject.SetActive(false);
            this.gameObject.SetActive(flag);
            EventManager.OnObjectUpdated();
        }
    }

    protected virtual void OnItemSelected(Guid guid)
    {
        if (_guid == guid)
        {
            uiLabelCanvas.gameObject.SetActive(true);
            uiLabelCanvas.GetComponent<FacingCamera>().holder = transform;
            uiLabel.text = name;
            EventManager.OnShowObjectInfo(this);
        }
    }
}
