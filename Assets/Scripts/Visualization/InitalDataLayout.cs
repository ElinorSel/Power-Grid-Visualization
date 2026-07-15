using UnityEngine;
using System.Collections.Generic;
using System;
using System.Numerics;



public class InitalDataLayout : INodeLayoutAlgorithm
{

    //This class places the nodes at the positions of the inital cordinates given in the dataset
    //if no inital data is provided, the positions will be randomized. 




    //Fills the nodePositions dictionary In graphLayout with the initial data
    //NOTE: in the nodeSnapshot class, currently it chooses randomly. TODO: make this conditional only if no data exists.
  public Dictionary<(string, TimeSpan), UnityEngine.Vector3> CalculateInitialPositions(GraphData graph)
    {
        Dictionary<(string, TimeSpan), UnityEngine.Vector3> positions = new();

        for (int currentTimeStep = 0; currentTimeStep < graph.TimeSteps.Count; currentTimeStep++)
        {
            TimeSpan timeStep = graph.TimeSteps[currentTimeStep];
            foreach (Node node in graph.Nodes.Values)
            {
                UnityEngine.Vector3 position = CalculatePosition( node.DataSnapshots[timeStep], currentTimeStep);
            }
        }

        return positions;
    }

    public Dictionary<(string, TimeSpan), UnityEngine.Vector3> UpdatePositions()
    {
        Dictionary<(string, TimeSpan), UnityEngine.Vector3> newPositions = new();
        //TODO: only update the height (this only changes based on settings)
        return newPositions;
    }


     private UnityEngine.Vector3 CalculatePosition(NodeSnapshot snapshot, int timestep)
    {
        float height = GetNodeHeight(snapshot, timestep);

        return new UnityEngine.Vector3(
            snapshot.Coordinates.x,
            height,
            snapshot.Coordinates.y
        );
    }


//TODO: calculate nodeheight in a different class to reduce code repitition?
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
