using UnityEngine;
using System.Collections.Generic;
using System;

public class GraphManager : MonoBehaviour
{
    private DataImporter dataImporter;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject nodePrefab;

    private GraphData graph;

    void Start()
    {
        dataImporter = GetComponent<DataImporter>();
        if (dataImporter == null)
        {
            Debug.LogError("DataImporter component not found on the GameObject.");
            return;
        }
        graph = dataImporter.ImportData();
        InstantiateGraph();

    }

    void InstantiateGraph()
    {
        GameObject visualization = new GameObject("Visualization");
        for (int currentTimeStep = 0; currentTimeStep < graph.TimeSteps.Count; currentTimeStep++)
        {    
            GameObject graphParent = new GameObject($"Hour_{currentTimeStep}");
            graphParent.transform.SetParent(visualization.transform);

            Debug.Log("Instantiating nodes and edges.");
            // OBS for now, nodes MUST be instanciated before edges, as edges use the node positions. 
            InstantiateNodes(graphParent, graph.TimeSteps[currentTimeStep],currentTimeStep);
            InstantiateEdges(graphParent, graph.TimeSteps[currentTimeStep],currentTimeStep);
        }
    }

    void InstantiateNodes(GameObject graphParent, TimeSpan timeStep, int index)
    { 
        GameObject nodeParent = new GameObject("Nodes");
        nodeParent.transform.SetParent(graphParent.transform);

        foreach (Node node in graph.Nodes.Values)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent.transform);
            nodeObject.name = "Node_" + node.Id + "_" + timeStep;
            nodeObject.GetComponent<NodeVisualizer>().Initialize(node, timeStep, index);
        }
    }

    void InstantiateEdges(GameObject graphParent, TimeSpan timeStep, int index) //TODO: fix so it takes timespan into account 
    {
        GameObject edgeParent = new GameObject("Edges");
        edgeParent.transform.SetParent(graphParent.transform);
        foreach (Edge edge in graph.Edges.Values)
        {
            GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
            edgeObject.name = "Edge_" + edge.Id;
            edgeObject.GetComponent<EdgeVisualizer>().Initialize(edge, timeStep, index);
        }
    }
}
