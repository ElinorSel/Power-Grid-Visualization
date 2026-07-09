using UnityEngine;
using TMPro;

public class NodeVisualizer : MonoBehaviour
{
    public Node Data {get; private set;}
    private bool useHeightOffset = true;
    [SerializeField] public TextMeshPro nodeID;
    private float zOffset = 0f;
    [SerializeField] private float zScaleFactor = 5f;

    public void Initialize(Node data)
    {

        Data = data;
        if(useHeightOffset)CalculateZOffset(data);
        transform.position = new Vector3(data.Coordinates.x, data.ZOffset, data.Coordinates.y);
        nodeID.text = data.VoltageLevelId;
    }

    public void CalculateZOffset(Node data)
    {
        //TODO: Implement a more sophisticated method to calculate the zOffset
        data.ZOffset = data.VAngle * zScaleFactor;
    }
}
