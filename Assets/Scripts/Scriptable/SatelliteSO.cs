using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SatSys;
using System;

[CreateAssetMenu(fileName = "SatelliteSO", menuName = "GeoSys/SatelliteSO")]
public class SatelliteSO : ObjectSO
{
    public SatData.SatelliteData satelliteData;
    public float updatePositionInterval = 0.1f;

    public override Transform Spawn(Guid guid)
    {
        var item = base.Spawn(guid);

        var satellite = item.GetComponent<SatelliteLogic>();
        satellite.satelliteData = satelliteData;
        satellite.updatePositionInterval = updatePositionInterval;
        return item;
    }
}
