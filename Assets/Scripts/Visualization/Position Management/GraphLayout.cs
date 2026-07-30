using UnityEngine;
using System.Collections.Generic;
using System;


//This class stores the current locations for the Nodes during the viz. 
// The positions can be updated elsewhere eg. the force directed script or static positions from the initial data
//  and the visualisers can then read the data from here
public class GraphLayout
{
    private Dictionary<TimeSpan, int> _timeStepLookup = new();
    private GraphData _graph;
    
    public Dictionary<(string nodeId, TimeSpan time), Vector3> NodePositions {get; private set;}= new(); //TODO: move to alg which need live updates. 

    private Dictionary<TimeSpan, float> _timeStepHeights;   //Store the base heights for each time step.
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
        this._graph = graphData;
        for (int i = 0; i < _graph.TimeSteps.Count; i++)
        {
            _timeStepLookup[_graph.TimeSteps[i]] = i;
        }
        NodePositions = layoutAlgorithm.CalculateInitialPositions(graphData); //only updates x and y
        UpdateHeightPositions(); //updates height
    }
    

    public void SetAlgorithm(INodeLayoutAlgorithm algorithm)
    {
        _layoutAlgorithm = algorithm;
    }

    public void UpdateLayout()
    {
        _layoutAlgorithm.UpdatePositions(_graph, NodePositions); //only updates x and y
        UpdateHeightPositions(); //updates hight 

    }

//____________________________Height calculations______________-

    public void UpdateHeightPositions()
    {
        float ceiling = 30;
        float floor = 0;

        float availibleSpace = ceiling - floor;

        float spacePerTimestep = availibleSpace / _graph.TimeSteps.Count;

        for (int timestep = 0; timestep < _graph.TimeSteps.Count; timestep++)
        {
            TimeSpan time = _graph.TimeSteps[timestep]; //get the timespan related to that timestep index

            foreach (Node node in _graph.Nodes.Values)
            {
                NodeSnapshot snapshot = node.DataSnapshots[time];

                float equalSpacing = spacePerTimestep * timestep; //decides that each timestep should be placed depending on a max distance
                float overrideEqualSpacing = VisualizationSettings.Instance.TimeStepZSize; //* timestep; //TODO: keep? overrides the automatic spacing
                float height = overrideEqualSpacing * (equalSpacing +
                               GetNodeHeight(snapshot) *
                               VisualizationSettings.Instance.NodeHeightScaleFactor);
                
                UnityEngine.Vector3 pos = NodePositions[(node.Id, time)];
                pos.y = height;

                NodePositions[(node.Id, time)] = pos;
            }
        }
        
    }

    
    private float GetNodeHeight(NodeSnapshot nodeSnapshot)
    {
        switch (VisualizationSettings.Instance.NodeHeightMapping)
        {
            case VisualizationSettings.NodeHeightMappingOption.None:
                return 1f;

            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                return CalculateZOffsetVoltageAngle(nodeSnapshot);

            default:
                return 0f;
        }
    }

    private float CalculateZOffsetVoltageAngle(NodeSnapshot nodeSnapshot)
    {
        return nodeSnapshot.VAngle;
    }
}
