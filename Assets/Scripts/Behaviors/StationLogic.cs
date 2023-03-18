using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLogic : ObjectLogic
{
    void Start()
    {
        transform.SetParent(_targetPlanet);
        var position = new Vector3(0, 0.2f, 0.4f);
        var rotation = new Quaternion();

        transform.SetLocalPositionAndRotation(position, rotation);
    }
}
