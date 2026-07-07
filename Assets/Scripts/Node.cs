using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public string voltageLevelId;

    public float vMagnitude;

    public Vector2 coordinates;

    [SerializeField] public float vAngle = 0;
    [SerializeField] public List<Edge> edges;

    public void DebugPrintData()
    {
        Debug.Log("Node Voltage Level ID: " + voltageLevelId + ", Voltage Magnitude: " + vMagnitude + ", Voltage Angle: " + vAngle + ", Coordinates: " + coordinates);
    }
}
