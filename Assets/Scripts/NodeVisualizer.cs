using UnityEngine;
using TMPro;

public class NodeVisualizer : MonoBehaviour
{
    public Node Data {get; set;}
    [SerializeField] public TextMeshPro nodeID;

    public void Initialize(Node data)
    {
        Data = data;
        transform.position = new Vector3(data.Coordinates.x, 0, data.Coordinates.y);
        nodeID.text = data.VoltageLevelId;
    }
}
