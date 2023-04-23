using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Transform infoContainer;

    void Start()
    {
        EventManager.ObjectInfoSent += UpdateInfo;

        if (infoContainer == null)
        {
            infoContainer = transform.Find("InfoContainer");
        }

        // var templateObject = new GameObject("TemplateLabel");
        // templateObject.transform.SetParent(transform);
        // templateObject.transform.localScale = Vector2.one;

        // var label = templateObject.AddComponent<TextMeshProUGUI>();
        // label.color = Color.black;
        // label.fontSize = 16;
        // label.fontStyle = FontStyles.Bold;
    }

    void UpdateInfo(ObjectLogic logic)
    {
        foreach (Transform item in infoContainer.transform)
        {
            GameObject.Destroy(item.gameObject);
        }
        switch (logic)
        {
            case SatelliteLogic satellite:
                var data = satellite.satelliteData;
                CreateLabel("Name:\t", satellite.name);
                CreateLabel("Position:\t", data.position.ToString());
                CreateLabel("Velocity:\t", data.velocity.ToString());
                break;
            case StationLogic station:
                CreateLabel("Name:\t", station.name);
                CreateLabel("Longitude:\t", station.longitude.ToString());
                CreateLabel("Latitude:\t", station.latitude.ToString());
                break;
            default:
                break;
        }
    }

    void CreateLabel(string text)
    {
        var template = transform.Find("TemplateLabel").GetComponent<TextMeshProUGUI>();
        var label = Instantiate(template);
        label.text = text;
        label.transform.SetParent(infoContainer.transform);
    }

    void CreateLabel(
        string left,
        string right,
        string leftColor = "#222222",
        string rightColor = "#666666"
    )
    {
        CreateLabel($"<color={leftColor}>{left}</color>" + $"<color={rightColor}>{right}</color>");
    }
}
