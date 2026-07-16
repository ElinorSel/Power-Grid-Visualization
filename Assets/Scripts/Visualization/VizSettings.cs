using UnityEngine;
using System;

public class VisualizationSettings : MonoBehaviour
{
    public event Action OnLayoutChanged;
    public event Action OnLayoutAlgorithmChanged;
    public event Action OnLabelSettingsChanged;
    public event Action OnNodeSizeChanged;
    public event Action OnNodeColorChanged;
    public event Action OnEdgeWidthChanged;
    public event Action OnEdgeColorChanged;
    public static VisualizationSettings Instance { get; private set; }

    public enum NodeHeightMappingOption
    {
        None,
        VoltageAngle,
        //VoltageMagnitude,
    }

    public enum NodeLayoutAlgorithOption
    {
        InitialData,
        ForceDirected, //TODO: rename to alg name if needed
    }


    public enum NodeColorMappingOption //TODO: if needed
    {
        None,
        VoltageAngle,
        VoltageMagnitude,
    }

    public enum NodeSizeMappingOption
    {
        None,
        VoltageAngle,
        VoltageMagnitude,
    }

    public enum EdgeColorMappingOption
    {
        None,
        Load,
    }

    public enum EdgeWidthMappingOption{
        None,
        MVALimit,
    }

    //------------------------------ INSPECTOR SETTINGS------------------------------

    [Header("General Settings")]
    [SerializeField]
    private bool showLabels = true;
    [SerializeField]
    [Tooltip("How tall each timeStep slice is")]
     private float timeStepZSize;

    [SerializeField]
    [Tooltip("Which Algorithm used to calculate node positions")]
    private NodeLayoutAlgorithOption nodeLayoutAlgorithm;



    //------------------------------
    [Header("Node Mapping")]
    //------------------------------
    [SerializeField]
    private NodeHeightMappingOption nodeHeightMapping = NodeHeightMappingOption.None;
    [SerializeField]
    private float nodeHeightScaleFactor = 2f;

    [SerializeField]
    private NodeColorMappingOption nodeColorMapping = NodeColorMappingOption.None;

    [SerializeField]
    private NodeSizeMappingOption nodeSizeMapping = NodeSizeMappingOption.VoltageMagnitude;

    [SerializeField]
    private float nodeSizeScaleFactor;

    //------------------------------
    [Header("Edge Mapping")]
    //------------------------------
    [SerializeField]
    private EdgeColorMappingOption edgeColorMapping = EdgeColorMappingOption.None;

    [SerializeField]
    private EdgeWidthMappingOption edgeWidthMapping = EdgeWidthMappingOption.MVALimit;

    [SerializeField]
    private float edgeWidthScaleFactor = 2f;




//------------------------------ GET METHODS ------------------------------
    public NodeLayoutAlgorithOption NodeLayoutAlgorithm => nodeLayoutAlgorithm;

    public NodeHeightMappingOption NodeHeightMapping => nodeHeightMapping;
    public NodeColorMappingOption NodeColorMapping => nodeColorMapping;
    public NodeSizeMappingOption NodeSizeMapping => nodeSizeMapping;
    public EdgeColorMappingOption EdgeColorMapping => edgeColorMapping;
    public EdgeWidthMappingOption EdgeWidthMapping => edgeWidthMapping;
    public bool ShowLabels => showLabels;
    public float NodeHeightScaleFactor => nodeHeightScaleFactor;
    public float NodeSizeScaleFactor => nodeSizeScaleFactor;
    public float TimeStepZSize => timeStepZSize;
    public float EdgeWidthScaleFactor => edgeWidthScaleFactor;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple VisualizationSettings instances found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }



//------------------------------ SET METHODS ------------------------------
    public void SetNodeLayoutAlgorithm(  NodeLayoutAlgorithOption algoritm)
    {
        if (nodeLayoutAlgorithm == algoritm) return;
        nodeLayoutAlgorithm = algoritm;
        OnLayoutAlgorithmChanged?.Invoke();
    }

    public void SetHeightMapping(NodeHeightMappingOption mapping)
    {
        if (nodeHeightMapping == mapping) return;
        nodeHeightMapping = mapping;
        OnLayoutChanged?.Invoke();
    }

    public void SetColorMapping(NodeColorMappingOption mapping)
    {
        if (nodeColorMapping == mapping) return;
        nodeColorMapping = mapping;
        OnNodeColorChanged?.Invoke();
    }

    public void SetSizeMapping(NodeSizeMappingOption mapping)
    {
        if (nodeSizeMapping == mapping) return;
        nodeSizeMapping = mapping;
        OnNodeSizeChanged?.Invoke();
    }

    public void SetShowLabels(bool show)
    {
        if(showLabels == show) return;
        showLabels = show;
        OnLabelSettingsChanged?.Invoke();
    }

    public void SetHeightScaleFactor(float scaleFactor)
    {
        if (nodeHeightScaleFactor == scaleFactor) return;
        nodeHeightScaleFactor = scaleFactor;
        OnLayoutChanged?.Invoke();
    }

    public void SetSizeScaleFactor(float scaleFactor)
    {
        if (nodeSizeScaleFactor == scaleFactor) return;
        nodeSizeScaleFactor = scaleFactor;
        OnNodeSizeChanged?.Invoke();
    }

    public void SetTimeStepZSize(float timeStepSize)
    {
        //TODO: timestepsize could be refactored in the whole codebase so that it simply moves the parent GO not loop through and change all nodes...
        if(timeStepZSize == timeStepSize) return;
        timeStepZSize = timeStepSize;
        OnLayoutChanged?.Invoke();
    }

    public void SetEdgeColorMapping(EdgeColorMappingOption mapping)
    {
        if(edgeColorMapping == mapping) return;
        edgeColorMapping = mapping;
        OnEdgeColorChanged?.Invoke();
    }

    public void SetEdgeWidthMapping(EdgeWidthMappingOption mapping)
    {
        if(edgeWidthMapping == mapping) return;
        edgeWidthMapping = mapping;
        OnEdgeWidthChanged?.Invoke();
    }
}