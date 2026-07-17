using UnityEngine;
using System;
using System.Collections.Generic;

public class ForceDirectedLayout : INodeLayoutAlgorithm
{
    public bool IsDynamic => true;
    public bool IsSimulating => _temperature < 0.01f;
    private readonly Dictionary<string, Vector3> _displacements = new();

    private const float IdealSpacing = 3f;
    private float _temperature = 1f;

   

    public Dictionary<(string, TimeSpan), Vector3> CalculateInitialPositions(GraphData graph)
    {
        // Start from the initial data positions.
        Dictionary<(string, TimeSpan), Vector3> positions = new();

        for (int timestep = 0; timestep < graph.TimeSteps.Count; timestep++)
        {
            TimeSpan time = graph.TimeSteps[timestep];

            foreach (Node node in graph.Nodes.Values)
            {
                NodeSnapshot snapshot = node.DataSnapshots[time];

                float graphOffset = VisualizationSettings.Instance.TimeStepZSize * timestep;
                float height = graphOffset +
                               GetNodeHeight(snapshot) *
                               VisualizationSettings.Instance.NodeHeightScaleFactor;

                positions[(node.Id, time)] =
                    new Vector3(
                        snapshot.Coordinates.x,
                        height,
                        snapshot.Coordinates.y);
            }
        }

        return positions;
    }

    public void UpdatePositions(
        GraphData graph,
        Dictionary<(string, TimeSpan), Vector3> positions)
    {
        if (graph.TimeSteps.Count == 0)
            return;

        // Force layout only calculates topology in X/Z.
        // All timesteps share topology positions.
        // Y is calculated separately from timestep + height mapping.
        TimeSpan baseTime = graph.TimeSteps[0]; 

        //----------------------------------------
        // Reset displacements
        //----------------------------------------

        _displacements.Clear();

        foreach (Node node in graph.Nodes.Values)
            _displacements[node.Id] = Vector3.zero;

        //----------------------------------------
        // Repulsive forces
        //----------------------------------------

        //TODO: fix bug: calculates every pair twice. 
        foreach (Node u in graph.Nodes.Values)
        {
            foreach (Node v in graph.Nodes.Values)
            {
                if (u == v)
                    continue;

                Vector3 posU = positions[(u.Id, baseTime)];
                Vector3 posV = positions[(v.Id, baseTime)];

                Vector2 pU = new Vector2(posU.x, posU.z);
                Vector2 pV = new Vector2(posV.x, posV.z);

                Vector2 delta = pU - pV;

                float distance = Mathf.Max(delta.magnitude, 0.01f);

                Vector2 direction = delta.normalized;

                float force = (IdealSpacing * IdealSpacing) / distance;

                _displacements[u.Id] +=
                    new Vector3(direction.x, 0, direction.y) * force;
            }
        }

        //----------------------------------------
        // Attractive forces
        //----------------------------------------

        foreach (Edge edge in graph.Edges.Values)
        {
            Vector3 posU = positions[(edge.Node1.Id, baseTime)];
            Vector3 posV = positions[(edge.Node2.Id, baseTime)];

            Vector2 pU = new Vector2(posU.x, posU.z);
            Vector2 pV = new Vector2(posV.x, posV.z);

            Vector2 delta = pU - pV;

            float distance = Mathf.Max(delta.magnitude, 0.01f);

            Vector2 direction = delta.normalized;

            float force = (distance * distance) / IdealSpacing;

            Vector3 attraction =
                new Vector3(direction.x, 0, direction.y) * force;

            _displacements[edge.Node1.Id] -= attraction;
            _displacements[edge.Node2.Id] += attraction;
        }

        //----------------------------------------
        // Move first timestep
        //----------------------------------------

        foreach (Node node in graph.Nodes.Values)
        {
            Vector3 movement = _displacements[node.Id];

            if (movement != Vector3.zero)
            {
                movement = movement.normalized *
                           Mathf.Min(movement.magnitude, _temperature);
            }

            positions[(node.Id, baseTime)] += movement;
        }

        //----------------------------------------
        // Copy X/Z to every timestep
        //----------------------------------------

        for (int timestep = 0; timestep < graph.TimeSteps.Count; timestep++)
        {
            TimeSpan time = graph.TimeSteps[timestep];

            foreach (Node node in graph.Nodes.Values)
            {
                NodeSnapshot snapshot = node.DataSnapshots[time];

                Vector3 basePosition =
                    positions[(node.Id, baseTime)];

                float graphOffset =
                    VisualizationSettings.Instance.TimeStepZSize * timestep;

                float height =
                    graphOffset +
                    GetNodeHeight(snapshot) *
                    VisualizationSettings.Instance.NodeHeightScaleFactor;

                positions[(node.Id, time)] =
                    new Vector3(
                        basePosition.x,
                        height,
                        basePosition.z);
            }
        }

        //----------------------------------------
        // Cool temperature
        //----------------------------------------

        _temperature *= 0.99f;
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