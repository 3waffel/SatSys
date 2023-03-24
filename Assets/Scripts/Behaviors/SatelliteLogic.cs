using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SatelliteLogic : ObjectLogic
{
    private SatelliteUtils.Satellite _satellite;
    [SerializeField] private float _rotationSpeed = 20;
    [SerializeField] private double _startTime = SatelliteUtils.GetJulianDate(DateTime.Now);
    [SerializeField] private double _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);

    void Start()
    {
        _satellite = new SatelliteUtils.Satellite();
        transform.localPosition = new Vector3(0, 0.6f, 0);
    }

    void Update()
    {
        _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);
        SatelliteRotation((float)(_currentTime - _startTime));
    }

    void SatelliteRotation(float time)
    {
        var state = _satellite.GetSatelliteState(time);
        Debug.Log(state);
        var position = state.Position;
        var velocity = state.Velocity;
        transform.position = position;
        // transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
