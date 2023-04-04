using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SatelliteLogic : ObjectLogic
{
    public SatelliteUtils.SatelliteData satelliteData;

    protected override void Start()
    {
        base.Start();

        transform.localPosition = new Vector3(0, 0.6f, 0);

        satelliteData = new SatelliteUtils.SatelliteData();
        satelliteData.CalculateStateExternal();

        EventManager.TimeChanged += UpdateSatelliteRotation;
    }

    void UpdateSatelliteRotation(double time)
    {
        // satelliteData.UpdateSatelliteState(time);
        satelliteData.UpdateStateExternal(Time.deltaTime * 0.001f);
        var velocity = satelliteData.velocity;

        transform.position = Vector3.Lerp(
            transform.position,
            SatelliteUtils.D2F(satelliteData.position),
            1f
        );
        // transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
