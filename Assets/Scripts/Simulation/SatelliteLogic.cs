using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SatSys;
using UnityEngine.Events;

public class SatelliteLogic : ObjectLogic
{
    public SatData.SatelliteData satelliteData = new SatData.SatelliteData();

    public float updatePositionInterval = 0.3f;
    public float lerpInterval = 0.1f;
    private Vector3 nextPosition;

    UnityAction updatePositionAction;
    UnityAction updateViewAction;

    // Check visibility of stations
    public float elevation = 10f;
    public float maxVisibleDistance = 1f;
    public Collider obstacleCollider;

    public List<SatelliteLogic> visibleSatellites;
    public List<StationLogic> visibleStations;

    protected override void Start()
    {
        base.Start();

        satelliteData.UpdateInternalState();
        transform.position = SatUtils.Vector3(satelliteData.position * SatUtils.Scale);

        EventManager.TimeChanged += (time) => satelliteData.UpdateAnomaly(time);
        EventManager.TimeScaleChanged += (timeScale) =>
            lerpInterval = Mathf.Clamp(timeScale * 30, 0.1f, 1);

        InitializeDirectMovement();
        InvokeRepeating(nameof(UpdatePositionTask), 0, updatePositionInterval);

        if (obstacleCollider == null)
        {
            obstacleCollider = targetPlanet.Find("Sphere").GetComponent<SphereCollider>();
        }
    }

    void Update()
    {
        updateViewAction?.Invoke();
    }

    void UpdatePositionTask()
    {
        updatePositionAction?.Invoke();
    }

    // movement based on elements calculation
    void InitializeDirectMovement()
    {
        updatePositionAction = () =>
            nextPosition = SatUtils.Vector3(satelliteData.position * SatUtils.Scale);
        updateViewAction = () =>
            transform.position = Vector3.Lerp(
                transform.position,
                targetPlanet.position + nextPosition,
                lerpInterval
            );
    }

    // movement based on rotation and ecllipse shape
    void InitializeEllipseMovement()
    {
        updatePositionAction = () => { };
        updateViewAction = () => { };
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
            && obstacleCollider == hit.collider;
        bool isVisible = !isBlocked;
        return isVisible;
    }

    public bool CheckSatelliteVisibility(SatelliteLogic satellite)
    {
        var direction = satellite.transform.position - transform.position;
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isVisible = !(raycast && obstacleCollider == hit.collider);
        return isVisible;
    }

    // check if is a link is blocked by central body, in a mathematical way
    public bool CentralBodyBlockChecker(float radius, float elevation)
    {
        return false;
    }
}
