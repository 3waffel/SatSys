using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectorWindow
    : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    [SerializeField]
    private Transform _inspectorCamera;

    [SerializeField]
    private float _rotationSensitivity = 1f;

    [SerializeField]
    private float _scrollSensitivity = 20f;

    private bool _isPointerInside = false;
    private bool _isPointerDown = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    void Update()
    {
        if (_isPointerInside)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                var scroll = Input.GetAxis("Mouse ScrollWheel");
                var camera = _inspectorCamera.GetComponent<Camera>();
                camera.fieldOfView -= scroll * _scrollSensitivity;
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 20, 150);
            }
        }

        if (_isPointerDown)
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                var x = Input.GetAxis("Mouse X") * _rotationSensitivity;
                var y = Input.GetAxis("Mouse Y") * _rotationSensitivity;
                _inspectorCamera.transform.RotateAround(
                    Vector3.zero,
                    Vector3.up,
                    x
                );
                _inspectorCamera.transform.RotateAround(
                    Vector3.zero,
                    _inspectorCamera.transform.right,
                    -y
                );
            }
        }
        else if (Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
