using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text;
using System.Linq;

public class DataImporter : MonoBehaviour
{
    [SerializeField] private bool makeDebugFiles = false;

    // Number of timesteps/hours to import
    [SerializeField] private int TimeRange = 24;

    // Folder inside StreamingAssets containing all CSV files
    // Expected path:
    // Assets/StreamingAssets/Data/
    [SerializeField] private string dataFolderName = "Data";


    // =========================================================
    // CSV HEADER NAMES
    // =========================================================

    [Header("Connections CSV Headers")]

    [SerializeField] private string ConnectionsFromHeading;
    [SerializeField] private string ConnectionsToHeading;
    [SerializeField] private string ConnectionsEdgeIDHeading = "";
    [SerializeField] private string ConnectionsInServiceHeading;
    [SerializeField] private string ConnectionsMaxLoadHeading;


    [Header("Node CSV Headers")]

    [SerializeField] private string NodePowerHeading;
    [SerializeField] private string NodeAngleHeading;
    [SerializeField] private string NodeIDHeading = "";


    [Header("Edge CSV Headers")]

    [SerializeField] private string EdgeIDHeading = "";
    [SerializeField] private string EdgeLoadHeading;
    [SerializeField] private string EdgePowerFromHeading;
    [SerializeField] private string EdgePowerToHeading;


    // =========================================================
    // DATA
    // =========================================================

    public GraphData Graph { get; private set; } = new GraphData();

    private CSVReader _csvReader;


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    private void Awake()
    {
        _csvReader = GetComponent<CSVReader>();

        if (_csvReader == null)
        {
            _csvReader = gameObject.AddComponent<CSVReader>();
        }
    }


    // =========================================================
    // MAIN IMPORT METHOD
    // =========================================================

    /// <summary>
    /// Imports all power grid data asynchronously.
    ///
    /// The import order is:
    ///
    /// 1. Import connections and create Nodes/Edges
    /// 2. Import node data for every timestep
    /// 3. Import edge data for every timestep
    /// 4. Optionally create debug CSV files
    ///
    /// GraphManager should wait for this coroutine to finish
    /// before initializing GraphLayout or creating visualizers.
    /// </summary>
    public IEnumerator ImportData(
        Action<GraphData> onComplete)
    {
        Debug.Log("Starting graph data import...");


        // Make sure Graph starts empty in case ImportData
        // is called more than once.
        Graph = new GraphData();


        // =====================================================
        // 1. IMPORT CONNECTIONS
        // =====================================================

        yield return StartCoroutine(
            ConnectEdgesToNodes()
        );


        Debug.Log(
            $"Connections imported. " +
            $"Nodes: {Graph.Nodes.Count}, " +
            $"Edges: {Graph.Edges.Count}"
        );


        // =====================================================
        // 2. IMPORT NODE DATA
        // =====================================================

        yield return StartCoroutine(
            ImportHourNodeData()
        );


        Debug.Log(
            "Node data imported successfully."
        );


        // =====================================================
        // 3. IMPORT EDGE DATA
        // =====================================================

        yield return StartCoroutine(
            ImportHourEdgeData()
        );


        Debug.Log(
            "Edge data imported successfully."
        );


        // =====================================================
        // 4. CREATE DEBUG FILES
        // =====================================================

        if (makeDebugFiles)
        {
            DebugDataImport();
        }


        // =====================================================
        // IMPORT COMPLETE
        // =====================================================

        Debug.Log(
            $"Graph data import complete.\n" +
            $"Nodes: {Graph.Nodes.Count}\n" +
            $"Edges: {Graph.Edges.Count}\n" +
            $"Timesteps: {Graph.TimeSteps.Count}"
        );


        // Return the completed GraphData to GraphManager
        onComplete?.Invoke(Graph);
    }


    // =========================================================
    // FILE PATH HANDLING
    // =========================================================

