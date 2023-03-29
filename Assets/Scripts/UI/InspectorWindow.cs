using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectorWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Transform _inspectorCamera;

    [SerializeField]
    private float _rotateSensitivity = 1f;

    [SerializeField]
    private float _scrollSensitivity = 20f;
    private bool _insideWindow = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        _insideWindow = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _insideWindow = false;
    }

    void Update()
    {
        if (_insideWindow)
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                var x = Input.GetAxis("Mouse X") * _rotateSensitivity;
                var y = Input.GetAxis("Mouse Y") * _rotateSensitivity;
                _inspectorCamera.transform.RotateAround(
                    Vector3.zero,
                    _inspectorCamera.transform.up,
                    x
                );
                _inspectorCamera.transform.RotateAround(
                    Vector3.zero,
                    _inspectorCamera.transform.right,
                    -y
                );
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                var scroll = Input.GetAxis("Mouse ScrollWheel");
                var camera = _inspectorCamera.GetComponent<Camera>();
                camera.fieldOfView -= scroll * _scrollSensitivity;
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 20, 150);
            }
        }
        else if (Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
