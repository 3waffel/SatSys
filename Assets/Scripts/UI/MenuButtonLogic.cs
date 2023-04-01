using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using MType = MenuButtonSO.MenuButtonType;
using static EventManager;

public class MenuButtonLogic : MonoBehaviour
{
    [SerializeField]
    private MenuButtonSO _buttonSO;

    [SerializeField]
    private Transform _submenuPrefab;

    [SerializeField]
    private Transform _submenuContainer;

    private void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnMenuToggled);

        GetComponentInChildren<TMP_Text>().text = _buttonSO.label;

        foreach (var eventType in _buttonSO.eventList)
        {
            var item = Instantiate(_submenuPrefab);
            item.SetParent(_submenuContainer);
            item.transform.localScale = Vector2.one;

            item.GetComponentInChildren<TMP_Text>().text = eventType.ToString();
            item.GetComponent<Button>()
                .onClick.AddListener(
                    delegate
                    {
                        OnSubmenuClicked(eventType);
                    }
                );
        }
    }

    private void OnSubmenuClicked(MType eventType)
    {
        switch (eventType)
        {
            case (MType.Open):
                OnOpenFileClicked();
                break;
            case (MType.Save):
                OnSaveFileClicked();
                break;
            case (MType.Create):
                OnCreateObjectClicked();
                break;
        }
    }

    public void OnMenuToggled(bool flag)
    {
        _submenuContainer.gameObject.SetActive(flag);
        transform.parent.SetAsLastSibling();
    }
}