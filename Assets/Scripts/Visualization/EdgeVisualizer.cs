using UnityEngine;
using System;

public class EdgeVisualizer : MonoBehaviour
{
    public Edge Edge {get; set;}
    private Vector3 startPosition;
    private Vector3 endPosition;
    public EdgeSnapshot Snapshot { get; private set; }
    public TimeSpan Time { get; private set; }
    public int TimeStepIndex { get; private set; }

    [SerializeField] private float width = 0.5f;
    [SerializeField] private GameObject arrowPrefab;


    public void Initialize(Edge data,TimeSpan time, int timeStepIndex)
    { 
        Edge = data;
        Time = time;
        Snapshot = Edge.DataSnapshots[time];
        TimeStepIndex = timeStepIndex;

        startPosition = new Vector3(data.Node1.DataSnapshots[time].Coordinates.x, data.Node1.DataSnapshots[time].ZOffset, data.Node1.DataSnapshots[time].Coordinates.y);
        endPosition = new Vector3(data.Node2.DataSnapshots[time].Coordinates.x, data.Node2.DataSnapshots[time].ZOffset, data.Node2.DataSnapshots[time].Coordinates.y);

        // [Width Settings]
        switch (VisualizationSettings.Instance.EdgeWidthMapping)
        {
            case VisualizationSettings.EdgeWidthMappingOption.None:
                width = VisualizationSettings.Instance.EdgeWidthScaleFactor; 
                break;
            case VisualizationSettings.EdgeWidthMappingOption.MVALimit:
                //width = CalculateWidthMVALimit(data); //TODO: fix later
                width = VisualizationSettings.Instance.EdgeWidthScaleFactor; 
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented width mapping option for Edges.");
                break;
        }

        // [Color Settings]
        switch (VisualizationSettings.Instance.EdgeColorMapping)
        {
            case VisualizationSettings.EdgeColorMappingOption.None:
                break;
            case VisualizationSettings.EdgeColorMappingOption.Load:
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented color mapping option for Edges.");
                break;
        }


        //RenderEdge(width);
        //Direction(data);

        
        
    }

    void RenderEdge(float edgeWidth)
    {
        // Add a LineRenderer component
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;

        // Set the material
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the color
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // Set the width
        lineRenderer.startWidth = edgeWidth;
        lineRenderer.endWidth = edgeWidth;

        // Set the number of vertices
        lineRenderer.positionCount = 2;

        // Set the positions of the vertices
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    } 
/*
    void Direction (Edge edge)
    {
        if (edge.Power>0) //flowing from Node1 to Node2
        {
            Vector3 direction = (endPosition - startPosition).normalized;
            GameObject arrow = Instantiate(arrowPrefab, endPosition - direction * 12f, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            arrow.transform.localScale = new Vector3 (2*width, 2*width, 2*width);
            arrow.name = "Arrow_" + edge.Id;
        }
        else if (edge.Power<0) //flowing from Node2 to Node1
        {
            Vector3 direction = (startPosition - endPosition).normalized;
            GameObject arrow = Instantiate(arrowPrefab, startPosition - direction * 12f, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            arrow.transform.localScale = new Vector3 (2*width, 2*width, 2*width);
            arrow.name = "Arrow_" + edge.Id;
        }
        else
        {
            // No power flow, do not instantiate an arrow
            // If there is no flow we could change the color of the line
        }

    }
    */

    /*

    float CalculateWidthMVALimit(Edge edge)
    {
        //return (edge.NormalMVALimit / 100f) * VisualizationSettings.Instance.EdgeWidthScaleFactor; // Scale factor can be adjusted as needed
        float value = edge.NormalMVALimit / 100f;
        return Mathf.Pow(value, 1.3f) * VisualizationSettings.Instance.EdgeWidthScaleFactor;
    }

    */
    
}