    /// <summary>
    /// Returns the full path to a file inside
    /// StreamingAssets/Data.
    ///
    /// Supported platforms:
    /// - Unity Editor
    /// - Windows standalone
    /// - Android / Meta Quest
    /// - WebGL
    ///
    /// Example:
    /// GetDataFilePath("ieee118_lines.csv")
    /// </summary>
    private string GetDataFilePath(
        string filename)
    {
        string path =
            System.IO.Path.Combine(
                Application.streamingAssetsPath,
                dataFolderName,
                filename
            );


#if UNITY_EDITOR || UNITY_STANDALONE

        // On desktop platforms, StreamingAssets
        // is a normal filesystem directory.
        // UnityWebRequest expects a file:// URL.
        // Uri builds a correct file:/// URL on Windows.
        return new Uri(path).AbsoluteUri;


#else

        // On Android, Application.streamingAssetsPath
        // points inside the APK (jar:file://...).
        // On WebGL, Unity provides the appropriate URL.
        // Do not add file:// here.
        return path;

#endif
    }

    /// <summary>
    /// Parses floats from CSV using invariant culture
    /// so '.' decimal separators work on all locales (Quest, WebGL, etc.).
    /// </summary>
    private static float ParseFloat(string value)
    {
        return float.Parse(
            value,
            CultureInfo.InvariantCulture
        );
    }


    // =========================================================
    // CONNECTIONS
    // =========================================================

    /// <summary>
    /// Imports the connections CSV.
    ///
    /// This creates all Nodes and Edges in GraphData.
    /// </summary>
    private IEnumerator ConnectEdgesToNodes()
    {
        string filePath =
            GetDataFilePath(
                "ieee118_lines.csv"
            );


        List<string[]> dataValues = null;


        // Load CSV asynchronously
        yield return StartCoroutine(
            _csvReader.ReadCSVFile(
                filePath,
                result =>
                {
                    dataValues = result;
                }
            )
        );


        // Check that the file loaded successfully
        if (dataValues == null ||
            dataValues.Count == 0)
        {
            Debug.LogError(
                $"Could not import connections CSV:\n" +
                $"{filePath}"
            );

            yield break;
        }


        // =====================================================
        // FIND HEADER INDICES
        // =====================================================

        string[] dataHeaders =
            dataValues[0];


        int node1IDIndex =
            Array.IndexOf(
                dataHeaders,
                ConnectionsFromHeading
            );


        int node2IDIndex =
            Array.IndexOf(
                dataHeaders,
                ConnectionsToHeading
            );


        int edgeIDIndex =
            Array.IndexOf(
                dataHeaders,
                ConnectionsEdgeIDHeading
            );


        int inServiceIndex =
            Array.IndexOf(
                dataHeaders,
                ConnectionsInServiceHeading
            );


        int maxLoadIndex =
            Array.IndexOf(
                dataHeaders,
                ConnectionsMaxLoadHeading
            );


        // Validate header indices
        if (!ValidateColumnIndex(
                node1IDIndex,
                ConnectionsFromHeading,
                filePath) ||
            !ValidateColumnIndex(
                node2IDIndex,
                ConnectionsToHeading,
                filePath) ||
            !ValidateColumnIndex(
                edgeIDIndex,
                ConnectionsEdgeIDHeading,
                filePath) ||
            !ValidateColumnIndex(
                inServiceIndex,
                ConnectionsInServiceHeading,
                filePath) ||
            !ValidateColumnIndex(
                maxLoadIndex,
                ConnectionsMaxLoadHeading,
                filePath))
        {
            yield break;
        }


        // =====================================================
        // IMPORT EACH CONNECTION
        // =====================================================

        for (
            int i = 1;
            i < dataValues.Count;
            i++)
        {
            string node1ID =
                dataValues[i][node1IDIndex];


            string node2ID =
                dataValues[i][node2IDIndex];


            string edgeID =
                dataValues[i][edgeIDIndex];


            bool inService =
                bool.Parse(
                    dataValues[i][inServiceIndex]
                );


            float maxLoad =
                ParseFloat(
                    dataValues[i][maxLoadIndex]
                );


            Node node1 =
                ImportNode(node1ID);


            Node node2 =
                ImportNode(node2ID);


            ImportEdge(
                edgeID,
                inService,
                maxLoad,
                node1,
                node2
            );
        }


        Debug.Log(
            $"Imported {Graph.Nodes.Count} nodes " +
            $"and {Graph.Edges.Count} edges."
        );
    }


