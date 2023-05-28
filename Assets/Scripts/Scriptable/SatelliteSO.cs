using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SatSys;
using System;

[CreateAssetMenu(fileName = "SatelliteSO", menuName = "GeoSys/SatelliteSO")]
public class SatelliteSO : ObjectSO
{
    public SatData.SatelliteData satelliteData;

    public string targetStationName;
    public string receiverStationName;

    public override Transform Spawn(Guid guid)
    {
        var item = base.Spawn(guid);

        var satellite = item.GetComponent<SatelliteLogic>();
        satellite.satelliteData = new SatData.SatelliteData(satelliteData);
        satellite.targetStationName = targetStationName;
        satellite.receiverStationName = receiverStationName;
        return item;
    }
}
