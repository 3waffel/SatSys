using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FacingCamera : MonoBehaviour
{
    public Transform targetCamera;
    public Transform holder;

    void Start()
    {
        targetCamera = FindObjectOfType<Camera>().transform;
    }

    void Update()
    {
        if (holder != null)
            transform.position = holder.position + Vector3.up * 0.1f;
    }

    void LateUpdate()
    {
        transform.LookAt(
            transform.position + targetCamera.rotation * Vector3.forward,
            targetCamera.transform.rotation * Vector3.up
        );
    }
}
