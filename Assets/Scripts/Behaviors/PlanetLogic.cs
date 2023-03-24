using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetLogic : MonoBehaviour
{
    [SerializeField] public float selfRotationSpeed = 24;
    [SerializeField] private double _startTime = SatelliteUtils.GetJulianDate(DateTime.Now);
    [SerializeField] private double _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);

    void Update()
    {
        _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);
        EarthRotation((float)(_currentTime - _startTime));
    }

    void EarthRotation(float epoch)
    {
        float angle = epoch * 360 * selfRotationSpeed;
        transform.rotation = Quaternion.Euler(0, (float)angle, 0);
    }
}
