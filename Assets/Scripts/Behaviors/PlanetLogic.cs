using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLogic : MonoBehaviour
{
    [SerializeField] public float selfRotateSpeed = 10f;

    void Update()
    {
        TestRotation();
    }

    void TestRotation()
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * selfRotateSpeed);
    }
}
