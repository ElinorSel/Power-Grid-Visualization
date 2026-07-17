using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Unity.GraphToolkit.Editor;
using UnityEngine.PlayerLoop;
public class IForceDirectedLayout
{
    
    //Fruchterman-Reingold force-directed layout
    Dictionary<(string nodeId, TimeSpan time), Vector3> _displacement;
    private Vector3 delta;
    private float distance;
    private Vector3 direction;

    private Vector3 force;

    private float temperature = 1f;
    private float displacementMagnitude = 1f; //TODO: is chosen by user later?

    private int idealNodeSpacing = 2;

    GraphData _graph;


    void CalculateInitialPositions()
    {
        //read the initial positions of the data
        
    }
    void UpdatePositions(GraphData graph, Dictionary<(string nodeId, TimeSpan time), Vector3> nodePositions)
    {
        _graph = graph;
        foreach(KeyValuePair<(string, TimeSpan), Vector3> nodeU in nodePositions)
        {
            foreach(KeyValuePair<(string, TimeSpan), Vector3> nodeV in nodePositions)
            {
                ComputeRepulse(nodeU.Key, nodeU.Value, nodeV.Key, nodeV.Value);
            }
        }

        ComputeAttraction(graph, nodePositions);

        MoveNodes(nodePositions);
        
    }

    void ComputeRepulse((string, TimeSpan) nodeU, Vector3 nodeUPos,(string, TimeSpan) nodeV, Vector3 nodeVPos)
    {
        delta = nodeUPos - nodeVPos;
        distance = delta.magnitude;
        direction = delta/distance;

        force = direction * (idealNodeSpacing * idealNodeSpacing /distance); //Fr(d) = k^2/d the repulsive force in Fruchterman-Reingold.

        _displacement[nodeU] += force;
        _displacement[nodeV] -= force;
    }

    void ComputeAttraction(GraphData graph,Dictionary<(string nodeId, TimeSpan time), Vector3> nodePositions)
    {
        foreach(var edge in graph.Edges.Values)
        {
            foreach( TimeSpan time in graph.TimeSteps)
            {
                Vector3 posU = nodePositions[(edge.Node1.Id, time)];
                Vector3 posV = nodePositions[(edge.Node2.Id, time)];
                delta = posU - posV;
                distance = delta.magnitude;
                direction = delta/distance; 
            }
        }

        
    }

    void MoveNodes(Dictionary<(string nodeId, TimeSpan time), Vector3> nodePositions)
    {
        foreach(var node in nodePositions.Keys)
        {
            Vector3 movement = _displacement[node].Normalize();
            movement *= Mathf.Min(displacementMagnitude, temperature);
            temperature -= 0.01f;
            nodePositions[node] += movement;
        }
    }
}
