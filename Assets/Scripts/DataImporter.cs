using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataImporter : MonoBehaviour
{
    [SerializeField] private string filePathEdges;
    [SerializeField] private string filePathNodes;
    public List<Edge> edges {get; set;}
    public List<Node> nodes {get; set;}
    public bool ready = false;
    void Start()
    {
        importEdges();
        importNodes();
        ConnectEdgesToNodes();
        ready = true;
    }

    void importEdges()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathEdges);
        edges = new List<Edge>();
        for(int i = 1; i < data_values.Count; i++)
        {
            Edge edge = new Edge(data_values[i][0], data_values[i][1], data_values[i][2], bool.Parse((data_values[i][3]).ToLower()), data_values[i][4], bool.Parse((data_values[i][5]).ToLower()), float.Parse(data_values[i][6]), float.Parse(data_values[i][7]), float.Parse(data_values[i][8]), float.Parse(data_values[i][13]));
            edges.Add(edge);
            //edge.DebugPrintData();

            //calculate load for each edge
            edge.Load = Mathf.Sqrt(Mathf.Pow(edge.Power, 2) + Mathf.Pow(edge.ReactivePower, 2)) / edge.NormalMVALimit;
            Debug.Log("Edge " + edge.Id + " Load: " + edge.Load);

        }
    }

    void importNodes()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathNodes);
        nodes = new List<Node>();
        for(int i = 1; i < data_values.Count; i++)
        {
            Node node = new Node(float.Parse(data_values[i][1]), float.Parse(data_values[i][2]), data_values[i][3], new Vector2(float.Parse(data_values[i][4]), float.Parse(data_values[i][5])));
            nodes.Add(node);
            //node.DebugPrintData();
        }
        
    }

    void ConnectEdgesToNodes()
    {
        for(int i = 0; i < edges.Count; i++)
        {
            for(int j = 0; j < nodes.Count; j++)
            {
                if(edges[i].VoltageLevel1Id == nodes[j].VoltageLevelId)
                {
                    edges[i].Node1 = nodes[j];
                    Debug.Log("Edge " + edges[i].Id + " connected to Node1 " + nodes[j].VoltageLevelId);

                }
                
                if(edges[i].VoltageLevel2Id == nodes[j].VoltageLevelId)
                {
                    edges[i].Node2 = nodes[j];
                    Debug.Log("Edge " + edges[i].Id + " connected to Node2 " + nodes[j].VoltageLevelId);
                }

            }
        }
    }
}
