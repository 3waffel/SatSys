using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SatSys;

public class SatelliteLogic : ObjectLogic
{
    public SatData.SatelliteData satelliteData = new SatData.SatelliteData();

    public float updateViewInterval = 0.3f;
    private Vector3 nextPosition;

    // Check visibility of stations
    public float elevation = 10f;
    public float maxVisibleDistance = 1f;
    public Collider obstacle;

    public List<SatelliteLogic> visibleSatellites;
    public List<StationLogic> visibleStations;

    protected override void Start()
    {
        base.Start();

        transform.position = new Vector3(0, 0.6f, 0);
        satelliteData.UpdateInternalState();

        EventManager.TimeChanged += UpdateSatelliteState;
        InvokeRepeating("UpdateViewFromState", 0, updateViewInterval);

        if (obstacle == null)
        {
            obstacle = targetPlanet.Find("Sphere").GetComponent<SphereCollider>();
        }
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

    // TODO
    public bool CheckStationVisibility(StationLogic station)
    {
        var direction = station.transform.position - transform.position;
        var distance = direction.magnitude;
        var angle = Vector3.Angle(Vector3.up, direction);
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isBlocked =
            angle > elevation
            && distance > maxVisibleDistance
            && raycast
            && obstacle == hit.collider;
        bool isVisible = !isBlocked;
        return isVisible;
    }

    public bool CheckSatelliteVisibility(SatelliteLogic satellite)
    {
        var direction = satellite.transform.position - transform.position;
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isVisible = !(raycast && obstacle == hit.collider);
        return isVisible;
    }
}
