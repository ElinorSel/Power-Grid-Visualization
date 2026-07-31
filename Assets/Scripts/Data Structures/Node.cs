using System.Collections.Generic;
using System;


public class Node
{
    public string Id {get; }
    public Dictionary<TimeSpan, NodeSnapshot> DataSnapshots {get; set;} 
    public List<Edge> Edges {get; set;}

    //for generators
    public bool IsGenerator { get; set; } = false;
    public string GeneratorType { get; set; } = null;

    public Node(string id)
    {
        this.Id = id;
        this.DataSnapshots = new Dictionary<TimeSpan, NodeSnapshot>();
        this.Edges = new List<Edge>();
    }
}
