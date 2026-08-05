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
    public event Action OnTimeRangeChanged;
    public event Action OnHideLowLoadChanged;
    public event Action OnShowGeneratorPowerChanged;
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
    private bool hideLowLoad = false;

    [SerializeField]
    private bool showGeneratorPower = false;

    [SerializeField]
    [Tooltip("How tall each timeStep slice is")]
     private float timeStepZSize;

    [SerializeField]
    [Tooltip("The earliest time that will show")]
     private int visibleStartIndex = 0;

    [SerializeField]
    [Tooltip("The latest time that will show")]
     private int visibleEndIndex = 23;

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

    public bool HideLowLoad => hideLowLoad;
    public bool ShowGeneratorPower => showGeneratorPower;
    public float NodeHeightScaleFactor => nodeHeightScaleFactor;
    public float NodeSizeScaleFactor => nodeSizeScaleFactor;
    public float TimeStepZSize => timeStepZSize;
    public float EdgeWidthScaleFactor => edgeWidthScaleFactor;

    public int VisibleStartIndex => visibleStartIndex;
    public int VisibleEndIndex => visibleEndIndex;




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
        if(showLabels == show) {
        Debug.Log("VIZ settings: value is the same as before, returning...");
         return;}

        
        showLabels = show;
        OnLabelSettingsChanged?.Invoke();
    }

    public void SetHideLowLoad(bool hide)
    {
        if (hideLowLoad == hide)
        {
            Debug.Log("VIZ settings: value is the same as before, returning...");
            return;
        }

        hideLowLoad = hide;
        OnHideLowLoadChanged?.Invoke();
    }

public void SetShowGeneratorPower(bool show)
    {
        if (showGeneratorPower == show)
        {
            Debug.Log("VIZ settings: value is the same as before, returning...");
            return;
        }

        showGeneratorPower = show;
        OnShowGeneratorPowerChanged?.Invoke();
    }

    public void SetHeightScaleFactor(float scaleFactor)
    {
        if (nodeHeightScaleFactor == scaleFactor) return;
        nodeHeightScaleFactor = scaleFactor;
        //Debug.Log("VIZ settings: setting heightscale tp" + scaleFactor + ". Invoking Onlabelsettingschanged." );
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
    public void SetEdgeWidthScaleFactor(float value)
    {
        if(edgeWidthScaleFactor == value) return;
        edgeWidthScaleFactor = value;
        OnEdgeWidthChanged?.Invoke();
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
        OnEdgeWidthChanged?.Invoke(); //TODO: this handler needs to be changed in the viz manager to work more like the layout alogorithm change
    }

    public void SetTimeRange(float minValue, float maxValue)
    {
        if (visibleStartIndex == Mathf.RoundToInt(minValue) && visibleEndIndex == Mathf.RoundToInt(maxValue))
            return;

        visibleStartIndex = Mathf.RoundToInt(minValue);
        visibleEndIndex   = Mathf.RoundToInt(maxValue);

        OnTimeRangeChanged?.Invoke();
        
    }

}