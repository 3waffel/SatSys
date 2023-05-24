using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Transform infoContainer;

    static ObjectLogic currentObject;

    void Start()
    {
        EventManager.ShowObjectInfo += (obj) =>
        {
            currentObject = obj;
            UpdateInfo();
        };
        EventManager.TimeChanged += (_) => UpdateInfo();
        EventManager.ObjectUpdated += UpdateInfo;

        if (infoContainer == null)
        {
            infoContainer = transform.Find("InfoContainer");
        }
    }

    void UpdateInfo()
    {
        // clear current info
        foreach (Transform item in infoContainer.transform)
        {
            GameObject.Destroy(item.gameObject);
        }
        if (currentObject != null)
        {
            switch (currentObject)
            {
                case SatelliteLogic satellite:
                    CreateLabel("Name:\t", satellite.name);
                    CreateLabel("Target:\t", satellite.targetStation?.name ?? "Null");
                    CreateLabel("Receiver:\t", satellite.receiverStation?.name ?? "Null");
                    if (satellite.orbitRecord == null)
                    {
                        var data = satellite.satelliteData;
                        string pos =
                            $"x: {data.position.x}\n\t\ty: {data.position.y}\n\t\tz: {data.position.z}";
                        string vel =
                            $"x: {data.velocity.x}\n\t\ty: {data.velocity.y}\n\t\tz: {data.velocity.z}";
                        CreateLabel("Position:\t", pos);
                        CreateLabel("Velocity:\t", vel);

                        CreateLabel(
                            "SemiMajorAxis:\t",
                            satellite.satelliteData.elements.SemiMajorAxis.ToString()
                        );
                        CreateLabel(
                            "Eccentricity:\t",
                            satellite.satelliteData.elements.Eccentricity.ToString()
                        );
                        CreateLabel(
                            "Inclination:\t",
                            satellite.satelliteData.elements.Inclination.ToString()
                        );
                        CreateLabel(
                            "Periapsis:\t\t",
                            satellite.satelliteData.elements.Periapsis.ToString()
                        );
                        CreateLabel(
                            "AscendingNode:\t",
                            satellite.satelliteData.elements.AscendingNode.ToString()
                        );
                    }
                    else
                    {
                        var record = satellite.orbitRecord;
                        var data = record[satellite.recordIndex];
                        string pos =
                            $"x: {data.position.x}\n\t\ty: {data.position.y}\n\t\tz: {data.position.z}";
                        CreateLabel("Position:\t", pos);
                    }
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
    }

    void CreateLabel(string text)
    {
        var template = Loader.Instance.infoLabelTemplate.GetComponent<TextMeshProUGUI>();
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
