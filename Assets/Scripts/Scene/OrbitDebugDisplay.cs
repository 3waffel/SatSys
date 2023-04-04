using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitDebugDisplay : MonoBehaviour
{
    public int numSteps = 1000;
    public float timeStep = 0.1f;

    public float width = 100;

    // Update is called once per frame
    void Update()
    {
        DrawOrbits();
    }

    void DrawOrbits()
    {
        SatelliteLogic[] bodies = FindObjectsOfType<SatelliteLogic>();
        var virtualBodies = new VirtualBody[bodies.Length];
        var drawPoints = new Vector3[bodies.Length][];
        int referenceFrameIndex = 0;
        Vector3 referenceBodyInitialPosition = Vector3.zero;

        for (int i = 0; i < virtualBodies.Length; i++) {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[numSteps];
        }

        for (int step = 0; step < numSteps; step++)
        {
            Vector3 referenceBodyPosition = Vector3.zero;
            for (int i = 0; i < virtualBodies.Length; i++)
            {
            }
        }

        LineRenderer renderer = new LineRenderer();

    }

    class VirtualBody
    {
        public Vector3 position;
        public Vector3 velocity;

        public VirtualBody(SatelliteLogic satellite)
        {
            position = SatelliteUtils.D2F(satellite.satelliteData.position);
            velocity = SatelliteUtils.D2F(satellite.satelliteData.velocity);
        }
    }
}
