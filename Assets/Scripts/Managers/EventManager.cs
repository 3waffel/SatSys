using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using static SatSys.SatData;

public static class EventManager
{
    // Main Logic Events
    public static UnityAction<ObjectSO> ObjectCreated;
    public static UnityAction ObjectUpdated;
    public static UnityAction<double> TimeChanged;
    public static UnityAction<double, double> TimeRangeChanged;
    public static UnityAction<float> TimeStepChanged;

    public static void OnObjectCreated(ObjectSO so) => ObjectCreated?.Invoke(so);

    public static void OnObjectUpdated() => ObjectUpdated?.Invoke();

    /// <summary>
    /// Elapsed time since start time
    /// </summary>
    /// <param name="time">elapsed time</param>
    public static void OnTimeChanged(double time) => TimeChanged?.Invoke(time);

    public static void OnTimeRangeChanged(double startTime, double endTime) =>
        TimeRangeChanged?.Invoke(startTime, endTime);

    public static void OnTimeStepChange(float timeScale) => TimeStepChanged?.Invoke(timeScale);

    // UI Browser Events
    public static UnityAction<Guid, bool> BrowserItemToggled;
    public static UnityAction<Guid> BrowserItemSelected;
    public static UnityAction<ObjectLogic> ObjectInfoSent;

    public static void OnBrowserItemToggled(Guid guid, bool flag) =>
        BrowserItemToggled?.Invoke(guid, flag);

    /// <summary>
    /// trigger when the browser item is selected
    /// </summary>
    public static void OnBrowserItemSelected(Guid id) => BrowserItemSelected?.Invoke(id);

    /// <summary>
    /// Send info from the scene object to the display window
    /// /// </summary>
    public static void OnObjectInfoSent(ObjectLogic logic) => ObjectInfoSent?.Invoke(logic);
}
