using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SatelliteLogic : ObjectLogic
{
    private SatelliteUtils.SatelliteData _satelliteData;

    [SerializeField]
    private double _startTime = SatelliteUtils.startJulianDate;

    [SerializeField]
    private double _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);

    protected override void Start()
    {
        base.Start();

        _satelliteData = new SatelliteUtils.SatelliteData();
        transform.localPosition = new Vector3(0, 0.6f, 0);
    }

    void Update()
    {
        _currentTime = SatelliteUtils.GetJulianDate(DateTime.Now);
        UpdateSatelliteRotation((float)(_currentTime - _startTime));
    }

    void UpdateSatelliteRotation(float time)
    {
        var state = _satelliteData.UpdateSatelliteState(time);
        var position = state.Position;
        var velocity = state.Velocity;
        transform.position = SatelliteUtils.D2F(position);
        // transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
