using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StationSO", menuName = "GeoSys/StationSO")]
public class StationSO : ObjectSO
{
    public double longitude;
    public double latitude;
    public double altitude;

    public override Transform Spawn(Guid guid)
    {
        var item = base.Spawn(guid);

        var station = item.GetComponent<StationLogic>();
        station.longitude = (float)longitude;
        station.latitude = (float)latitude;
        return item;
    }
}
