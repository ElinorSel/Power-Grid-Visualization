using UnityEngine;
using System.Collections.Generic;

public class GraphData
{
    public Dictionary<string, Node> Nodes {get;} = new();
    public Dictionary<string, Edge> Edges {get;} = new();
    
}
