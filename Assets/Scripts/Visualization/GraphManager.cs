using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class GraphManager : MonoBehaviour
{
    private DataImporter dataImporter;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Material edgeMaterial;

    private GraphData graphData ;
    private GraphLayout layout= new();
    private GraphStyle style = new();

    void Start()
    {
        dataImporter = GetComponent<DataImporter>();
        if (dataImporter == null)
        {
            Debug.LogError("DataImporter component not found on the GameObject.");
            return;
        }
        graphData = dataImporter.ImportData();
        StartCoroutine(InstantiateGraph());

    }

    IEnumerator InstantiateGraph()
    {
        GameObject visualization = new GameObject("Visualization");
        for (int currentTimeStep = 0; currentTimeStep < graphData.TimeSteps.Count; currentTimeStep++)
        {    
            GameObject graphParent = new GameObject($"Hour_{currentTimeStep}");
            graphParent.transform.SetParent(visualization.transform);

            Debug.Log("Instantiating nodes and edges.");
            // OBS for now, nodes MUST be instanciated before edges, as edges use the node positions. 
            yield return StartCoroutine(InstantiateNodes(graphParent, graphData.TimeSteps[currentTimeStep],currentTimeStep));
            yield return StartCoroutine(InstantiateEdges(graphParent, graphData.TimeSteps[currentTimeStep],currentTimeStep));
            // wait one frame before creating the next timestep
             yield return null;
        }
        Debug.Log("Finished instantiating graph");
    }

    IEnumerator InstantiateNodes(GameObject graphParent, TimeSpan timeStep, int index)
    { 
        GameObject nodeParent = new GameObject("Nodes");
        nodeParent.transform.SetParent(graphParent.transform);
        int count = 0;
        foreach (Node node in graphData.Nodes.Values)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent.transform);
            nodeObject.name = "Node_" + node.Id + "_" + timeStep;
            nodeObject.GetComponent<NodeVisualizer>().Initialize(node, timeStep, index, layout, style);
            count++;
            if (count % 20 == 0)yield return null; //Pause 1 frame every 20 nodes
        }
    }

    IEnumerator InstantiateEdges(GameObject graphParent, TimeSpan timeStep, int index) //TODO: fix so it takes timespan into account 
    {
        GameObject edgeParent = new GameObject("Edges");
        edgeParent.transform.SetParent(graphParent.transform);
        int count = 0;

        foreach (Edge edge in graphData.Edges.Values)
        {
            GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
            edgeObject.name = "Edge_" + edge.Id;
            edgeObject.GetComponent<EdgeVisualizer>().Initialize(edge, timeStep, index,  layout, style, edgeMaterial);
            count++;
            if (count % 20 == 0)yield return null; //Pause 1 frame every 20 edges

        }
    }
}
