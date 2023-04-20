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

    public static Material sharedOrbitMaterial;
    public static Material sharedSatLinkMaterial;

    public Transform orbits;
    public Transform links;

    public AdjacencyGraph<SatelliteLogic, Edge<SatelliteLogic>> satGraph =
        new AdjacencyGraph<SatelliteLogic, Edge<SatelliteLogic>>();

    void Awake()
    {
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Unlit/Color"));
        }
        if (sharedOrbitMaterial == null)
        {
            sharedOrbitMaterial = new Material(lineMaterial);
            sharedOrbitMaterial.color = Color.cyan;
        }
        if (sharedSatLinkMaterial == null)
        {
            sharedSatLinkMaterial = new Material(lineMaterial);
            sharedSatLinkMaterial.color = Color.yellow;
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

        UpdateEdgesByVisibility();
        UpdateSatLinksByEdges();
    }

    public void CreateLine(Vector3[] points, Transform parent, Material lineMaterial)
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(parent);
        go.AddComponent<LineRenderer>();

        var lr = go.GetComponent<LineRenderer>();
        lr.positionCount = points.Length;
        lr.startWidth = 0.005f;
        lr.material = lineMaterial;
        lr.SetPositions(points);
    }

    public void CreateDirectLine(
        Vector3 from,
        Vector3 to,
        Transform parent,
        Material lineMaterial
    ) => CreateLine(new Vector3[] { from, to }, parent, lineMaterial);

    public void UpdateOrbits()
    {
        foreach (Transform orbit in orbits)
        {
            GameObject.Destroy(orbit.gameObject);
        }

        foreach (var logic in satGraph.Vertices)
        {
            UpdateOrbitRenderer(logic.satelliteData, orbits);
        }
    }

    public void UpdateOrbitRenderer(SatelliteData data, Transform parent)
    {
        var positions = data.GetScaledOrbit();
        CreateLine(positions.ToArray(), parent, sharedOrbitMaterial);
    }

    public void UpdateGraph()
    {
        if (!satGraph.IsVerticesEmpty)
            satGraph.Clear();

        var logics = FindObjectsOfType<SatelliteLogic>();
        satGraph.AddVertexRange(logics);
    }

    public void UpdateEdgesByVisibility()
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
                    var direction = vertices[j].transform.position - vertices[i].transform.position;
                    var raycast = Physics.Raycast(
                        vertices[i].transform.position,
                        direction,
                        out RaycastHit hit
                    );
                    if (raycast && vertices[j].gameObject == hit.collider.gameObject)
                    {
                        if (!satGraph.Edges.Contains(edge))
                            satGraph.AddEdge(edge);
                    }
                }
            }
        }
    }

    public void UpdateSatLinksByEdges()
    {
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
            CreateDirectLine(from, to, links, sharedSatLinkMaterial);
        }
    }
}
