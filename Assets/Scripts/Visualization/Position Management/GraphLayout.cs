using UnityEngine;
using System.Collections.Generic;
using System;


//This class stores the current locations for the Nodes during the viz. 
// The positions can be updated elsewhere eg. the force directed script or static positions from the initial data
//  and the visualisers can then read the data from here
public class GraphLayout
{
    private Dictionary<TimeSpan, int> _timeStepLookup = new();
    private GraphData graph;
    
    public Dictionary<(string nodeId, TimeSpan time), Vector3> NodePositions {get; private set;}= new(); //TODO: move to alg which need live updates. 

    private INodeLayoutAlgorithm _layoutAlgorithm;

    public Vector3 GetNodePosition(string nodeId, TimeSpan time)
    {
        return NodePositions[(nodeId, time)];
    }

    public bool IsDynamic()
    {
        return _layoutAlgorithm.IsDynamic;
    }
    public bool IsSimulating()
    {
        return _layoutAlgorithm.IsSimulating;
    }

    public void Initialize(INodeLayoutAlgorithm layoutAlgorithm, GraphData graphData)
    {
        _layoutAlgorithm = layoutAlgorithm;
        this.graph = graphData;
        for (int i = 0; i < graph.TimeSteps.Count; i++)
        {
            _timeStepLookup[graph.TimeSteps[i]] = i;
        }
        NodePositions = layoutAlgorithm.CalculateInitialPositions(graphData);
    }
    

    public void SetAlgorithm(INodeLayoutAlgorithm algorithm)
    {
        _layoutAlgorithm = algorithm;
    }

    public void UpdateLayout()
    {
        _layoutAlgorithm.UpdatePositions(graph, NodePositions);

    }
}
