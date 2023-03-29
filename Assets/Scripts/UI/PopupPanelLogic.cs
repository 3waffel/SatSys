using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPanelLogic : MonoBehaviour
{
    void Start()
    {
        EventManager.CreateObjectClicked += OnCreateObjectClicked;
    }

    private void AcceptElements() { }

    private void OnCreateObjectClicked()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
