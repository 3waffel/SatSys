using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static EventManager;
using static SatSys.SatData;
using static SatSys.SatElements;
using SatSys;
using UI.Dialogs;

[CreateAssetMenu(fileName = "MenuButtonSO", menuName = "GeoSys/MenuButtonSO")]
public class MenuButtonSO : ScriptableObject
{
    public string label;

    public enum EventType
    {
        None,

        // load file
        Open,

        // save scene to file
        Save,

        // object management
        Create,

        // show orbits
        Orbits,

        // show sat links
        Links,

        // show info routes
        Routes,

        // delect selected
        Delete,

        // modify selected
        Modify,

        // clear scene
        Clear,

        // test spawn
        Test,

        // enable fixed camera follow
        Fixed,

        // draw orbit in earth frame
        Draw,
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
                action += FileManager.OpenFile;
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
                var holder = Loader.Instance.dialogCanvas;
                var satDialog = holder.Find("SatelliteInputDialog").GetComponent<uDialog>();
                var sttDialog = holder.Find("StationInputDialog").GetComponent<uDialog>();
                satDialog.Buttons[0].OnClick = () =>
                {
                    var input = satDialog.GetComponentInChildren<InputArea>();
                    var values = input.GetInputValues();
                    var satSO = ScriptableObject.CreateInstance<SatelliteSO>();
                    if (
                        double.TryParse(values[0], out double res0)
                        && double.TryParse(values[1], out double res1)
                        && double.TryParse(values[2], out double res2)
                        && double.TryParse(values[3], out double res3)
                        && double.TryParse(values[4], out double res4)
                        && double.TryParse(values[5], out double res5)
                        && res0 > SatUtils.EarthRadius
                        && res1 >= 0
                        && res1 < 1
                    )
                    {
                        satSO.satelliteData = new SatelliteData(
                            new KeplerianElements
                            {
                                SemiMajorAxis = res0,
                                Eccentricity = res1,
                                Inclination = res2,
                                Periapsis = res3,
                                AscendingNode = res4,
                                MeanAnomaly = res5,
                            }
                        );
                        satSO.name = values[6] ?? "New Satellite";
                        satSO.targetStationName = values[7] ?? "Null";
                        satSO.receiverStationName = values[8] ?? "Null";
                        EventManager.OnObjectCreated(satSO);
                    }
                    else
                    {
                        CreateSimpleDialog()
                            .SetTitleText("Error")
                            .SetContentText("Invalid Input")
                            .AddButton("Confirm", () => { });
                    }
                };
                sttDialog.Buttons[0].OnClick = () =>
                {
                    var input = sttDialog.GetComponentInChildren<InputArea>();
                    var values = input.GetInputValues();
                    var sttSO = ScriptableObject.CreateInstance<StationSO>();
                    if (
                        float.TryParse(values[0], out float res0)
                        && float.TryParse(values[1], out float res1)
                    )
                    {
                        sttSO.longitude = res0;
                        sttSO.latitude = res1;
                        sttSO.name = values[2] ?? "New Station";
                        EventManager.OnObjectCreated(sttSO);
                    }
                    else
                    {
                        CreateSimpleDialog()
                            .SetTitleText("Warning")
                            .SetContentText("Invalid Input")
                            .AddButton("Confirm", () => { });
                    }
                };
                action = () =>
                {
                    CreateSimpleDialog()
                        .SetTitleText("Notice")
                        .SetContentText("Please select type of the creation.")
                        .AddButton("Satellite", () => satDialog.Show())
                        .AddButton("Station", () => sttDialog.Show())
                        .AddButton("Cancel", () => { });
                };
                break;
            case (EventType.Delete):
                action = () =>
                    CreateSimpleDialog()
                        .SetTitleText("Notice")
                        .SetContentText("Are you sure to delete the object?")
                        .AddButton("Confirm", () => EventManager.OnObjectDeleted())
                        .AddButton("Cancel", () => { });
                break;
            case (EventType.Modify):
                action = () =>
                {
                    CreateSimpleDialog()
                        .SetTitleText("Notice")
                        .SetContentText("Please delete then create the object")
                        // .AddButton("Confirm", () => EventManager.OnObjectDeleted())
                        .AddButton("Cancel", () => { });
                };
                break;
            case (EventType.Clear):
                action = () =>
                    CreateSimpleDialog()
                        .SetTitleText("Notice")
                        .SetContentText("Are you sure to clear the scene?")
                        .AddButton("Confirm", () => ObjectManager.Instance.ClearScene())
                        .AddButton("Cancel", () => { });
                break;
            case (EventType.Test):
                action = () =>
                    CreateSimpleDialog()
                        .SetTitleText("Notice")
                        .SetContentText("Are you sure to test spawn?")
                        .AddButton("Confirm", () => ObjectManager.Instance.RandomSpawnTest())
                        .AddButton("Cancel", () => { });
                break;
            case (EventType.Fixed):
                action = () =>
                {
                    var camera = FindObjectOfType<InspectorCamera>().transform;
                    var planet = FindObjectOfType<PlanetLogic>().transform;
                    if (camera.parent != planet)
                    {
                        camera.parent = planet;
                    }
                    else
                    {
                        camera.parent = planet.parent;
                    }
                };
                break;
        }
    }

    uDialog CreateSimpleDialog()
    {
        return uDialog
            .NewDialog(Loader.Instance.dialogCanvas.GetComponent<RectTransform>())
            .SetModal(true)
            .SetDestroyAfterClose(true)
            .SetShowTitleCloseButton(false)
            .SetCloseWhenAnyButtonClicked(true)
            .SetShowAnimation(eShowAnimation.FadeIn)
            .SetCloseAnimation(eCloseAnimation.FadeOut);
    }
}
