using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class EventManager
{
    // Main Logic Events
    public static UnityAction<ObjectSO> ObjectCreated;
    public static UnityAction<Guid, bool> ObjectToggled;
    public static UnityAction<double> TimeChanged;
    public static UnityAction<double, double> TimeRangeChanged;

    public static void OnObjectCreated(ObjectSO so) => ObjectCreated?.Invoke(so);

    public static void OnObjectToggled(Guid guid, bool flag) => ObjectToggled?.Invoke(guid, flag);

    /// <summary>
    /// Elapsed Time Since Start Time
    /// </summary>
    /// <param name="time"></param>
    public static void OnTimeChanged(double time) => TimeChanged?.Invoke(time);

    public static void OnTimeRangeChanged(double startTime, double endTime) =>
        TimeRangeChanged?.Invoke(startTime, endTime);

    // UI Menu Events
    public static UnityAction OpenButtonClicked;
    public static UnityAction SaveButtonClicked;
    public static UnityAction CreateButtonClicked;

    public static void OnOpenButtonClicked() => OpenButtonClicked?.Invoke();

    public static void OnSaveButtonClicked() => SaveButtonClicked?.Invoke();

    public static void OnCreateButtonClicked() => CreateButtonClicked?.Invoke();
}
