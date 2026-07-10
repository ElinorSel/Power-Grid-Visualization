using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataImporter : MonoBehaviour
{
    [SerializeField] private int TimeRange = 24; //range of time that we go through, default 24 hours

    //filepaths for data
    [SerializeField] private string filePathConnections; //ieee188_lines.csv
    [SerializeField] private string fileFolderPath; //path to folder containing all datafiles

    //Fields for headers to make it more adaptable
    [SerializeField] private string ConnectionsFromHeading;
    [SerializeField] private string ConnectionsToHeading;
    [SerializeField] private string ConnectionsEdgeIDHeading;
    [SerializeField] private string ConnectionsInServiceHeading;
    [SerializeField] private string ConnectionsMaxLoadHeading;
    [SerializeField] private string NodePowerHeading;
    [SerializeField] private string NodeAngleHeading;
    [SerializeField] private string NodeIDHeading;
    [SerializeField] private string EdgeIDHeading;
    [SerializeField] private string EdgeLoadHeading;
    [SerializeField] private string EdgePowerFromHeading;
    [SerializeField] private string EdgePowerToHeading;

    //List of nodes and edges for iterating
    public List<Edge> edges {get; set;}
    public List<Node> nodes {get; set;}

    //Dictionaries of nodes and edges for quick ID lookup
    public Dictionary<string, Node> nodeLookup = new Dictionary<string, Node>();
    public Dictionary<string, Edge> edgeLookup = new Dictionary<string, Edge>();


    //Bool for graphmanager
    public bool ready = false;
    void Start()
    {
        nodes = new List<Node>();
        edges = new List<Edge>();
        ConnectEdgesToNodes();
        importHourNodeData();
        importHourEdgeData();
        ready = true;
    }

        void ConnectEdgesToNodes()
    {
        CSVReader csvReader = new CSVReader();
        List<string[]> data_values = csvReader.ReadCSVFile(filePathConnections);

        //Find the index of all the headings to make it adaptable to changes
        string[] data_headers = data_values[0];
        int Node1IDIndex = Array.IndexOf(data_headers, ConnectionsFromHeading);
        int Node2IDIndex = Array.IndexOf(data_headers, ConnectionsToHeading);
        int edgeIDIndex = Array.IndexOf(data_headers, ConnectionsEdgeIDHeading);
        int inServiceIndex = Array.IndexOf(data_headers, ConnectionsInServiceHeading);
        int maxLoadIndex = Array.IndexOf(data_headers, ConnectionsMaxLoadHeading);

        for(int i = 1; i < data_values.Count; i++)
        {
            importNodes(data_values[i][Node1IDIndex]);
            importNodes(data_values[i][Node2IDIndex]);
            //have to change so it's a node object and not just the nodeID for edges
            Node Node1 = nodeLookup[Node1IDIndex];
            Node Node2 = nodeLookup[Node2IDIndex];
            importEdges(data_values[i][edgeIDIndex], bool.Parse(data_values[i][inServiceIndex]), float.Parse(data_values[i][maxLoadIndex]),data_values[i], Node1, Node2);
        }
    }

    void importEdges(string ID, bool inService, float maxLoad, Node Node1, Node Node2)
    {
        Edge edge = new Edge(ID, inService, maxLoad, Node1, Node2);
        edgeLookup.Add(ID, edge); //add to dictionary
        edges.Add(edge); //add to list
        //edge.DebugPrintData();

        
    }

    void importNodes(string ID)
    {
        Node node = new Node(ID);
        nodeLookup.Add(ID, node); //add to dictionary
        nodes.Add(node); // add to list
        //node.DebugPrintData();
        
        
    }

    void importHourNodeData()
    {
        CSVReader csvReader = new CSVReader();
        //looping through time
        for (time = 0; time = TimeRange; time++)
        {
            //getting the correct file
            string filename = Path.Combine(fileFolderPath, $"ieee118_{time}_bus.csv");
            List<string[]> data_values = csvReader.ReadCSVFile(filename);

            //getting the index of headers in the file
            string[] data_headers = data_values[0];
            int NodeIDIndex = Array.IndexOf(data_headers, NodeIDHeading);
            int angleIndex = Array.IndexOf(data_headers, NodeAngleHeading);
            int powerIndex = Array.IndexOf(data_headers, NodePowerHeading);

            //looping through each line
            for(int i = 1; i < data_values.Count; i++)
            {
                string nodeID = data_values[i][NodeIDIndex];
                Node node = nodeLookup[nodeID];
                NodeSnapshot dataSnapShot = new NodeSnapshot(float.Parse(data_values[i][powerIndex]), float.Parse(data_values[i][angleIndex]));
                node.dataSnapshots[TimeSpan.Parse(time)] = dataSnapShot;
            }
        }

    }

    void importHourEdgeData()
    {
        CSVReader csvReader = new CSVReader();
        //looping through time
        for (time = 0; time = TimeRange; time++)
        {
            //getting the correct file
            string filename = Path.Combine(fileFolderPath, $"ieee118_{time}_line.csv");
            List<string[]> data_values = csvReader.ReadCSVFile(filename);

            //getting the index of headers in the file
            string[] data_headers = data_values[0];
            int EdgeIDIndex = Array.IndexOf(data_headers, EdgeIDHeading);
            int loadPercentIndex = Array.IndexOf(data_headers, EdgeLoadHeading);
            int powerFromIndex = Array.IndexOf(data_headers, EdgePowerFromHeading);
            int powerToIndex = Array.IndexOf(data_headers, EdgePowerToHeading);

            //looping through each line
            for(int i = 1; i < data_values.Count; i++)
            {
                string  edgeID = data_values[i][EdgeIDIndex];
                Edge edge = edgeLookup[edgeID];
                EdgeSnapShot dataSnapShot = new EdgeSnapShot(float.Parse(data_values[i][loadPercentIndex], float.Parse(data_values[i][powerFromIndex], float.Parse(data_values[i][powerToIndex]))));
                edge.data[TimeSpan.Parse(time)] = dataSnapShot;
            }
        }

    }

}
