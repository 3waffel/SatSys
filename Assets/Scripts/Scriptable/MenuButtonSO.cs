using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static EventManager;

[CreateAssetMenu(fileName = "MenuButtonSO", menuName = "GeoSys/MenuButtonSO")]
public class MenuButtonSO : ScriptableObject
{
    public string label;

    public enum EventType
    {
        None,

        // file management
        Open,
        Save,

        // object management
        Create,

        // lines visibility
        Orbits,
        Links,
        Routes,

        // delect selected
        Delete,

        // modify selected
        Modify,
    }

    public EventType type;

    public UnityAction action;

    public void RegisterButton(Transform buttonPrefab, Transform buttonContainer)
    {
        RegisterEvents();

        if (label == null)
            label = type.ToString();

        var item = Instantiate(buttonPrefab);
        item.SetParent(buttonContainer);
        item.transform.localScale = Vector2.one;

        item.GetComponentInChildren<TMP_Text>().text = label;
        item.GetComponent<Button>().onClick.AddListener(action);
    }

    private void RegisterEvents()
    {
        switch (type)
        {
            case (EventType.Open):
                action += FileManager.OpenTask;
                break;
            case (EventType.Save):
                action += FileManager.SaveTask;
                break;
            case (EventType.Orbits):
                action += () =>
                {
                    var orbitsHolder = LineManager.Instance.orbits.gameObject;
                    orbitsHolder.SetActive(!orbitsHolder.activeSelf);
                };
                break;
            case (EventType.Links):
                action += () =>
                {
                    var linksHolder = LineManager.Instance.links.gameObject;
                    linksHolder.SetActive(!linksHolder.activeSelf);
                };
                break;
            case (EventType.Routes):
                action += () =>
                {
                    var routesHolder = LineManager.Instance.routes.gameObject;
                    routesHolder.SetActive(!routesHolder.activeSelf);
                };
                break;
            case (EventType.Create):
                action += () => { };
                break;
            case (EventType.Delete):
                action += () => { };
                break;
            case (EventType.Modify):
                action += () => { };
                break;
        }
    }
}
