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

    public void UpdateHeightPositions(IReadOnlyList<TimeSpan> visibleTimeSteps = null)
    {
        if (_graph == null || _graph.TimeSteps == null || _graph.TimeSteps.Count == 0)
            return;

        List<TimeSpan> timesToProcess = new List<TimeSpan>();

        if (visibleTimeSteps != null && visibleTimeSteps.Count > 0)
        {
            timesToProcess.AddRange(visibleTimeSteps);
        }
        else
        {
            timesToProcess.AddRange(_graph.TimeSteps);
        }

        // Slider (TimeStepZSize) sets the total vertical span.
        // Layers use flexbox-style space-between: first at floor, last at ceiling.
        float floor = 0f;
        float ceiling = VisualizationSettings.Instance.TimeStepZSize;
        float availableSpace = ceiling - floor;
        int layerCount = timesToProcess.Count;

        for (int timestep = 0; timestep < layerCount; timestep++)
        {
            TimeSpan time = timesToProcess[timestep];

            float baseHeight = (layerCount == 1)
                ? floor
                : floor + availableSpace * timestep / (layerCount - 1);

            foreach (Node node in _graph.Nodes.Values)
            {
                NodeSnapshot snapshot = node.DataSnapshots[time];

                float height = baseHeight +
                    GetNodeHeight(snapshot) *
                    VisualizationSettings.Instance.NodeHeightScaleFactor;

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
