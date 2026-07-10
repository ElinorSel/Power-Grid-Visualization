using UnityEngine;
using System.Collections.Generic;
using System;


public class Node
{
    public string Id {get; }
    public Dictionary<TimeSpan, NodeSnapshot> DataSnapshots {get; set;} 
    public Node(string id)
    {
        this.Id = id;
        this.DataSnapshots = new Dictionary<TimeSpan, NodeSnapshot>();
    }
}
