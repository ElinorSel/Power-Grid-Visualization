using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Linq;

public class DataImporter : MonoBehaviour
{
    [SerializeField] private bool makeDebugFiles = false; //make files to compare with original data to check for errors
    [SerializeField] private int TimeRange = 24; //range of time that we go through, default 24 hours

    //filepaths for data
    [SerializeField] private string filePathConnections; //ieee188_lines.csv
    [SerializeField] private string fileFolderPath; //path to folder containing all datafiles

    //Fields for headers to make it more adaptable
    [SerializeField] private string ConnectionsFromHeading;
    [SerializeField] private string ConnectionsToHeading;
    [SerializeField] private string ConnectionsEdgeIDHeading = "";
    [SerializeField] private string ConnectionsInServiceHeading;
    [SerializeField] private string ConnectionsMaxLoadHeading;
    [SerializeField] private string NodePowerHeading;
    [SerializeField] private string NodeAngleHeading;
    [SerializeField] private string NodeIDHeading = ""; 
    [SerializeField] private string EdgeIDHeading = "";
    [SerializeField] private string EdgeLoadHeading;
    [SerializeField] private string EdgePowerFromHeading;
    [SerializeField] private string EdgePowerToHeading;


    //Dictionaries of nodes and edges 
    public GraphData Graph {get; private set;} = new();


    public GraphData ImportData()
    {
        
        ConnectEdgesToNodes();
        ImportHourNodeData();
        ImportHourEdgeData();
        if(makeDebugFiles)DebugDataImport();
        return Graph;
    }

