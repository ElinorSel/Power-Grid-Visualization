using UnityEngine;
using TMPro;

public class NodeVisualizer : MonoBehaviour
{
    public Node Data {get; private set;}
    [SerializeField] public TextMeshPro nodeID;

    private float sizeMapping = 1f;

    public void Initialize(Node data)
    {

        Data = data;

        // [Height Mapping]
        switch (VisualizationSettings.Instance.HeightMappingNode)
        {
            case VisualizationSettings.NodeHeightMapping.None:
                data.ZOffset = 0f;
                break;
            case VisualizationSettings.NodeHeightMapping.VoltageAngle:
                data.ZOffset = CalculateZOffsetVoltageAngle(data);
                break;
            default:
                Debug.LogWarning("Unknown / Unimplementedheight mapping option.");
                break;
        }

        //[Size Mapping]
        switch (VisualizationSettings.Instance.SizeMappingNode)
        {
            case VisualizationSettings.NodeSizeMapping.None:
                sizeMapping = VisualizationSettings.Instance.SizeScaleFactor;
                break;
            case VisualizationSettings.NodeSizeMapping.VoltageMagnitude:
                sizeMapping = CalculateSizeMappingVoltageMagnitude(data);
                break;
            case VisualizationSettings.NodeSizeMapping.VoltageAngle:
                sizeMapping = CalculateSizeMappingVoltageAngle(data);
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented size mapping option.");
                break;
        }

        transform.position = new Vector3(data.Coordinates.x, data.ZOffset, data.Coordinates.y);
        transform.localScale = Vector3.one * sizeMapping;

        // [Show Labels]
        if (VisualizationSettings.Instance.ShowLabels) nodeID.text = data.VoltageLevelId;
        else nodeID.text = "";
    }

    public float CalculateZOffsetVoltageAngle(Node data)
    {
        //TODO: Implement a more sophisticated method to calculate the zOffset
        return data.VAngle * VisualizationSettings.Instance.SizeScaleFactor;
    }

    public float CalculateSizeMappingVoltageMagnitude(Node data)
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        return data.VMagnitude * 0.02f * VisualizationSettings.Instance.SizeScaleFactor;
    }

        public float CalculateSizeMappingVoltageAngle(Node data)
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        return data.VAngle * 0.2f* VisualizationSettings.Instance.SizeScaleFactor;
    }
    
}
