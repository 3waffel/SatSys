using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMovement : MonoBehaviour
{
    public float selfRotateSpeed = 10f;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * selfRotateSpeed);
    }
}
