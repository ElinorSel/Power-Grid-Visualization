using System;
using System.Collections.Generic;
using UnityEngine;

public interface INodeLayoutAlgorithm
{
    // Blueprint for the algorithms which will controll how the nodes will be positioned in the world
    //Vector3 GetNodePosition(Node node, TimeSpan time, int timeStepIndex);
    Dictionary<(string, TimeSpan), UnityEngine.Vector3> CalculateInitialPositions(GraphData graph);
    void UpdatePositions(GraphData graph, Dictionary<(string nodeId, TimeSpan time), Vector3> positions);

}
