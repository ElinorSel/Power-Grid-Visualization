using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Android.Gradle.Manifest;


//This class stores the current locations for the Nodes during the viz. 
// The positions can be updated elsewhere eg. the force directed script or static positions from the initial data
//  and the visualisers can then read the data from here
public class GraphLayout
{
    private Dictionary<TimeSpan, int> timeStepLookup = new();
    private GraphData graph;
    public Dictionary<(string nodeId, TimeSpan time), Vector3> NodePositions {get; private set;}= new(); //TODO: move to alg which need live updates. 

    private INodeLayoutAlgorithm layoutAlgorithm;

    public Vector3 GetNodePosition(string nodeId, TimeSpan time)
    {
        return NodePositions[(nodeId, time)];
    }

    public void Initialize(INodeLayoutAlgorithm layoutAlgorithm, GraphData graphData)
    {
        this.layoutAlgorithm = layoutAlgorithm;
        this.graph = graphData;
        for (int i = 0; i < graph.TimeSteps.Count; i++)
        {
            timeStepLookup[graph.TimeSteps[i]] = i;
        }
        NodePositions = layoutAlgorithm.CalculateInitialPositions(graphData);
    }
    

    public void SetAlgorithm(INodeLayoutAlgorithm algorithm)
    {
        layoutAlgorithm = algorithm;
    }

    public void UpdateLayout()
    {
        layoutAlgorithm.UpdatePositions(graph, NodePositions);
    }
}
