using UnityEngine;
using TMPro;
using System;

public class NodeVisualizer : MonoBehaviour
{
    
    [SerializeField] public TextMeshPro nodeID;

    public Node Node {get; private set;}
    public NodeSnapshot Snapshot { get; private set; }
    public TimeSpan Time { get; private set; }
    public int TimeStepIndex { get; private set; }
    private float sizeMapping = 1f;


    public void Initialize(Node data, TimeSpan time, int timeStepIndex)
    {
        
        Node = data;
        Time = time;
        Snapshot = Node.DataSnapshots[time];
        TimeStepIndex = timeStepIndex;

        // [Height Mapping]
        switch (VisualizationSettings.Instance.NodeHeightMapping)
        {
            case VisualizationSettings.NodeHeightMappingOption.None:
                Snapshot.ZOffset = 0f + VisualizationSettings.Instance.TimeStepZSize * TimeStepIndex;
                break;
            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                Snapshot.ZOffset = CalculateZOffsetVoltageAngle() +  VisualizationSettings.Instance.TimeStepZSize  * TimeStepIndex;
                break;
            default:
                Debug.LogWarning("Unknown / Unimplementedheight mapping option.");
                break;
        }

        //[Size Mapping]
        switch (VisualizationSettings.Instance.NodeSizeMapping)
        {
            case VisualizationSettings.NodeSizeMappingOption.None:
                sizeMapping = VisualizationSettings.Instance.NodeSizeScaleFactor;
                break;
            case VisualizationSettings.NodeSizeMappingOption.VoltageMagnitude:
                sizeMapping = CalculateSizeMappingVoltageMagnitude();
                break;
            case VisualizationSettings.NodeSizeMappingOption.VoltageAngle:
                sizeMapping = CalculateSizeMappingVoltageAngle();
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented size mapping option.");
                break;
        }

        transform.position = new Vector3(Snapshot.Coordinates.x, Snapshot.ZOffset, Snapshot.Coordinates.y); 
        transform.localScale = Vector3.one * sizeMapping;

        // [Show Labels]
        if (VisualizationSettings.Instance.ShowLabels) nodeID.text = Node.Id;
        else nodeID.text = "";
    

        
    }

    public float CalculateZOffsetVoltageAngle()
    {
        //TODO: Implement a more sophisticated method to calculate the zOffset
        return Node.DataSnapshots[Time].VAngle * VisualizationSettings.Instance.NodeHeightScaleFactor;
    }

    public float CalculateSizeMappingVoltageMagnitude()
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        return Node.DataSnapshots[Time].Power * 0.01f * VisualizationSettings.Instance.NodeSizeScaleFactor;
    }

        public float CalculateSizeMappingVoltageAngle()
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        return Node.DataSnapshots[Time].VAngle * 0.01f * VisualizationSettings.Instance.NodeSizeScaleFactor;
    }

    
    
}
