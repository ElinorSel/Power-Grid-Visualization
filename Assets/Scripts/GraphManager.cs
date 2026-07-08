using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    private DataImporter dataImporter;
    private List<Edge> edges;
    private List<Node> nodes;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject nodePrefab;

    private bool instantiated = false;

    void Start()
    {
        dataImporter = GetComponent<DataImporter>();
        if (dataImporter == null)
        {
            Debug.LogError("DataImporter component not found on the GameObject.");
            return;
        }

    }

    void Update()
    {
        if (dataImporter.ready && !instantiated)
        {
            Debug.Log("DataImporter is ready. Instantiating nodes and edges.");
            edges = dataImporter.edges;
            nodes = dataImporter.nodes;
            InstantiateNodes();
            InstantiateEdges();
            instantiated = true;
        }
    }

    void InstantiateNodes()
    { 
        GameObject nodeParent = new GameObject("Nodes");
        foreach (Node node in nodes)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent.transform);
            nodeObject.name = "Node_" + node.VoltageLevelId;
            nodeObject.GetComponent<NodeVisualizer>().Initialize(node);
        }
    }

    void InstantiateEdges()
    {
        GameObject edgeParent = new GameObject("Edges");
        foreach (Edge edge in edges)
        {
            GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
            edgeObject.name = "Edge_" + edge.Id;
            edgeObject.GetComponent<EdgeVisualizer>().Initialize(edge);
        }
    }
}