    // =========================================================
    // IMPORT EDGE
    // =========================================================

    private void ImportEdge(
        string ID,
        bool inService,
        float maxLoad,
        Node Node1,
        Node Node2)
    {
        if (Graph.Edges.ContainsKey(ID))
        {
            Debug.LogWarning(
                $"Duplicate edge ID found: {ID}"
            );

            return;
        }


        Edge edge =
            new Edge(
                ID,
                inService,
                maxLoad,
                Node1,
                Node2
            );


        Graph.Edges.Add(
            ID,
            edge
        );


        Node1.Edges.Add(
            edge
        );


        Node2.Edges.Add(
            edge
        );
    }


    // =========================================================
    // IMPORT NODE
    // =========================================================

    private Node ImportNode(
        string ID)
    {
        if (!Graph.Nodes.TryGetValue(
                ID,
                out Node node))
        {
            node =
                new Node(
                    ID
                );


            Graph.Nodes.Add(
                ID,
                node
            );
        }


        return node;
    }


    // =========================================================
    // NODE DATA
    // =========================================================

    /// <summary>
    /// Imports node data for every timestep.
    ///
    /// Example:
    /// ieee118_hour_0_bus.csv
    /// ieee118_hour_1_bus.csv
    /// ...
    /// ieee118_hour_23_bus.csv
    /// </summary>
    private IEnumerator ImportHourNodeData()
    {
        for (
            int time = 0;
            time < TimeRange;
            time++)
        {
            TimeSpan currentTime =
                TimeSpan.FromHours(time);


            string filename =
                GetDataFilePath(
                    $"ieee118_hour_{time}_bus.csv"
                );


            List<string[]> dataValues = null;


            // Load CSV asynchronously
            yield return StartCoroutine(
                _csvReader.ReadCSVFile(
                    filename,
                    result =>
                    {
                        dataValues = result;
                    }
                )
            );


            if (dataValues == null ||
                dataValues.Count == 0)
            {
                Debug.LogError(
                    $"Could not import node CSV:\n" +
                    $"{filename}"
                );

                yield break;
            }


            // =================================================
            // FIND HEADER INDICES
            // =================================================

            string[] dataHeaders =
                dataValues[0];


            int nodeIDIndex =
                Array.IndexOf(
                    dataHeaders,
                    NodeIDHeading
                );


            int angleIndex =
                Array.IndexOf(
                    dataHeaders,
                    NodeAngleHeading
                );


            int powerIndex =
                Array.IndexOf(
                    dataHeaders,
                    NodePowerHeading
                );


            if (!ValidateColumnIndex(
                    nodeIDIndex,
                    NodeIDHeading,
                    filename) ||
                !ValidateColumnIndex(
                    angleIndex,
                    NodeAngleHeading,
                    filename) ||
                !ValidateColumnIndex(
                    powerIndex,
                    NodePowerHeading,
                    filename))
            {
                yield break;
            }


            // =================================================
            // IMPORT EACH NODE
            // =================================================

            for (
                int i = 1;
                i < dataValues.Count;
                i++)
            {
                string nodeID =
                    dataValues[i][nodeIDIndex];


                Node node =
                    ImportNode(
                        nodeID
                    );


                float power =
                    ParseFloat(
                        dataValues[i][powerIndex]
                    );


                float angle =
                    ParseFloat(
                        dataValues[i][angleIndex]
                    );


                NodeSnapshot dataSnapshot =
                    new NodeSnapshot(
                        power,
                        angle
                    );


                node.DataSnapshots[
                    currentTime
                ] = dataSnapshot;
            }


            Debug.Log(
                $"Imported node data for timestep {time}."
            );
        }
    }


