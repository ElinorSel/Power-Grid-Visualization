using UnityEngine;
using System.Collections.Generic;
using System;
using System.Numerics;
using Unity.GraphToolkit.Editor;



public class InitalDataLayout : INodeLayoutAlgorithm
{


    //Fills the nodePositions dictionary In graphLayout with the initial data
    //Updates the nodePositions based on the graphData and viz settings
    //NOTE: in the nodeSnapshot class, currently it chooses randomly. TODO: make this conditional only if no data exists.
  public Dictionary<(string, TimeSpan), UnityEngine.Vector3> CalculateInitialPositions(GraphData graph)
    {
        Dictionary<(string, TimeSpan), UnityEngine.Vector3> positions = new();

        for (int currentTimeStep = 0; currentTimeStep < graph.TimeSteps.Count; currentTimeStep++)
        {
            TimeSpan timeStep = graph.TimeSteps[currentTimeStep];
            foreach (Node node in graph.Nodes.Values)
            {
                positions[(node.Id, timeStep)] = CalculatePosition( node.DataSnapshots[timeStep], currentTimeStep);
            }
        }

        return positions;
    }


    public void UpdatePositions(GraphData graph, Dictionary<(string, TimeSpan), UnityEngine.Vector3> positions) //Need a reference to nodes dictionary from graphManager?
    {
        for (int timeStepIndex = 0; timeStepIndex < graph.TimeSteps.Count; timeStepIndex++)
        {
            TimeSpan time = graph.TimeSteps[timeStepIndex];

            foreach (Node node in graph.Nodes.Values)
            {
                NodeSnapshot snapshot = node.DataSnapshots[time];
                UnityEngine.Vector3 pos = positions[(node.Id, time)];
                float graphOffset = VisualizationSettings.Instance.TimeStepZSize * timeStepIndex;
                pos.y = GetNodeHeight(snapshot);
                positions[(node.Id, time)] = pos;
            }
        }
    }


     private UnityEngine.Vector3 CalculatePosition(NodeSnapshot snapshot, int timestep)
    {
        float graphOffset = VisualizationSettings.Instance.TimeStepZSize * timestep;
        float height = graphOffset + GetNodeHeight(snapshot);

        return new UnityEngine.Vector3(
            snapshot.Coordinates.x,
            height,
            snapshot.Coordinates.y
        );
    }


//TODO: calculate nodeheight in a different class to reduce code repitition?
        private float GetNodeHeight(NodeSnapshot nodeSnapshot)
    {
        // [Height Mapping]
        switch (VisualizationSettings.Instance.NodeHeightMapping)
        {
            case VisualizationSettings.NodeHeightMappingOption.None:
                return  1f;
            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                 return CalculateZOffsetVoltageAngle(nodeSnapshot);
            default:
                Debug.LogWarning("Unknown / Unimplementedheight mapping option for Node Height Offset.");
                return 0f;
        }
    }

    
        private float CalculateZOffsetVoltageAngle(NodeSnapshot nodeSnapshot)
    {
        //TODO: Implement a more sophisticated method to calculate the zOffset
        return nodeSnapshot.VAngle * VisualizationSettings.Instance.NodeHeightScaleFactor;
    }
    
}
