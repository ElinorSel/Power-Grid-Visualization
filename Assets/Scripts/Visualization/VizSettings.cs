using UnityEngine;

public class VisualizationSettings : MonoBehaviour
{
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
        nodeLayoutAlgorithm = algoritm;
    }

    public void SetHeightMapping(NodeHeightMappingOption mapping)
    {
        nodeHeightMapping = mapping;
    }

    public void SetColorMapping(NodeColorMappingOption mapping)
    {
        nodeColorMapping = mapping;
    }

    public void SetSizeMapping(NodeSizeMappingOption mapping)
    {
        nodeSizeMapping = mapping;
    }

    public void SetShowLabels(bool show)
    {
        showLabels = show;
    }

    public void SetHeightScaleFactor(float scaleFactor)
    {
        nodeHeightScaleFactor = scaleFactor;
    }

    public void SetSizeScaleFactor(float scaleFactor)
    {
        nodeSizeScaleFactor = scaleFactor;
    }

    public void SetTimeStepZSize(float timeStepSize)
    {
        timeStepZSize = timeStepSize;
    }

    public void SetEdgeColorMapping(EdgeColorMappingOption mapping)
    {
        edgeColorMapping = mapping;
    }

    public void SetEdgeWidthMapping(EdgeWidthMappingOption mapping)
    {
        edgeWidthMapping = mapping;
    }
}