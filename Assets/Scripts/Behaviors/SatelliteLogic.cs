using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteLogic : ObjectLogic
{
    [SerializeField] private float _rotateSpeed;

    void Start()
    {
        transform.localPosition = new Vector3(0, 0.6f, 0);
    }

    void Update()
    {
        TestRotation();
    }

    void TestRotation()
    {
        transform.RotateAround(_targetPlanet.position, Vector3.back, Time.deltaTime * _rotateSpeed);
    }
}
