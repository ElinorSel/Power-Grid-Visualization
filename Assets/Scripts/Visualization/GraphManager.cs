using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

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

    private Dictionary<int, GameObject> _timeStepParents = new();

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
        VisualizationSettings.Instance.OnTimeRangeChanged += HandleTimeRangeChanged;
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
        VisualizationSettings.Instance.OnTimeRangeChanged -= HandleTimeRangeChanged;
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

        //create the Node and edges GameObjects, also starts simulation if the simulation is dynamic
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

    private string GetEdgePairKey(Edge edge)
    {
        string a = edge.Node1.Id;
        string b = edge.Node2.Id;

        return a.CompareTo(b) <= 0 ? $"{a}-{b}" : $"{b}-{a}";
    }

    IEnumerator InstantiateGraph()
    {
        GameObject visualization = new GameObject("_____VISUALIZATION____");
        for (int currentTimeStep = 0; currentTimeStep < _graphData.TimeSteps.Count; currentTimeStep++)
        {    
            GameObject graphParent = new GameObject($"Hour_{currentTimeStep}");
            graphParent.transform.SetParent(visualization.transform);
            _timeStepParents[currentTimeStep] = graphParent;

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

        var groupedEdges = _graphData.Edges.Values.GroupBy(e => GetEdgePairKey(e)).ToList();

        foreach (var group in groupedEdges)
        {
            int groupIndex = 0;
            foreach (Edge edge in group)
            {
                GameObject edgeObject = Instantiate(edgePrefab, edgeParent.transform); 
                edgeObject.name = "Edge_" + edge.Id;

                EdgeVisualizer visualizer = edgeObject.GetComponent<EdgeVisualizer>();
                visualizer.Initialize(edge, timeStep, index,  _layout, _style, edgeMaterial, groupIndex, group.Count()); //
                _edgeVisualizers.Add(visualizer);
                count++;
                if (count % 20 == 0)yield return null; //Pause 1 frame every 20 edges

                groupIndex++;
            }

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
            edge.RefreshDirectionArrow();
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

    //=============Time Range Functions ========================
        private List<TimeSpan> GetVisibleTimeSteps()
    {
        int startIndex = VisualizationSettings.Instance.VisibleStartIndex;
        int endIndex = VisualizationSettings.Instance.VisibleEndIndex;

        if (startIndex < 0) startIndex = 0;
        if (endIndex >= _graphData.TimeSteps.Count) endIndex = _graphData.TimeSteps.Count - 1;
        return _graphData.TimeSteps
        .Where((timeStep, index) => index >= startIndex && index <= endIndex)
        .ToList();
    }

    private void UpdateTimeStepVisibility()
    {
        for (int i = 0; i < _graphData.TimeSteps.Count; i++)
            {
                bool isVisible = IsTimeStepVisible(i);

                _timeStepParents[i].SetActive(isVisible);
            }
    }

    private bool IsTimeStepVisible(int index)
    {
        return index >= VisualizationSettings.Instance.VisibleStartIndex
                && index <= VisualizationSettings.Instance.VisibleEndIndex;
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

        _layout.Initialize(CreateLayoutAlgorithm(), _graphData);


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

        foreach (var edge in _edgeVisualizers)
        {
            edge.RefreshDirectionArrow();
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
            edge.RefreshDirectionArrow();
        }
    }
    private void HandleEdgeColorChanged()
    {
        foreach(var edge in _edgeVisualizers)
        {
            edge.RefreshColor();
        }
    }

    private void HandleTimeRangeChanged()
    {
        List<TimeSpan> visibleTimeSteps = GetVisibleTimeSteps();
        UpdateTimeStepVisibility();
        _layout.UpdateHeightPositions(visibleTimeSteps);
    }

}
