using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SatSys;

public class StationLogic : ObjectLogic
{
    public float altitude;
    public float longitude;
    public float latitude;
    public float elevation = 5f;

    public SatelliteLogic[] satellites;

    /// <summary>
    /// a series of switch time between visible and invisible,
    /// start from visible time
    /// </summary>
    public List<double> timeWindows;
    public bool enableCheckingTimeWindows = false;
    private double currentTime;
    private bool wasVisible = false;

    /// <summary>
    /// collider of the central body, defined by `targetPlanet`
    /// </summary>
    public Collider obstacleCollider;

    protected override void Start()
    {
        base.Start();

        transform.SetParent(targetPlanet);
        InitializePosition();

        satellites = FindObjectsOfType<SatelliteLogic>();
        EventManager.TimeChanged += (time) => currentTime = time;
        EventManager.ObjectUpdated += () => satellites = FindObjectsOfType<SatelliteLogic>();

        if (obstacleCollider == null)
        {
            obstacleCollider = targetPlanet.GetComponentInChildren<SphereCollider>();
        }
    }

    private void Update()
    {
        if (enableCheckingTimeWindows)
            UpdateTimeWindows();
    }

    /// <summary>
    /// initialize station's position relative to the parent planet,
    /// using `altitude`, `longitude` and `latitude`
    /// </summary>
    private void InitializePosition()
    {
        var planetRadius = targetPlanet.gameObject.GetComponent<PlanetLogic>().SphereRadius;
        transform.localPosition = new Vector3(altitude * SatUtils.EarthScale + planetRadius, 0, 0);
        transform.RotateAround(Vector3.zero, Vector3.up, -longitude);
        transform.RotateAround(Vector3.zero, transform.forward, latitude);
    }

    /// <summary>
    /// update `timeWindows` by checking visibility every frame
    /// </summary>
    public void UpdateTimeWindows()
    {
        bool isVisible = false;
        foreach (var sat in satellites)
        {
            isVisible = LogicalSatelliteVisibilityChecker(sat);
        }

        if (isVisible ^ wasVisible)
        {
            timeWindows.Add(currentTime);
        }
        wasVisible = isVisible;
    }

    /// <summary>
    /// check visibility of a satellite from the station, judging from obstacle's collider
    /// </summary>
    /// <param name="satellite">target satellite</param>
    /// <returns></returns>
    public bool PhysicalSatelliteVisibilityChecker(SatelliteLogic satellite)
    {
        var direction = satellite.transform.position - transform.position;
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isVisible = !(raycast && obstacleCollider == hit.collider);
        return isVisible;
    }

    // TODO
    public bool LogicalSatelliteVisibilityChecker(SatelliteLogic satellite)
    {
        return satellite.LogicalStationVisibilityChecker(transform.position);
    }

    /// <summary>
    /// return a list of satellites that can be seen from the station
    /// </summary>
    /// <returns>visible satellites</returns>
    public List<SatelliteLogic> GetVisibleSatellites()
    {
        var visibleSatellites = new List<SatelliteLogic>();
        foreach (var sat in satellites)
        {
            if (LogicalSatelliteVisibilityChecker(sat))
            {
                visibleSatellites.Add(sat);
            }
        }
        return visibleSatellites;
    }
}
