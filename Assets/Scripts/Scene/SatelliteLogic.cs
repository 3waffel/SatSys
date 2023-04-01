using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SatelliteLogic : ObjectLogic
{
    private SatelliteUtils.SatelliteData _satelliteData;

    protected override void Start()
    {
        base.Start();

        _satelliteData = new SatelliteUtils.SatelliteData();
        transform.localPosition = new Vector3(0, 0.6f, 0);

        EventManager.TimeChanged += UpdateSatelliteRotation;
    }

    void UpdateSatelliteRotation(double time)
    {
        var state = _satelliteData.UpdateSatelliteState(time);
        var position = state.Position;
        var velocity = state.Velocity;
        transform.position = SatelliteUtils.D2F(position);
        // transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
