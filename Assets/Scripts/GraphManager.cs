using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    private DataImporter dataImporter;
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
        if (dataImporter.Ready && !instantiated)
        {
            Debug.Log("DataImporter is ready. Instantiating nodes and edges.");
            InstantiateNodes();
            InstantiateEdges();
            instantiated = true;
        }
    }

    void InstantiateNodes()
    { 
        GameObject nodeParent = new GameObject("Nodes");
        foreach (Node node in dataImporter.Nodes.Values)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent.transform);
            nodeObject.name = "Node_" + node.Id;
            nodeObject.GetComponent<NodeVisualizer>().Initialize(node);
        }
    }

    void InstantiateEdges()
    {
        GameObject edgeParent = new GameObject("Edges");
        foreach (Edge edge in dataImporter.Edges.Values)
        {
            GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
            edgeObject.name = "Edge_" + edge.Id;
            edgeObject.GetComponent<EdgeVisualizer>().Initialize(edge);
        }
    }
}
