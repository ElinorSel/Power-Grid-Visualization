using UnityEngine;
using System.Collections.Generic;
using System;


//This class stores the current locations for the Nodes during the viz. 
// The positions can be updated elsewhere eg. the force directed script or static positions from the initial data
//  and the visualisers can then read the data from here
public class GraphLayout
{
    public Dictionary<(string nodeId, TimeSpan time), Vector3> NodePositions = new();

    //TODO: get node position based on the values stored here not the nodesnapshot.
    public Vector3 GetNodePosition(string nodeId, TimeSpan time)
    {
        //This method will eventually get positions based on an algoritm for force directions.
        return NodePositions[(nodeId, time)];
    }

    
    public Vector3 GetInitialNodePosition(NodeSnapshot nodeSnapshot, int timeStepIndex)
    {
        //This method gets the position of the given node at a timestep, based on the start coordinates given by the data
        float height = GetNodeHeight(nodeSnapshot, timeStepIndex);
        return new Vector3( nodeSnapshot.Coordinates.x, height ,nodeSnapshot.Coordinates.y);

    }

    private float GetNodeHeight(NodeSnapshot nodeSnapshot, int timeStepIndex)
    {
        // [Height Mapping]
        switch (VisualizationSettings.Instance.NodeHeightMapping)
        {
            case VisualizationSettings.NodeHeightMappingOption.None:
                return  VisualizationSettings.Instance.TimeStepZSize * timeStepIndex;
            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                 return CalculateZOffsetVoltageAngle(nodeSnapshot) +  VisualizationSettings.Instance.TimeStepZSize  * timeStepIndex;
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
