using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Transform _targetPlanet;

    void Start()
    {
        // this.transform.SetParent(_targetPlanet);
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