    // =========================================================
    // EDGE DATA
    // =========================================================

    /// <summary>
    /// Imports edge data for every timestep.
    ///
    /// Example:
    /// ieee118_hour_0_line.csv
    /// ieee118_hour_1_line.csv
    /// ...
    /// ieee118_hour_23_line.csv
    /// </summary>
    private IEnumerator ImportHourEdgeData()
    {
        for (
            int time = 0;
            time < TimeRange;
            time++)
        {
            TimeSpan currentTime =
                TimeSpan.FromHours(time);


            // Save timestep to GraphData
            Graph.TimeSteps.Add(
                currentTime
            );


            string filename =
                GetDataFilePath(
                    $"ieee118_hour_{time}_line.csv"
                );


            List<string[]> dataValues = null;


            // Load CSV asynchronously
            yield return StartCoroutine(
                _csvReader.ReadCSVFile(
                    filename,
                    result =>
                    {
                        dataValues = result;
                    }
                )
            );


            if (dataValues == null ||
                dataValues.Count == 0)
            {
                Debug.LogError(
                    $"Could not import edge CSV:\n" +
                    $"{filename}"
                );

                yield break;
            }


            // =================================================
            // FIND HEADER INDICES
            // =================================================

            string[] dataHeaders =
                dataValues[0];


            int edgeIDIndex =
                Array.IndexOf(
                    dataHeaders,
                    EdgeIDHeading
                );


            int loadPercentIndex =
                Array.IndexOf(
                    dataHeaders,
                    EdgeLoadHeading
                );


            int powerFromIndex =
                Array.IndexOf(
                    dataHeaders,
                    EdgePowerFromHeading
                );


            int powerToIndex =
                Array.IndexOf(
                    dataHeaders,
                    EdgePowerToHeading
                );


            if (!ValidateColumnIndex(
                    edgeIDIndex,
                    EdgeIDHeading,
                    filename) ||
                !ValidateColumnIndex(
                    loadPercentIndex,
                    EdgeLoadHeading,
                    filename) ||
                !ValidateColumnIndex(
                    powerFromIndex,
                    EdgePowerFromHeading,
                    filename) ||
                !ValidateColumnIndex(
                    powerToIndex,
                    EdgePowerToHeading,
                    filename))
            {
                yield break;
            }


            // =================================================
            // IMPORT EACH EDGE
            // =================================================

            for (
                int i = 1;
                i < dataValues.Count;
                i++)
            {
                string edgeID =
                    dataValues[i][edgeIDIndex];


                if (!Graph.Edges.TryGetValue(
                        edgeID,
                        out Edge edge))
                {
                    Debug.LogError(
                        $"Edge with ID '{edgeID}' " +
                        $"was found in {filename}, " +
                        $"but was not imported from " +
                        $"ieee118_lines.csv."
                    );

                    continue;
                }


                float load =
                    ParseFloat(
                        dataValues[i][loadPercentIndex]
                    );


                float powerFrom =
                    ParseFloat(
                        dataValues[i][powerFromIndex]
                    );


                float powerTo =
                    ParseFloat(
                        dataValues[i][powerToIndex]
                    );


                EdgeSnapshot dataSnapshot =
                    new EdgeSnapshot(
                        load,
                        powerFrom,
                        powerTo
                    );


                edge.DataSnapshots[
                    currentTime
                ] = dataSnapshot;
            }


            Debug.Log(
                $"Imported edge data for timestep {time}."
            );
        }
    }


    // =========================================================
    // HEADER VALIDATION
    // =========================================================

