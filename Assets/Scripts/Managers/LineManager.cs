using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SatSys.SatData;
using static SatSys.SatUtils;
using QuikGraph;
using System;
using System.Linq;

public class LineManager : MonoBehaviour
{
    public Material lineMaterial;

    public Color orbitColor = Color.cyan;
    public Color satLinkColor = Color.yellow;

    // holder for different lines
    public Transform orbits;
    public Transform links;
    public Transform routes;

    /// <summary>
    /// record of visible connections among satellites
    /// </summary>
    private AdjacencyGraph<SatelliteLogic, Edge<SatelliteLogic>> satGraph =
        new AdjacencyGraph<SatelliteLogic, Edge<SatelliteLogic>>();

    public static LineManager LM => FindObjectOfType<LineManager>();

    void Awake()
    {
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Unlit/Color"));
        }

        if (orbits == null)
        {
            var go = new GameObject("Orbits");
            go.transform.SetParent(transform);
            orbits = go.transform;
        }
        if (links == null)
        {
            var go = new GameObject("Links");
            go.transform.SetParent(transform);
            links = go.transform;
        }
        if (routes == null)
        {
            var go = new GameObject("Routes");
            go.transform.SetParent(transform);
            routes = go.transform;
        }
    }

    void Start()
    {
        EventManager.ObjectUpdated += UpdateGraph;
        EventManager.ObjectUpdated += UpdateOrbits;
    }

    void Update()
    {
        if (satGraph.IsVerticesEmpty)
            return;

        UpdateGraphByVisibility();
        UpdateSatLinksByGraph();
        UpdateStationRoutes();
    }

    public void CreateLine(
        Vector3[] points,
        Transform parent,
        Material lineMaterial,
        Color startColor = default,
        Color endColor = default,
        float startWidth = 0.005f,
        float endWidth = 0.005f
    )
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(parent);
        go.AddComponent<LineRenderer>();

        var lr = go.GetComponent<LineRenderer>();
        lr.positionCount = points.Length;
        lr.SetPositions(points);
        lr.material = lineMaterial;
        lr.startColor = startColor;
        lr.endColor = endColor;
        lr.startWidth = startWidth;
        lr.endWidth = endWidth;
    }

    /// <summary>
    /// helper method to create a straight line
    /// </summary>
    public void CreateSimpleLine(
        Vector3 from,
        Vector3 to,
        Transform parent,
        Material lineMaterial,
        Color lineColor,
        float lineWidth = 0.005f
    ) =>
        CreateLine(
            new Vector3[] { from, to },
            parent,
            lineMaterial,
            lineColor,
            lineColor,
            lineWidth,
            lineWidth
        );

    /// <summary>
    /// create or update orbits when satellites change
    /// </summary>
    public void UpdateOrbits()
    {
        if (orbits == null)
            return;
        foreach (Transform orbit in orbits)
        {
            GameObject.Destroy(orbit.gameObject);
        }

        foreach (var logic in satGraph.Vertices)
        {
            CreateOrbitFromData(logic.satelliteData, orbits);
        }
    }

    /// <summary>
    /// create orbit from satellite data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="parent">holder of the orbit</param>
    public void CreateOrbitFromData(SatelliteData data, Transform parent)
    {
        var positions = data.GetScaledOrbit();
        CreateLine(positions.ToArray(), parent, lineMaterial, orbitColor, orbitColor);
    }

    /// <summary>
    /// create or update vertices in the graph when satellites change
    /// </summary>
    public void UpdateGraph()
    {
        if (!satGraph.IsVerticesEmpty)
            satGraph.Clear();

        var logics = FindObjectsOfType<SatelliteLogic>();
        satGraph.AddVertexRange(logics);
    }

    /// <summary>
    /// update edges of the graph based on visibility between vertices,
    /// executed every frame
    /// </summary>
    public void UpdateGraphByVisibility()
    {
        if (satGraph.IsVerticesEmpty)
            return;

        var vertices = satGraph.Vertices.ToArray();
        foreach (var vertex in vertices)
        {
            satGraph.ClearEdges(vertex);
        }

        for (int i = 0; i < satGraph.VertexCount - 1; i++)
        {
            for (int j = i + 1; j < satGraph.VertexCount; j++)
            {
                if (vertices[i] != vertices[j])
                {
                    var edge = new Edge<SatelliteLogic>(vertices[i], vertices[j]);
                    bool isVisible = vertices[i].CheckSatelliteVisibility(vertices[j]);

                    if (isVisible && !satGraph.ContainsEdge(edge))
                        satGraph.AddEdge(edge);
                }
            }
        }
    }

    /// <summary>
    /// destroy previous links and create new links,
    /// executed every frame
    /// </summary>
    public void UpdateSatLinksByGraph()
    {
        if (links == null)
            return;
        foreach (Transform link in links)
        {
            GameObject.Destroy(link.gameObject);
        }

        if (satGraph.IsEdgesEmpty)
            return;
        foreach (var edge in satGraph.Edges)
        {
            var from = edge.Source.transform.position;
            var to = edge.Target.transform.position;
            CreateSimpleLine(from, to, links, lineMaterial, satLinkColor);
        }
    }

    public void UpdateStationRoutes()
    {
        if (routes == null)
            return;
        foreach (Transform route in routes)
        {
            GameObject.Destroy(route.gameObject);
        }

        if (satGraph.IsVerticesEmpty)
            return;

        var vertices = satGraph.Vertices.ToArray();
        foreach (var vertex in vertices)
        {
            if (vertex.linkRoute != null)
            {
                var points = new List<Vector3>();
                vertex.linkRoute.ForEach((logic) => points.Add(logic.transform.position));
                CreateLine(
                    points.ToArray(),
                    routes,
                    lineMaterial,
                    Color.red,
                    Color.green,
                    0.05f,
                    0.1f
                );
            }
        }
    }
}
