using UnityEngine;

public class VisualizationSettings : MonoBehaviour
{
    public static VisualizationSettings Instance { get; private set; }

    public enum NodeHeightMapping
    {
        None,
        VoltageAngle,
        //VoltageMagnitude,
    }


    public enum NodeColorMapping //TODO: if needed
    {
        None,
        VoltageAngle,
        VoltageMagnitude,
    }

    public enum NodeSizeMapping
    {
        None,
        VoltageAngle,
        VoltageMagnitude,
    }

    public enum EdgeColorMapping
    {
        None,
        Load,
    }

    public enum EdgeWidthMapping{
        None,
        MVALimit,
    }

    //------------------------------ INSPECTOR SETTINGS------------------------------

    [Header("General Settings")]
    [SerializeField]
    private bool showLabels = true;


    //------------------------------
    [Header("Node Mapping")]
    //------------------------------
    [SerializeField]
    private NodeHeightMapping heightMappingNode = NodeHeightMapping.None;
    [SerializeField]
    private float heightScaleFactorNode = 1f;

    [SerializeField]
    private NodeColorMapping colorMappingNode = NodeColorMapping.None;

    [SerializeField]
    private NodeSizeMapping sizeMappingNode = NodeSizeMapping.None;

    [SerializeField]
    private float sizeScaleFactorNode;

    //------------------------------
    [Header("Edge Mapping")]
    //------------------------------
    [SerializeField]
    private EdgeColorMapping colorMappingEdge = EdgeColorMapping.None;

    [SerializeField]
    private EdgeWidthMapping widthMappingEdge = EdgeWidthMapping.MVALimit;

    [SerializeField]
    private float widthScaleFactorEdge = 1f;




//------------------------------ GET METHODS ------------------------------
    public NodeHeightMapping HeightMappingNode => heightMappingNode;
    public NodeColorMapping ColorMappingNode => colorMappingNode;
    public NodeSizeMapping SizeMappingNode => sizeMappingNode;
    public EdgeColorMapping ColorMappingEdge => colorMappingEdge;
    public EdgeWidthMapping WidthMappingEdge => widthMappingEdge;
    public bool ShowLabels => showLabels;
    public float HeightScaleFactor => heightScaleFactorNode;
    public float SizeScaleFactor => sizeScaleFactorNode;
    public float WidthScaleFactorEdge => widthScaleFactorEdge;


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
    public void SetHeightMapping(NodeHeightMapping mapping)
    {
        heightMappingNode = mapping;
    }

    public void SetColorMapping(NodeColorMapping mapping)
    {
        colorMappingNode = mapping;
    }

    public void SetSizeMapping(NodeSizeMapping mapping)
    {
        sizeMappingNode = mapping;
    }

    public void SetShowLabels(bool show)
    {
        showLabels = show;
    }

    public void SetHeightScaleFactor(float scaleFactor)
    {
        heightScaleFactorNode = scaleFactor;
    }

    public void SetSizeScaleFactor(float scaleFactor)
    {
        sizeScaleFactorNode = scaleFactor;
    }

    public void SetEdgeColorMapping(EdgeColorMapping mapping)
    {
        colorMappingEdge = mapping;
    }

    public void SetEdgeWidthMapping(EdgeWidthMapping mapping)
    {
        widthMappingEdge = mapping;
    }
}