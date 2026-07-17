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

    private Coroutine _simulationCoroutine;

    private GraphData _graphData;
    private GraphLayout _layout = new();
    private GraphStyle _style = new();

    private readonly List<NodeVisualizer> _nodeVisualizers = new();
    private readonly List<EdgeVisualizer> _edgeVisualizers = new();

    private void SubscribeToSettingsEvents() {
        if (VisualizationSettings.Instance == null)
        {
            Debug.LogError("VisualizationSettings instance missing!");
            return;
        }
        VisualizationSettings.Instance.OnLayoutChanged += HandleLayoutChanged;
        VisualizationSettings.Instance.OnLayoutAlgorithmChanged += HandleLayoutAlgorithmChanged;
        VisualizationSettings.Instance.OnLabelSettingsChanged += HandleLabelSettingsChanged;
        VisualizationSettings.Instance.OnNodeSizeChanged += HandleNodeSizeChanged;
        VisualizationSettings.Instance.OnNodeColorChanged += HandleNodeColorChanged;
        VisualizationSettings.Instance.OnEdgeWidthChanged += HandleEdgeWidthChanged;
        VisualizationSettings.Instance.OnEdgeColorChanged += HandleEdgeColorChanged;
        Debug.Log("Subscribed to label event");
    }
    private void OnDestroy()
    {
        if (VisualizationSettings.Instance == null)return;

        VisualizationSettings.Instance.OnLayoutChanged -= HandleLayoutChanged;
        VisualizationSettings.Instance.OnLayoutAlgorithmChanged -= HandleLayoutAlgorithmChanged;
        VisualizationSettings.Instance.OnLabelSettingsChanged -= HandleLabelSettingsChanged;
        VisualizationSettings.Instance.OnNodeSizeChanged -= HandleNodeSizeChanged;
        VisualizationSettings.Instance.OnNodeColorChanged -= HandleNodeColorChanged;
        VisualizationSettings.Instance.OnEdgeWidthChanged -= HandleEdgeWidthChanged;
        VisualizationSettings.Instance.OnEdgeColorChanged -= HandleEdgeColorChanged;
    }

    void Start()
    {
        SubscribeToSettingsEvents();
        dataImporter = GetComponent<DataImporter>();
        if (dataImporter == null)
        {
            Debug.LogError("DataImporter component not found on the GameObject.");
            return;
        }
        
        //save nodes and edges to graph data
        _graphData = dataImporter.ImportData();
        //layout will handle where nodes are positioned. Create a new layout using the current viz settings

        //Fill the layout with the initial graph data (saves the start positions of nodes and edges)
        _layout.Initialize(CreateLayoutAlgorithm(), _graphData);

        //create the Node and edges GameObjects, 
        StartCoroutine(InstantiateGraph());
    }

    private INodeLayoutAlgorithm CreateLayoutAlgorithm()
    {
        switch (VisualizationSettings.Instance.NodeLayoutAlgorithm)
        {
            case VisualizationSettings.NodeLayoutAlgorithOption.InitialData:
                return new InitalDataLayout();  
            case VisualizationSettings.NodeLayoutAlgorithOption.ForceDirected:
                 return new ForceDirectedLayout(); 
            default:
                Debug.LogWarning("Unknown / Unimplementedheight algorithm option for Node Layout. Using initial data instead.");
                return new InitalDataLayout(); 
        }
    }

    IEnumerator InstantiateGraph()
    {
        GameObject visualization = new GameObject("Visualization");
        for (int currentTimeStep = 0; currentTimeStep < _graphData.TimeSteps.Count; currentTimeStep++)
        {    
            GameObject graphParent = new GameObject($"Hour_{currentTimeStep}");
            graphParent.transform.SetParent(visualization.transform);

            //Debug.Log("Instantiating nodes and edges.");
            yield return StartCoroutine(InstantiateNodes(graphParent, _graphData.TimeSteps[currentTimeStep],currentTimeStep));
            yield return StartCoroutine(InstantiateEdges(graphParent, _graphData.TimeSteps[currentTimeStep],currentTimeStep));
            // wait one frame before creating the next timestep
             yield return null;
        }
        Debug.Log("Finished instantiating graph");
        if(_layout.IsDynamic()){ 
            Debug.Log("Simulation is dynamic. Starting simulation....");
            StartSimulation();
        }

    }

    IEnumerator InstantiateNodes(GameObject graphParent, TimeSpan timeStep, int index)
    { 
        GameObject nodeParent = new GameObject("Nodes");
        nodeParent.transform.SetParent(graphParent.transform);
        int count = 0;
        foreach (Node node in _graphData.Nodes.Values)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent.transform);
            nodeObject.name = "Node_" + node.Id + "_" + timeStep;
            NodeVisualizer visualizer = nodeObject.GetComponent<NodeVisualizer>();
            visualizer.Initialize(node, timeStep, index, _layout, _style);
            _nodeVisualizers.Add(visualizer);
            count++;
            if (count % 20 == 0)yield return null; //Pause 1 frame every 20 nodes
        }
    }

    IEnumerator InstantiateEdges(GameObject graphParent, TimeSpan timeStep, int index) //TODO: fix so it takes timespan into account 
    {
        GameObject edgeParent = new GameObject("Edges");
        edgeParent.transform.SetParent(graphParent.transform);
        int count = 0;

        foreach (Edge edge in _graphData.Edges.Values)
        {
            GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
            edgeObject.name = "Edge_" + edge.Id;

            EdgeVisualizer visualizer = edgeObject.GetComponent<EdgeVisualizer>();
            visualizer.Initialize(edge, timeStep, index,  _layout, _style, edgeMaterial);
            _edgeVisualizers.Add(visualizer);
            count++;
            if (count % 20 == 0)yield return null; //Pause 1 frame every 20 edges

        }
    }

    public void RefreshLayoutVisualizers()
    {
        foreach(var node in _nodeVisualizers)
        {
            node.RefreshPosition();
        }

        foreach(var edge in _edgeVisualizers)
        {
            edge.RefreshPosition();
        }
    }

        private IEnumerator RunSimulation()
    {
        WaitForSeconds wait = new(0.01f);
        while(!_layout.IsSimulating())
        {
            _layout.UpdateLayout();

            RefreshLayoutVisualizers();

            yield return wait;
        }
        Debug.Log("Stopped simulation.");
    }

    private void StartSimulation()
    {
        _simulationCoroutine = StartCoroutine(RunSimulation());
    }

    //=============Event Handlers==========================

    private void HandleLayoutChanged()
    {
        _layout.UpdateLayout();
        RefreshLayoutVisualizers();
    }
    private void HandleLayoutAlgorithmChanged()
    {
        if (_simulationCoroutine != null)
        {
            StopCoroutine(_simulationCoroutine);
            _simulationCoroutine = null;
        }

        _layout.SetAlgorithm(CreateLayoutAlgorithm());

        if (_layout.IsDynamic())
        {
            _simulationCoroutine = StartCoroutine(RunSimulation());
        }
        else
        {
            _layout.UpdateLayout();
            RefreshLayoutVisualizers();
        }
    }
    private void HandleLabelSettingsChanged()
    {
        foreach(var node in _nodeVisualizers)
        {
            node.RefreshLabel();
        }
    }

    private void HandleNodeSizeChanged()
    {
        foreach(var node in _nodeVisualizers)
        {
            node.RefreshNodeSize();
        }
    }
    private void HandleNodeColorChanged()
    {
        foreach(var node in _nodeVisualizers)
        {
            node.RefreshNodeColor();
        }
    }
    private void HandleEdgeWidthChanged()
    {
        foreach(var edge in _edgeVisualizers)
        {
            edge.RefreshWidth();
        }
    }
    private void HandleEdgeColorChanged()
    {
        foreach(var edge in _edgeVisualizers)
        {
            edge.RefreshColor();
        }
    }
}
