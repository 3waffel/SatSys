using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuButtonLogic : MonoBehaviour
{
    [SerializeField]
    private List<MenuButtonSO> _buttons;

    [SerializeField]
    private Transform _submenuPrefab;

    [SerializeField]
    private Transform _submenuContainer;

    private void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnMenuToggled);
        _buttons.ForEach((button) => button.RegisterButton(_submenuPrefab, _submenuContainer));
    }

    public void OnMenuToggled(bool flag)
    {
        _submenuContainer.gameObject.SetActive(flag);
        // transform.parent.SetAsLastSibling();
    }
}
