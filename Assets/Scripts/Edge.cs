using UnityEngine;
using System.Collections.Generic;

public class Edge
{
    public string Id {get; set;}

    public bool InService {get;set;}
    public float MaxLoad {get; set;}

    public Node Node1 {get; set;}
    public Node Node2 {get; set;}

    public Edge(string id, bool inService, float maxload, Node node1, Node node2)
    {
        this.Id = id;
        this.InService = inService;
        this.MaxLoad = maxload;
        this.Node1 = node1;
        this.Node2 = node2;
    }

/*
    public void DebugPrintData()
    {
        Debug.Log("Edge ID: " + Id + ", Edge Type: " + type + ", Voltage Level 1 ID: " + VoltageLevel1Id + "Is Connected 1: " + isConnected1 + ", Voltage Level 2 ID: " + VoltageLevel2Id + ", Is Connected 2: " + isConnected2 + ", Power: " + Power + ", Reactive Power: " + ReactivePower + ", Current: " + current + ", Normal MVA Limit: " + NormalMVALimit);

    }

    */
}
