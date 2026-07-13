using UnityEngine;
using System.Collections.Generic;

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
        Debug.Log("Instantiating nodes and edges.");
        InstantiateNodes();
        InstantiateEdges();
    }

    void InstantiateNodes()
    { 
        GameObject nodeParent = new GameObject("Nodes");
        foreach (Node node in graph.Nodes.Values)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent.transform);
            nodeObject.name = "Node_" + node.Id;
            nodeObject.GetComponent<NodeVisualizer>().Initialize(node);
        }
    }

    void InstantiateEdges()
    {
        GameObject edgeParent = new GameObject("Edges");
        foreach (Edge edge in graph.Edges.Values)
        {
            GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
            edgeObject.name = "Edge_" + edge.Id;
            edgeObject.GetComponent<EdgeVisualizer>().Initialize(edge);
        }
    }
}
