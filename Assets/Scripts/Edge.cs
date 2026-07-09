using UnityEngine;
using System.Collections.Generic;

public class Edge
{
    public string Id {get; set;}
    private string type;
    public Node Node1 {get; set;}
    public Node Node2 {get; set;}
    public string VoltageLevel1Id {get; set;}
    public string VoltageLevel2Id {get; set;}
    private bool isConnected1;
    private bool isConnected2;
    public float Power {get; set;}
    public float ReactivePower {get; set;}
    private float current;
    public float NormalMVALimit {get; set;}

    public float Load {get; set;}

    public Edge(string id, string type, string voltageLevel1id, bool isConnected1, string voltageLevel2id, bool isConnected2, float power, float reactivePower, float current, float normalMVALimit)
    {
        this.Id = id;
        this.type = type;
        this.VoltageLevel1Id = voltageLevel1id;
        this.isConnected1 = isConnected1;
        this.VoltageLevel2Id = voltageLevel2id;
        this.isConnected2 = isConnected2;
        this.Power = power;
        this.ReactivePower = reactivePower;
        this.current = current;
        this.NormalMVALimit = normalMVALimit;
    }

    public void DebugPrintData()
    {
        Debug.Log("Edge ID: " + Id + ", Edge Type: " + type + ", Voltage Level 1 ID: " + VoltageLevel1Id + "Is Connected 1: " + isConnected1 + ", Voltage Level 2 ID: " + VoltageLevel2Id + ", Is Connected 2: " + isConnected2 + ", Power: " + Power + ", Reactive Power: " + ReactivePower + ", Current: " + current + ", Normal MVA Limit: " + NormalMVALimit);

    }
}
