using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StationSO", menuName = "GeoSys/StationSO")]
public class StationSO : ObjectSO
{
    public float longitude;
    public float latitude;
    public float altitude;
    public float elevation = 5f;

    public override Transform Spawn(Guid guid)
    {
        var item = base.Spawn(guid);

        var station = item.GetComponent<StationLogic>();
        station.longitude = longitude;
        station.latitude = latitude;
        station.altitude = altitude;
        station.elevation = elevation;
        return item;
    }
}
