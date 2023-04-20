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
        Open,
        Save,
        Create,
        None,
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
            case (EventType.Create):
                action += () => Debug.Log("Create Button Clicked");
                break;
        }
    }
}
