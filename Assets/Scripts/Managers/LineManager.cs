using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SatSys.SatData;
using static SatSys.SatUtils;

public static class LineManager
{
    public static Material lineMaterial = new Material(Shader.Find("Unlit/Color"));

    public static Material orbitMaterial
    {
        get
        {
            var m = new Material(lineMaterial);
            m.color = Color.cyan;
            return m;
        }
    }

    public class LineController : MonoBehaviour
    {
        public LineRenderer lr;
        public Vector3[] points;

        void Awake()
        {
            lr = gameObject.GetComponent<LineRenderer>();
        }

        void Update()
        {
            for (int i = 0; i < points.Length; i++)
            {
                lr.SetPosition(i, points[i]);
            }
        }
    }

    public static void CreateLine(Vector3[] points, Transform parent)
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(parent);
        go.AddComponent<LineRenderer>();
        go.AddComponent<LineController>();

        var lc = go.GetComponent<LineController>();
        lc.lr.positionCount = points.Length;
        lc.lr.startWidth = 0.005f;
        lc.lr.material = orbitMaterial;
        lc.points = points;
        lc.transform.SetParent(parent);
    }

    public static void UpdateOrbitRenderer(
        SatelliteData data,
        Transform parent,
        double timeStep = 0.001
    )
    {
        var lc = parent.GetComponentInChildren<LineController>();
        if (lc == null)
        {
            var positions = data.GetOrbit(timeStep);
            CreateLine(positions.ToArray(), parent);
        }
        else
        {
            lc.points = data.GetOrbit(timeStep).ToArray();
        }
    }
}
