using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLogic : ObjectLogic
{
    public float altitude;
    public float longitude;
    public float latitude;

    public SatelliteLogic[] satellites;

    protected override void Start()
    {
        base.Start();

        transform.SetParent(targetPlanet);
        UpdatePosition();

        satellites = FindObjectsOfType<SatelliteLogic>();
    }

    private void Update()
    {
        bool isConnected = false;
        foreach (var sat in satellites)
        {
            var direction = sat.transform.position - transform.position;
            var raycast = Physics.Raycast(transform.position, direction, out RaycastHit hit);
            if (raycast && sat.gameObject == hit.collider.gameObject)
            {
                Debug.DrawRay(transform.position, direction, Color.red);
                isConnected = true;
            }
            else
            {
                Debug.DrawRay(transform.position, direction, Color.green);
            }
        }
    }

    /// <summary>
    /// Position relative to the parent planet
    /// </summary>
    private void UpdatePosition()
    {
        var basis = targetPlanet.gameObject.GetComponent<PlanetLogic>().SphereRadius;
        transform.localPosition = new Vector3(altitude + basis, 0, 0);
        transform.RotateAround(Vector3.zero, Vector3.up, longitude);
        transform.RotateAround(Vector3.zero, transform.forward, latitude);
    }
}
