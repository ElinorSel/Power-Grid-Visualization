using UnityEngine;
using System.Collections.Generic;
using System;

public class GraphData
{
    public Dictionary<string, Node> Nodes {get;} = new();
    public Dictionary<string, Edge> Edges {get;} = new();
    public List<TimeSpan> TimeSteps { get;} = new();
    
}
