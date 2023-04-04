using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SatSys;

public class SatelliteLogic : ObjectLogic
{
    public SatData.SatelliteData satelliteData = new SatData.SatelliteData();

    public float updateViewInterval = 0.3f;

    private Vector3 currentPosition;
    private Vector3 currentVelocity;
    private Vector3 nextPosition;

    protected override void Start()
    {
        base.Start();

        transform.position = new Vector3(0, 0.6f, 0);
        satelliteData.UpdateInternalState();
        currentPosition = SatUtils.Vector3(satelliteData.position);
        currentVelocity = SatUtils.Vector3(satelliteData.velocity);

        EventManager.TimeChanged += UpdateSatelliteState;
        InvokeRepeating("UpdateViewFromState", 0, updateViewInterval);
    }

    void Update()
    {
        // transform.RotateAround(Vector3.zero, Vector3.right, Time.deltaTime * 10);
        transform.position = Vector3.Lerp(
            transform.position,
            targetPlanet.position + nextPosition,
            0.1f
        );
    }

    void UpdateSatelliteState(double time)
    {
        satelliteData.UpdateAnomaly(time);
    }

    void UpdateViewFromState()
    {
        // var nextPosition = currentPosition + Time.deltaTime * currentVelocity;
        nextPosition = SatUtils.Vector3(satelliteData.position);
        // transform.position = nextPosition;
    }
}
