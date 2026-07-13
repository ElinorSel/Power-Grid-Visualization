using UnityEngine;
using TMPro;

public class NodeVisualizer : MonoBehaviour
{
    public Node Data {get; private set;}
    [SerializeField] public TextMeshPro nodeID;

    private float sizeMapping = 1f;

    public void Initialize(Node data)
    {
        /*
        Data = data;

        // [Height Mapping]
        switch (VisualizationSettings.Instance.NodeHeightMapping)
        {
            case VisualizationSettings.NodeHeightMappingOption.None:
                data.ZOffset = 0f;
                break;
            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                data.ZOffset = CalculateZOffsetVoltageAngle(data);
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

        transform.position = new Vector3(data.Coordinates.x, data.ZOffset, data.Coordinates.y);
        transform.localScale = Vector3.one * sizeMapping;

        // [Show Labels]
        if (VisualizationSettings.Instance.ShowLabels) nodeID.text = data.Id;
        else nodeID.text = "";
    

        */
    }

    public float CalculateZOffsetVoltageAngle(Node data)
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
