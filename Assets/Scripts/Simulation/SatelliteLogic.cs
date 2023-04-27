using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SatSys;
using UnityEngine.Events;

public class SatelliteLogic : ObjectLogic
{
    public SatData.SatelliteData satelliteData;

    /// <summary>
    /// interval to update `nextPosition`
    /// </summary>
    public float updatePositionInterval = 0.3f;

    /// <summary>
    /// interval to update view position using `Lerp`
    /// </summary>
    public float lerpInterval = 0.1f;

    /// <summary>
    /// next position the satellite will lerp to,
    /// updated by `updatePositionAction`
    /// </summary>
    private Vector3 nextPosition;

    UnityAction updatePositionAction;
    UnityAction updateViewAction;

    /// <summary>
    /// collider of the central body, defined by `targetPlanet`
    /// </summary>
    public Collider obstacleCollider;

    // elevation when checking visibility of a certain station
    public float elevation = 10f;

    // max distance from a station for it to be visible
    public float maxVisibleDistance = 10f;

    // intermedia records
    public List<SatelliteLogic> visibleSatellites;
    public List<StationLogic> visibleStations;

    // task for checking link between target station and receiver station
    public string targetStationName;
    public string receiverStationName;
    public StationLogic targetStation;
    public StationLogic receiverStation;
    public List<ObjectLogic> linkRoute;

    void Awake()
    {
        if (satelliteData == null)
        {
            satelliteData = new SatData.SatelliteData();
        }
        satelliteData.UpdateInternalState();
    }

    protected override void Start()
    {
        base.Start();

        transform.position = SatUtils.Vector3(satelliteData.position * SatUtils.Scale);
        targetStation = targetPlanet.Find(targetStationName).GetComponent<StationLogic>();
        receiverStation = targetPlanet.Find(receiverStationName).GetComponent<StationLogic>();

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
        UpdateLinkRoute();
    }

    void UpdatePositionTask()
    {
        updatePositionAction?.Invoke();
    }

    /// <summary>
    /// initialize movement based on direct calculation of orbital elements,
    /// `nextPosition` is provided by
    /// </summary>
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

    // TODO movement based on rotation and ecllipse shape
    void InitializeEllipseMovement()
    {
        updatePositionAction = () => { };
        updateViewAction = () => { };
    }

    // TODO judging from elevation, distance and obstacle's collider
    public bool CheckStationVisibility(StationLogic station)
    {
        var direction = station.transform.position - transform.position;
        var distance = direction.magnitude;
        var angle = Vector3.Angle(Vector3.up, direction);
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isBlocked =
        // angle > elevation
        // || distance > maxVisibleDistance ||
        (raycast && obstacleCollider == hit.collider);
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

    // TODO check if is a link is blocked by central body, in a mathematical way
    public bool CentralBodyBlockChecker(Vector3 position)
    {
        // float centralBodyRadius = 0.5f;
        Vector3 toCenter = transform.position - targetPlanet.position;
        Vector3 toTarget = transform.position - position;
        var angle = Vector3.Angle(toCenter, toTarget);
        return false;
    }

    /// <summary>
    /// get a route from target station to receiver station
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dest"></param>
    /// <returns>a list of objects on the route</returns>
    public static List<ObjectLogic> CheckStationLinkRoute(StationLogic target, StationLogic dest)
    {
        if (target == dest)
            return null;

        var visibles = target.GetVisibleSatellites();
        if (visibles.Count != 0)
        {
            var route = new List<ObjectLogic>();
            route.Add(target);
            foreach (var sat in visibles)
            {
                if (sat.CheckStationVisibility(dest))
                {
                    route.Add(sat);
                    break;
                }
            }
            route.Add(dest);
            return route;
        }

        return null;
    }

    // the route will be only traced one layer
    public void UpdateLinkRoute()
    {
        if (targetStation == null || receiverStation == null || targetStation == receiverStation)
        {
            linkRoute = null;
            return;
        }

        var route = new List<ObjectLogic>();
        if (CheckStationVisibility(targetStation))
        {
            route.Add(targetStation);
            route.Add(this);
            if (CheckStationVisibility(receiverStation))
            {
                route.Add(receiverStation);
                linkRoute = route;
                return;
            }
            else
            {
                var visibleSats = receiverStation.GetVisibleSatellites();
                foreach (var sat in visibleSats)
                {
                    if (CheckSatelliteVisibility(sat))
                    {
                        route.Add(sat);
                        route.Add(receiverStation);
                        linkRoute = route;
                        return;
                    }
                }
                // couldn't find a route
                linkRoute = null;
                return;
            }
        }
        else
        {
            linkRoute = null;
        }
    }
}
