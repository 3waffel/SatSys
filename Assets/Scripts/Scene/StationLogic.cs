using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLogic : ObjectLogic
{
    public float altitude;
    public float longitude;
    public float latitude;

    protected override void Start()
    {
        base.Start();

        transform.SetParent(targetPlanet);
        altitude = targetPlanet.gameObject.GetComponent<PlanetLogic>().SphereRadius;
        UpdatePosition();
    }

    /// <summary>
    /// Position relative to the parent planet
    /// </summary>
    private void UpdatePosition()
    {
        transform.localPosition = new Vector3(altitude, 0, 0);
        transform.RotateAround(Vector3.zero, Vector3.up, longitude);
        transform.RotateAround(Vector3.zero, transform.forward, latitude);
    }
}
