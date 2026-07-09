using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class DataImporter : MonoBehaviour
{
    [SerializeField] private string filePathEdges;
    [SerializeField] private string filePathNodes;
    public List<Edge> edges {get; set;}
    public List<Node> nodes {get; set;}
    public bool ready = false;

    public Dictionary<TimeSpan, List<Node>> nodesByTime { get; set; }
    public Dictionary<TimeSpan, List<Edge>> edgesByTime { get; set; }
    void Start()
    {
        edges = new List<Edge>();
        nodes = new List<Node>();

        edgesByTime = new Dictionary<TimeSpan, List<Edge>>();
        nodesByTime = new Dictionary<TimeSpan, List<Node>>();

        importEdges();
        importNodes();
        ConnectEdgesToNodes();
        ready = true;
    }

    void importEdges()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathEdges);
        for(int i = 1; i < data_values.Count; i++)
        {
            Edge edge = new Edge(data_values[i][0], data_values[i][1], data_values[i][2], bool.Parse((data_values[i][3]).ToLower()), data_values[i][4], bool.Parse((data_values[i][5]).ToLower()), float.Parse(data_values[i][6]), float.Parse(data_values[i][7]), float.Parse(data_values[i][8]), float.Parse(data_values[i][13]), TimeSpan.Parse(data_values[i][14]));
            edges.Add(edge);
            //edge.DebugPrintData();

            //calculate load for each edge
            edge.Load = Mathf.Sqrt(Mathf.Pow(edge.Power, 2) + Mathf.Pow(edge.ReactivePower, 2)) / edge.NormalMVALimit;

            //make dictionary depending on time
            TimeSpan time = edge.TimeStamp;

            if (!edgesByTime.ContainsKey(time))
                edgesByTime[time] = new List<Edge>();

            edgesByTime[time].Add(edge);

        }
    }

    void importNodes()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathNodes);
        for(int i = 1; i < data_values.Count; i++)
        {
            Node node = new Node(float.Parse(data_values[i][1]), float.Parse(data_values[i][2]), data_values[i][3], new Vector2(float.Parse(data_values[i][4]), float.Parse(data_values[i][5])), TimeSpan.Parse(data_values[i][6]));
            nodes.Add(node);
            //node.DebugPrintData();

            //make dictionary depending on time
            TimeSpan time = node.TimeStamp;

            if (!nodesByTime.ContainsKey(time))
                nodesByTime[time] = new List<Node>();

            nodesByTime[time].Add(node);
        }


        /*
        if (nodesByTime.TryGetValue(TimeSpan.Parse("23:00"), out var hourNodes))
        {
            foreach (var node in hourNodes)
            {
                Debug.Log("Node in dictionary at 23:00" + node.VoltageLevelId);
            }
        }
        */
        
    }

    void ConnectEdgesToNodes()
    {
        for(int i = 0; i < edges.Count; i++)
        {
            TimeSpan edgeTime = edges[i].TimeStamp;

            // Making sure the nodes are at the same time
            if (!nodesByTime.TryGetValue(edgeTime, out var nodesAtTime))
                continue;
            for(int j = 0; j < nodesAtTime.Count; j++)
            {
                if(edges[i].VoltageLevel1Id == nodesAtTime[j].VoltageLevelId)
                {
                    edges[i].Node1 = nodesAtTime[j];
                    Debug.Log("Edge " + edges[i].Id + " connected to Node1 " + nodesAtTime[j].VoltageLevelId + " at time" + edgeTime);

                }
                
                if(edges[i].VoltageLevel2Id == nodesAtTime[j].VoltageLevelId)
                {
                    edges[i].Node2 = nodesAtTime[j];
                    Debug.Log("Edge " + edges[i].Id + " connected to Node2 " + nodesAtTime[j].VoltageLevelId + " at time" + edgeTime);
                }

            }
        }
    }
}
