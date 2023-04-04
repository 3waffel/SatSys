using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetLogic : MonoBehaviour
{
    /// <summary>
    /// Rotation rounds in one day (Julian Day)
    /// </summary>
    public float selfRotationSpeed = 1;

    private Material lineMaterial;

    public Transform sphere;

    public float SphereRadius
    {
        get =>
            sphere.GetComponent<SphereCollider>().radius
            * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
    }

    void Start()
    {
        InitializeRotation();
        EventManager.TimeChanged += UpdateEarthRotation;
    }

    /// <summary>
    /// Make sure the coordinate is correct
    /// </summary>
    void InitializeRotation()
    {
        sphere.RotateAround(Vector3.zero, Vector3.up, -130);
    }

    void UpdateEarthRotation(double epoch)
    {
        float angle = (float)(epoch * 360 * selfRotationSpeed);
        transform.rotation = Quaternion.Euler(0, (float)angle, 0);
    }

    void OnRenderObject()
    {
        DrawAxis();
    }

    void CreateLineMaterial()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    void DrawAxis()
    {
        if (lineMaterial == null)
        {
            CreateLineMaterial();
        }
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);

        //Draw X axis
        GL.Color(Color.red);
        GL.Vertex3(-10f, 0, 0);
        GL.Vertex3(10.0f, 0.0f, 0.0f);
        //Draw Y axis
        GL.Color(Color.green);
        GL.Vertex3(0, -10f, 0);
        GL.Vertex3(0.0f, 10.0f, 0.0f);
        //Draw Z axis
        GL.Color(Color.blue);
        GL.Vertex3(0, 0, -10f);
        GL.Vertex3(0.0f, 0.0f, 10.0f);

        GL.End();
        GL.PopMatrix();
    }
}
