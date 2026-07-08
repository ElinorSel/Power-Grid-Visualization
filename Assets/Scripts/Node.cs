using UnityEngine;
using System.Collections.Generic;

public class Node
{
    private float vMagnitude;
    private float vAngle = 0;

    public string VoltageLevelId {get; set;}

    public Vector2 Coordinates {get; set;}

    public Node(float vMagnitude, float vAngle, string voltageLevel, Vector2 coordinates)
    {
    
        this.vMagnitude = vMagnitude;
        this.vAngle = vAngle;
        this.VoltageLevelId = voltageLevel;
        this.Coordinates = coordinates;
    }

    public void DebugPrintData()
    {
        Debug.Log("Node Voltage Level ID: " + VoltageLevelId + ", Voltage Magnitude: " + vMagnitude + ", Voltage Angle: " + vAngle + ", Coordinates: " + Coordinates);
    }
}