        void ConnectEdgesToNodes()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathConnections);

        //Find the index of all the headings to make it adaptable to changes
        string[] data_headers = data_values[0];
        int node1IDIndex = Array.IndexOf(data_headers, ConnectionsFromHeading);
        int node2IDIndex = Array.IndexOf(data_headers, ConnectionsToHeading);
        int edgeIDIndex = Array.IndexOf(data_headers, ConnectionsEdgeIDHeading);
        int inServiceIndex = Array.IndexOf(data_headers, ConnectionsInServiceHeading);
        int maxLoadIndex = Array.IndexOf(data_headers, ConnectionsMaxLoadHeading);

        for(int i = 1; i < data_values.Count; i++)
        {
            Node Node1 = ImportNode(data_values[i][node1IDIndex]);
            Node Node2 = ImportNode(data_values[i][node2IDIndex]);
            ImportEdge(data_values[i][edgeIDIndex], bool.Parse(data_values[i][inServiceIndex]), float.Parse(data_values[i][maxLoadIndex]), Node1, Node2);
        }
    }

    void ImportEdge(string ID, bool inService, float maxLoad, Node Node1, Node Node2)
    {
        if (Graph.Edges.ContainsKey(ID))
        {
            Debug.LogWarning($"Duplicate edge ID found: {ID}");
            return;
        }
        Edge edge = new Edge(ID, inService, maxLoad, Node1, Node2);
        Graph.Edges.Add(ID, edge); //add to dictionary
        Node1.Edges.Add(edge);
        Node2.Edges.Add(edge);
    }

    Node ImportNode(string ID)
    {
        if (!Graph.Nodes.TryGetValue(ID, out Node node))
        {
            node = new Node(ID);
            Graph.Nodes.Add(ID, node); 
        }
        return node;
    }

    void ImportHourNodeData()
    {
        CSVReader csvReader = new CSVReader();
        //looping through time
        for (int time = 0; time < TimeRange; time++)
        {
            TimeSpan currentTime = TimeSpan.Parse(time.ToString());
            //getting the correct file
            string filename = Path.Combine(fileFolderPath, $"ieee118_hour_{time}_bus.csv");
            List<string[]> data_values = csvReader.ReadCSVFile(filename);

            //getting the index of headers in the file
            string[] data_headers = data_values[0];
            int nodeIDIndex = Array.IndexOf(data_headers, NodeIDHeading);
            int angleIndex = Array.IndexOf(data_headers, NodeAngleHeading);
            int powerIndex = Array.IndexOf(data_headers, NodePowerHeading);

            //looping through each line
            for(int i = 1; i < data_values.Count; i++)
            {
                string nodeID = data_values[i][nodeIDIndex];
                Node node = ImportNode(nodeID);
                
                NodeSnapshot dataSnapShot = new NodeSnapshot(float.Parse(data_values[i][powerIndex]), float.Parse(data_values[i][angleIndex]));
                node.DataSnapshots[currentTime] = dataSnapShot;
            }
        }

    }

    void ImportHourEdgeData()
    {
        CSVReader csvReader = new CSVReader();
        //looping through time
        for (int time = 0; time < TimeRange; time++)
        {
            TimeSpan currentTime = TimeSpan.Parse(time.ToString());
            Graph.TimeSteps.Add(currentTime); // Save the timestep to the graph object
            //getting the correct file
            string filename = Path.Combine(fileFolderPath, $"ieee118_hour_{time}_line.csv");
            List<string[]> data_values = csvReader.ReadCSVFile(filename);

            //getting the index of headers in the file
            string[] data_headers = data_values[0];
            int edgeIDIndex = Array.IndexOf(data_headers, EdgeIDHeading);
            int loadPercentIndex = Array.IndexOf(data_headers, EdgeLoadHeading);
            int powerFromIndex = Array.IndexOf(data_headers, EdgePowerFromHeading);
            int powerToIndex = Array.IndexOf(data_headers, EdgePowerToHeading);

            //looping through each line
            for(int i = 1; i < data_values.Count; i++)
            {
                string  edgeID = data_values[i][edgeIDIndex];
                Edge edge = Graph.Edges[edgeID]; //assuming that all edges have been imported in the first step
                EdgeSnapshot dataSnapShot = new EdgeSnapshot(float.Parse(data_values[i][loadPercentIndex]), float.Parse(data_values[i][powerFromIndex]), float.Parse(data_values[i][powerToIndex]));
                edge.DataSnapshots[TimeSpan.Parse(time.ToString())] = dataSnapShot;
            }
        }

    }

    void DebugDataImport()
    {
        string timeStep = "10"; //we are only checking the first timestep for now, can be expanded later
        //CONNECTIONS
        Debug.Log("Creating Debugging CSV file for Connections. Data points:" + Graph.Edges.Count);
        var csvConnections = new StringBuilder();
        csvConnections.AppendLine("EdgeID, Node1ID, Node2ID, InService, MaxLoad");

        foreach (Edge edge in Graph.Edges.Values){
            var newLine = $"{edge.Id},{edge.Node1.Id},{edge.Node2.Id},{edge.InService},{edge.MaxLoad}";
            csvConnections.AppendLine(newLine); 
        }
        File.WriteAllText("Assets/Data/Debugging/Connections.csv", csvConnections.ToString());

        
        //EDGES
        Debug.Log("Creating Debugging CSV file for Edges. Data points:" + Graph.Edges.Count + "Time step: " + timeStep);
        var csvEdges = new StringBuilder();
        csvEdges.AppendLine("EdgeID, PowerFrom, PowerTo, Load");

        foreach (Edge edge in Graph.Edges.Values){
            var newLine = $"{edge.Id},{edge.DataSnapshots[TimeSpan.Parse(timeStep)].PowerFrom}, {edge.DataSnapshots[TimeSpan.Parse(timeStep)].PowerTo}, {edge.DataSnapshots[TimeSpan.Parse(timeStep)].Load}";
            csvEdges.AppendLine(newLine); 
        }
        
        File.WriteAllText("Assets/Data/Debugging/Edges.csv", csvEdges.ToString());

        //NODES
        Debug.Log("Creating Debugging CSV file for Nodes. Data points:" + Graph.Nodes.Count + "Time step: " + timeStep);
        var csvNodes = new StringBuilder();
        csvNodes.AppendLine("NodeID, VAngle, Power");

        foreach (Node node in Graph.Nodes.Values.OrderBy(n => int.Parse(n.Id))){
            var newLine = $"{node.Id},{node.DataSnapshots[TimeSpan.Parse(timeStep)].VAngle}, {node.DataSnapshots[TimeSpan.Parse(timeStep)].Power}";
            csvNodes.AppendLine(newLine); 
        }
        
        File.WriteAllText("Assets/Data/Debugging/Nodes.csv", csvNodes.ToString());
    }

}
