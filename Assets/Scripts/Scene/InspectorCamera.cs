using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _targetInspector;
    private RenderTexture _renderTexture;
    private Camera _camera;

    void Start()
    {
        _camera = this.GetComponent<Camera>();
    }

    void Update() { }
}