    /// <summary>
    /// Checks whether a CSV column was found.
    /// </summary>
    private bool ValidateColumnIndex(
        int index,
        string heading,
        string filename)
    {
        if (index == -1)
        {
            Debug.LogError(
                $"Could not find CSV header '{heading}' " +
                $"in file:\n{filename}"
            );

            return false;
        }


        return true;
    }


    // =========================================================
    // DEBUG DATA
    // =========================================================

    /// <summary>
    /// Creates debug CSV files containing imported data.
    ///
    /// Files are written to Application.persistentDataPath
    /// because this location is writable on Android/Quest.
    /// </summary>
    private void DebugDataImport()
    {
        TimeSpan timeStep = TimeSpan.FromHours(0);


        // =====================================================
        // CREATE DEBUG DIRECTORY
        // =====================================================

        string debugDirectory =
            System.IO.Path.Combine(
                Application.persistentDataPath,
                "Debugging"
            );


        System.IO.Directory.CreateDirectory(
            debugDirectory
        );


        // =====================================================
        // CONNECTIONS
        // =====================================================

        Debug.Log(
            "Creating debugging CSV file for Connections. " +
            $"Data points: {Graph.Edges.Count}"
        );


        var csvConnections =
            new StringBuilder();


        csvConnections.AppendLine(
            "EdgeID,Node1ID,Node2ID,InService,MaxLoad"
        );


        foreach (
            Edge edge in
            Graph.Edges.Values)
        {
            var newLine =
                $"{edge.Id}," +
                $"{edge.Node1.Id}," +
                $"{edge.Node2.Id}," +
                $"{edge.InService}," +
                $"{edge.MaxLoad}";


            csvConnections.AppendLine(
                newLine
            );
        }


        string connectionsPath =
            System.IO.Path.Combine(
                debugDirectory,
                "Connections.csv"
            );


        System.IO.File.WriteAllText(
            connectionsPath,
            csvConnections.ToString()
        );


        // =====================================================
        // EDGES
        // =====================================================

        Debug.Log(
            "Creating debugging CSV file for Edges. " +
            $"Data points: {Graph.Edges.Count}. " +
            $"Time step: {timeStep}"
        );


        var csvEdges =
            new StringBuilder();


        csvEdges.AppendLine(
            "EdgeID,PowerFrom,PowerTo,Load"
        );


        foreach (
            Edge edge in
            Graph.Edges.Values)
        {
            var newLine =
                $"{edge.Id}," +
                $"{edge.DataSnapshots[timeStep].PowerFrom}," +
                $"{edge.DataSnapshots[timeStep].PowerTo}," +
                $"{edge.DataSnapshots[timeStep].Load}";


            csvEdges.AppendLine(
                newLine
            );
        }


        string edgesPath =
            System.IO.Path.Combine(
                debugDirectory,
                "Edges.csv"
            );


        System.IO.File.WriteAllText(
            edgesPath,
            csvEdges.ToString()
        );


        // =====================================================
        // NODES
        // =====================================================

        Debug.Log(
            "Creating debugging CSV file for Nodes. " +
            $"Data points: {Graph.Nodes.Count}. " +
            $"Time step: {timeStep}"
        );


        var csvNodes =
            new StringBuilder();


        csvNodes.AppendLine(
            "NodeID,VAngle,Power"
        );


        foreach (
            Node node in
            Graph.Nodes.Values
                .OrderBy(n => int.Parse(n.Id)))
        {
            var newLine =
                $"{node.Id}," +
                $"{node.DataSnapshots[timeStep].VAngle}," +
                $"{node.DataSnapshots[timeStep].Power}";


            csvNodes.AppendLine(
                newLine
            );
        }


        string nodesPath =
            System.IO.Path.Combine(
                debugDirectory,
                "Nodes.csv"
            );


        System.IO.File.WriteAllText(
            nodesPath,
            csvNodes.ToString()
        );


        // =====================================================
        // LOG LOCATION
        // =====================================================

        Debug.Log(
            $"Debug CSV files created at:\n" +
            $"{debugDirectory}"
        );
    }
}
