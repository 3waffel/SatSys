using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent<ObjectSO> onObjectCreated;
    public UnityEvent onObjectToggled;
    public UnityEvent<double> onTimeChanged;
}