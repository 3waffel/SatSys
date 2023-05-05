using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Loader : MonoBehaviour
{
    public static Loader Instance;

    public Transform objectLabelCanvas;
    public Transform infoLabelTemplate;
    public Transform dialogCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
