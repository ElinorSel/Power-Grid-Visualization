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


    [Header("Node Mapping")]
    [SerializeField]
    private NodeHeightMapping heightMapping = NodeHeightMapping.None;
    [SerializeField]
    private float heightScaleFactor;

    [SerializeField]
    private NodeColorMapping colorMapping = NodeColorMapping.None;

    [SerializeField]
    private NodeSizeMapping sizeMapping = NodeSizeMapping.None;

    [SerializeField]
    private float sizeScaleFactor;


    [Header("General Settings")]
    [SerializeField]
    private bool showLabels = true;


    public NodeHeightMapping HeightMapping => heightMapping;
    public NodeColorMapping ColorMapping => colorMapping;
    public NodeSizeMapping SizeMapping => sizeMapping;
    public bool ShowLabels => showLabels;
    public float HeightScaleFactor => heightScaleFactor;
    public float SizeScaleFactor => sizeScaleFactor;


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



    // Setters for the UI to change settings
    public void SetHeightMapping(NodeHeightMapping mapping)
    {
        heightMapping = mapping;
    }

    public void SetColorMapping(NodeColorMapping mapping)
    {
        colorMapping = mapping;
    }

    public void SetSizeMapping(NodeSizeMapping mapping)
    {
        sizeMapping = mapping;
    }

    public void SetShowLabels(bool show)
    {
        showLabels = show;
    }

    public void SetHeightScaleFactor(float scaleFactor)
    {
        heightScaleFactor = scaleFactor;
    }

    public void SetSizeScaleFactor(float scaleFactor)
    {
        sizeScaleFactor = scaleFactor;
    }
}