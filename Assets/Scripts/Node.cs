using UnityEngine;
using System.Collections.Generic;
using System;

public class Node
{
    private float vMagnitude;
    private float vAngle = 0;

    public string VoltageLevelId {get; set;}

    public Vector2 Coordinates {get; set;}

    public TimeSpan TimeStamp {get; set;}

    public Node(float vMagnitude, float vAngle, string voltageLevel, Vector2 coordinates, TimeSpan timeStamp)
    {
    
        this.vMagnitude = vMagnitude;
        this.vAngle = vAngle;
        this.VoltageLevelId = voltageLevel;
        this.Coordinates = coordinates;
        this.TimeStamp = timeStamp;
    }

    public void DebugPrintData()
    {
        Debug.Log("Node Voltage Level ID: " + VoltageLevelId + ", Voltage Magnitude: " + vMagnitude + ", Voltage Angle: " + vAngle + ", Coordinates: " + Coordinates + ", TimeStamp: " + TimeStamp);
    }
}
