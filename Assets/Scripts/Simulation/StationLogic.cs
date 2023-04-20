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

    public List<double> timeWindows;
    private double currentTime;
    private bool wasVisible = false;
    public Collider obstacle;

    protected override void Start()
    {
        base.Start();

        transform.SetParent(targetPlanet);
        InitializePosition();

        satellites = FindObjectsOfType<SatelliteLogic>();
        EventManager.TimeChanged += delegate(double time)
        {
            currentTime = time;
        };
        if (obstacle == null)
        {
            obstacle = targetPlanet.Find("Sphere").GetComponent<SphereCollider>();
        }
    }

    private void Update()
    {
        UpdateTimeWindows();
    }

    /// <summary>
    /// Position relative to the parent planet
    /// </summary>
    private void InitializePosition()
    {
        var planetRadius = targetPlanet.gameObject.GetComponent<PlanetLogic>().SphereRadius;
        transform.localPosition = new Vector3(altitude * SatUtils.Scale + planetRadius, 0, 0);
        transform.RotateAround(Vector3.zero, Vector3.up, longitude);
        transform.RotateAround(Vector3.zero, transform.forward, latitude);
    }

    public void UpdateTimeWindows()
    {
        bool isVisible = false;
        foreach (var sat in satellites)
        {
            isVisible = CheckSatelliteVisibility(sat);
        }

        if (isVisible ^ wasVisible)
        {
            timeWindows.Add(currentTime);
        }
        wasVisible = isVisible;
    }

    public bool CheckSatelliteVisibility(SatelliteLogic satellite)
    {
        var direction = satellite.transform.position - transform.position;
        var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);

        bool isVisible = !(raycast && obstacle == hit.collider);
        if (isVisible)
            Debug.DrawRay(transform.position, direction, Color.red);
        return isVisible;
    }
}
