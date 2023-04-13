using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Update()
    {
        bool isVisible = false;
        foreach (var sat in satellites)
        {
            var direction = sat.transform.position - transform.position;
            var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);
            if (raycast && sat.gameObject == hit.collider.gameObject)
            {
                Debug.DrawRay(transform.position, direction, Color.red);
                isVisible = true;
            }
            else
            {
                Debug.DrawRay(transform.position, direction, Color.green);
            }
        }

        if (isVisible ^ wasVisible)
        {
            timeWindows.Add(currentTime);
        }
        wasVisible = isVisible;
    }

    /// <summary>
    /// Position relative to the parent planet
    /// </summary>
    private void InitializePosition()
    {
        var planetRadius = targetPlanet.gameObject.GetComponent<PlanetLogic>().SphereRadius;
        transform.localPosition = new Vector3(altitude + planetRadius, 0, 0);
        transform.RotateAround(Vector3.zero, Vector3.up, longitude);
        transform.RotateAround(Vector3.zero, transform.forward, latitude);
    }
}
