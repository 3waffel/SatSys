using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SatSys;

public class SatelliteLogic : ObjectLogic
{
    public SatData.SatelliteData satelliteData = new SatData.SatelliteData();

    public float updateViewInterval = 0.3f;
    public float elevation = 10f;

    private Vector3 nextPosition;

    public List<SatelliteLogic> visibleSatellites;
    public List<StationLogic> visibleStations;

    protected override void Start()
    {
        base.Start();

        transform.position = new Vector3(0, 0.6f, 0);
        satelliteData.UpdateInternalState();

        EventManager.TimeChanged += UpdateSatelliteState;
        InvokeRepeating("UpdateViewFromState", 0, updateViewInterval);
    }

    void Update()
    {
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
        nextPosition = SatUtils.Vector3(satelliteData.position * SatUtils.Scale);
    }

    void CheckVisibleStations() { }
}
