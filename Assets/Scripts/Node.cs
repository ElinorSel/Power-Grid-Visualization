using UnityEngine;

public class Node
{
    public float VMagnitude {get; }
    public float VAngle  {get; }
    public string VoltageLevelId {get; }
    public Vector2 Coordinates {get; }
    public float ZOffset {get; set;} //not from data CSV and is soley for vixualization purposes, to offset the node in the z direction based on its voltage angle
    public Node(float vMagnitude, float vAngle, string voltageLevel, Vector2 coordinates)
    {
        this.VMagnitude = vMagnitude;
        this.VAngle = vAngle;
        this.VoltageLevelId = voltageLevel;
        this.Coordinates = coordinates;
        this.ZOffset = 0f; 
    }

    public void DebugPrintData()
    {
        Debug.Log("Node Voltage Level ID: " + VoltageLevelId + ", Voltage Magnitude: " + VMagnitude + ", Voltage Angle: " + VAngle + ", Coordinates: " + Coordinates);
    }
}
