using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class EventManager
{
    public static UnityAction<ObjectSO> ObjectCreated;
    public static UnityAction<Guid, bool> ObjectToggled;
    public static UnityAction<double> TimeChanged;

    public static void OnObjectCreated(ObjectSO so) => ObjectCreated?.Invoke(so);

    public static void OnObjectToggled(Guid guid, bool flag) => ObjectToggled?.Invoke(guid, flag);

    public static void OnTimeChanged(double time) => TimeChanged?.Invoke(time);

    /// <summary>
    /// UI Menu Events
    /// </summary>

    public static UnityAction OpenFileClicked;
    public static UnityAction SaveFileClicked;
    public static UnityAction CreateObjectClicked;

    public static void OnOpenFileClicked() => OpenFileClicked?.Invoke();

    public static void OnSaveFileClicked() => SaveFileClicked?.Invoke();

    public static void OnCreateObjectClicked() => CreateObjectClicked?.Invoke();
}
