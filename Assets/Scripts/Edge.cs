using UnityEngine;
using System.Collections.Generic;
using System;


public class Edge
{
    public string Id {get; set;}
    public bool InService {get;set;}
    public float MaxLoad {get; set;}

    public Node Node1 {get; set;}
    public Node Node2 {get; set;}
    
    public Dictionary<TimeSpan, EdgeSnapshot> DataSnapshots {get; set;} 

    public Edge(string id, bool inService, float maxload, Node node1, Node node2)
    {
        this.Id = id;
        this.InService = inService;
        this.MaxLoad = maxload;
        this.Node1 = node1;
        this.Node2 = node2;
        this.DataSnapshots = new Dictionary<TimeSpan, EdgeSnapshot>();
    }


    public void DebugPrintData()
    {
        Debug.Log($"Edge ID: {Id}, InService: {InService}, MaxLoad: {MaxLoad}, Node1 ID: {Node1.Id}, Node2 ID: {Node2.Id}");
    }


}
