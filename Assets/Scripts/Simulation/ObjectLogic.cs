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

        // TODO
        if (uiLabelCanvas == null)
        {
            uiLabelCanvas = transform.parent.Find("LabelCanvas");
            var fc = uiLabelCanvas.gameObject.AddComponent<FacingCamera>();
            fc.targetCamera = FindObjectOfType<Camera>().transform;
            uiLabelCanvas.gameObject.SetActive(false);
        }
    }

    protected virtual void SwitchActive(Guid guid, bool flag)
    {
        if (_guid == guid)
        {
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
            EventManager.OnObjectInfoSent(this);
        }
    }

    [DisallowMultipleComponent]
    class FacingCamera : MonoBehaviour
    {
        public Transform targetCamera;
        public Transform holder;

        void Update()
        {
            if (holder != null)
                transform.position = holder.position + Vector3.up * 0.1f;
        }

        void LateUpdate()
        {
            transform.LookAt(
                transform.position + targetCamera.rotation * Vector3.forward,
                targetCamera.transform.rotation * Vector3.up
            );
        }
    }
}
