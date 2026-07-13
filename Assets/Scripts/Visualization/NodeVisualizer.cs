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
                Snapshot.ZOffset = 0f + VisualizationSettings.Instance.TimeStepZSize * timeStepIndex;
                break;
            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                Snapshot.ZOffset = CalculateZOffsetVoltageAngle() +  VisualizationSettings.Instance.TimeStepZSize  * timeStepIndex;
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
                sizeMapping = CalculateSizeMappingVoltageMagnitude(data);
                break;
            case VisualizationSettings.NodeSizeMappingOption.VoltageAngle:
                sizeMapping = CalculateSizeMappingVoltageAngle(data);
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented size mapping option.");
                break;
        }

        transform.position = new Vector3(Snapshot.Coordinates.x, Snapshot.ZOffset, Snapshot.Coordinates.y); 
        transform.localScale = Vector3.one * sizeMapping;

        // [Show Labels]
        if (VisualizationSettings.Instance.ShowLabels) nodeID.text = data.Id;
        else nodeID.text = "";
    

        
    }

    public float CalculateZOffsetVoltageAngle()
    {
        //TODO: Implement a more sophisticated method to calculate the zOffset
        //return data.VAngle * VisualizationSettings.Instance.NodeHeightScaleFactor;
        return VisualizationSettings.Instance.NodeHeightScaleFactor; //TODO: PLACEHOLDER
    }

    public float CalculateSizeMappingVoltageMagnitude(Node data)
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        //return data.VMagnitude * 0.02f * VisualizationSettings.Instance.NodeSizeScaleFactor;
        
        return VisualizationSettings.Instance.NodeSizeScaleFactor; //TODO: PLACEHOLDER
    }

        public float CalculateSizeMappingVoltageAngle(Node data)
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        //return data.VAngle * 0.2f* VisualizationSettings.Instance.NodeSizeScaleFactor;
        
        return VisualizationSettings.Instance.NodeSizeScaleFactor; //TODO: PLACEHOLDER
    }

    
    
}
