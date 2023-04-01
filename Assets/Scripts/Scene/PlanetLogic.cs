using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetLogic : MonoBehaviour
{
    [SerializeField]
    public float selfRotationSpeed = 24;

    void Start()
    {
        // ShowAxis();
        EventManager.TimeChanged += UpdateEarthRotation;
    }

    void UpdateEarthRotation(double epoch)
    {
        float angle = (float)(epoch * 360 * selfRotationSpeed);
        transform.rotation = Quaternion.Euler(0, (float)angle, 0);
    }

    void ShowAxis()
    {
        Debug.DrawLine(Vector3.zero, Vector3.right * 20, Color.red);
        Debug.DrawLine(Vector3.zero, Vector3.up * 20, Color.green);
        Debug.DrawLine(Vector3.zero, Vector3.forward * 20, Color.blue);
    }
}
