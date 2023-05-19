using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SatSys;
using UnityEngine.Events;

public class SatelliteLogic : ObjectLogic
{
    public SatData.SatelliteData satelliteData;
    public List<SatRecord.TimedPosition> orbitRecord;

    /// <summary>
    /// interval to update `nextPosition`,
    /// should be updated when `timeScale` changes
    /// </summary>
    [SerializeField]
    private float updatePositionInterval = 1e-5f / Timeline.timeStep;

    /// <summary>
    /// interpolation value to update view position using `Lerp`,
    /// between 0 and 1
    /// </summary>
    private float lerpCount = 0f;

    public int recordIndex = 0;

    /// <summary>
    /// next position the satellite will lerp to,
    /// updated by `updatePositionAction`
    /// </summary>
    private Vector3 nextPosition;

    UnityAction updateAction;

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

    protected override void Start()
    {
        base.Start();

        // TODO
        if (updateAction == null)
        {
            if (satelliteData != null)
            {
                InitializeDirectMovement();
            }
            else if (orbitRecord != null)
            {
                InitializeRecordMovement();
            }
        }

        if (
            targetStationName != null
            && receiverStationName != null
            && targetStationName != receiverStationName
        )
        {
            updateAction += () => UpdateLinkRoute();
        }

        if (obstacleCollider == null)
        {
            obstacleCollider = targetPlanet.GetComponentInChildren<SphereCollider>();
        }
    }

    void Update()
    {
        updateAction?.Invoke();
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, SatUtils.Vector3(satelliteData.velocity), Color.magenta);
#endif
    }

    /// <summary>
    /// initialize movement based on direct calculation of orbital elements,
    /// `nextPosition` is provided by
    /// </summary>
    public void InitializeDirectMovement()
    {
        transform.position = SatUtils.Vector3(satelliteData.position * SatUtils.EarthScale);
        EventManager.TimeChanged += (time) => satelliteData.UpdateAnomaly(time);
        EventManager.TimeStepChanged += (timeStep) => updatePositionInterval = 1e-5f / timeStep;

        updateAction = () =>
        {
            if (lerpCount < 1.0f)
            {
                lerpCount += Time.deltaTime / updatePositionInterval;
            }
            else
            {
                lerpCount = 0;
                nextPosition = SatUtils.Vector3(satelliteData.position * SatUtils.EarthScale);
            }
            transform.position = Vector3.Lerp(
                transform.position,
                targetPlanet.position + nextPosition,
                lerpCount
            );
        };
    }

    /// <summary>
    /// movement based on discrete data, provided by `SatTask`
    /// </summary>
    public void InitializeRecordMovement()
    {
        transform.position = (Vector3)orbitRecord[0].position * SatUtils.EarthScale;
        float step = (float)(orbitRecord[1].elapsedTime - orbitRecord[0].elapsedTime);
        EventManager.TimeChanged += (time) =>
        {
            int idx = (int)(time / step);
            idx = orbitRecord.FindIndex(idx, (item) => item.elapsedTime >= time);
            recordIndex = idx < 0 ? orbitRecord.Count - 1 : idx;
        };
        EventManager.TimeStepChanged += (timeStep) => updatePositionInterval = step / timeStep;

        updateAction = () =>
        {
            if (lerpCount < 1.0f)
            {
                lerpCount += Time.deltaTime / updatePositionInterval;
            }
            else
            {
                lerpCount = 0;
                nextPosition = (Vector3)orbitRecord[recordIndex].position * SatUtils.EarthScale;
            }
            transform.position = Vector3.Lerp(
                transform.position,
                targetPlanet.position + nextPosition,
                lerpCount
            );
        };
    }

    // TODO movement based on rotation and ecllipse shape
    void InitializeEllipseMovement()
    {
        updateAction = () => { };
    }

    public List<Vector3> GetScaledOrbit()
    {
        if (satelliteData != null)
        {
            return satelliteData.GetScaledOrbit();
        }
        else if (orbitRecord != null)
        {
            return null;
        }
        return null;
    }

    // TODO judging from elevation, distance and obstacle's collider
    public bool PhysicalStationVisibilityChecker(StationLogic station)
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

    public bool PhysicalSatelliteVisibilityChecker(SatelliteLogic satellite)
    {
        var direction = satellite.transform.position - transform.position;
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isVisible = !(raycast && obstacleCollider == hit.collider);
        return isVisible;
    }

    /// <summary>
    /// presume that the station is on surface of the planet
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public bool LogicalStationVisibilityChecker(Vector3 targetPosition)
    {
        Vector3 targetToCenter = targetPlanet.position - targetPosition;
        Vector3 targetToSelf = transform.position - targetPosition;
        var angle = Vector3.Angle(targetToCenter, targetToSelf);

        bool isVisible = angle > 90;
        return isVisible;
    }

    /// <summary>
    /// judging from angle and distance
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public bool LogicalSatelliteVisibilityChecker(Vector3 targetPosition)
    {
        var planetRadius = targetPlanet.gameObject.GetComponent<PlanetLogic>().SphereRadius;

        Vector3 direction = Vector3.Normalize(transform.position - targetPosition);
        float distance = Vector3
            .Cross(direction, targetPlanet.position - transform.position)
            .magnitude;

        Vector3 centerToTarget = targetPosition - targetPlanet.position;
        Vector3 centerToSelf = transform.position - targetPlanet.position;
        var angle = Vector3.Angle(centerToTarget, centerToSelf);

        bool isVisible = distance > planetRadius || (distance < planetRadius && angle < 90);
        return isVisible;
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
                if (sat.LogicalStationVisibilityChecker(dest.transform.position))
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

    /// <summary>
    /// update `linkRoute` every frame,
    /// targetStation -> currentSatellite -> relaySatellites -> destStation
    /// </summary>
    /// <param name="relay">max numbder of relay satellites</param>
    public void UpdateLinkRoute(int relay = 1)
    {
        if (targetStation == null || receiverStation == null || targetStation == receiverStation)
        {
            targetStation = targetPlanet.Find(targetStationName)?.GetComponent<StationLogic>();
            receiverStation = targetPlanet.Find(receiverStationName)?.GetComponent<StationLogic>();
            linkRoute = null;
            return;
        }

        var route = new List<ObjectLogic>();
        if (LogicalStationVisibilityChecker(targetStation.transform.position))
        {
            route.Add(targetStation);
            route.Add(this);
            if (LogicalStationVisibilityChecker(receiverStation.transform.position))
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
                    if (LogicalSatelliteVisibilityChecker(sat.transform.position))
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
