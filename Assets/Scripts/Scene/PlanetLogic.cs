using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetLogic : MonoBehaviour
{
    [SerializeField]
    public float selfRotationSpeed = 24;

    [SerializeField]
    private double _startTime = SatelliteUtils.startJulianDate;

    [SerializeField]
    private double _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);

    void Start()
    {
        ShowAxis();
    }

    void Update()
    {
        _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);
        UpdateEarthRotation((float)(_currentTime - _startTime));
    }

    void UpdateEarthRotation(float epoch)
    {
        float angle = epoch * 360 * selfRotationSpeed;
        transform.rotation = Quaternion.Euler(0, (float)angle, 0);
    }

    void ShowAxis()
    {
        Debug.DrawLine(Vector3.zero, Vector3.right * 20, Color.red);
        Debug.DrawLine(Vector3.zero, Vector3.up * 20, Color.green);
        Debug.DrawLine(Vector3.zero, Vector3.forward * 20, Color.blue);
    }
}
