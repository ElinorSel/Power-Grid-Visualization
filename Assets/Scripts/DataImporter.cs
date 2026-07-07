using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataImporter : MonoBehaviour
{
    [SerializeField] private string filePathEdges;
    [SerializeField] private string filePathNodes;
    private List<Edge> edges;
    private List<Node> nodes;
    void Start()
    {
        importEdges();
        importNodes();
    }

    void importEdges()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathEdges);
        edges = new List<Edge>();
        for(int i = 1; i < data_values.Count; i++)
        {
            Edge edge = new Edge();
            edge.id = data_values[i][0];
            edge.type = data_values[i][1]; 
            edge.voltageLevel1id = data_values[i][2];
            edge.isConnected1 = bool.Parse((data_values[i][3]).ToLower());
            edge.voltageLevel2id = data_values[i][4];
            edge.isConnected2 = bool.Parse((data_values[i][5]).ToLower());
            edge.power = float.Parse(data_values[i][6]);
            edge.reactivePower = float.Parse(data_values[i][7]);
            edge.current = float.Parse(data_values[i][8]);
            edge.normalMVALimit = float.Parse(data_values[i][13]);
            edges.Add(edge);
            edge.DebugPrintData();

        }
    }

    void importNodes()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathNodes);
        nodes = new List<Node>();
        for(int i = 1; i < data_values.Count; i++)
        {
            Node node = new Node();
            node.vMagnitude = float.Parse(data_values[i][1]);
            node.vAngle = float.Parse(data_values[i][2]);
            node.voltageLevelId = data_values[i][3];
            node.coordinates = new Vector2(float.Parse(data_values[i][4]), float.Parse(data_values[i][5]));
            nodes.Add(node);
            node.DebugPrintData();
        }
        
    }
}
