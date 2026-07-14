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

        startPosition = new Vector3(Edge.Node1.DataSnapshots[time].Coordinates.x, Edge.Node1.DataSnapshots[time].ZOffset, Edge.Node1.DataSnapshots[time].Coordinates.y);
        endPosition = new Vector3(Edge.Node2.DataSnapshots[time].Coordinates.x, Edge.Node2.DataSnapshots[time].ZOffset, Edge.Node2.DataSnapshots[time].Coordinates.y);

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


        RenderEdge(width);
        CalculateWidthMVALimit(); //TODO: will we have other mappings to width?
        Direction();

        
        
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



    void Direction ()
    {
        //TODO: idk if the direction is correct
        if (Edge.DataSnapshots[Time].Direction>0) //flowing from Node1 to Node2
        {
            Vector3 direction = (endPosition - startPosition).normalized;
            GameObject arrow = Instantiate(arrowPrefab, endPosition - direction * 2f, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            arrow.transform.localScale = new Vector3 (2*width, 2*width, 2*width);
            arrow.name = "Arrow_" + Edge.Id;
        }
        else if (Edge.DataSnapshots[Time].Direction<0) //flowing from Node2 to Node1
        {
            Vector3 direction = (startPosition - endPosition).normalized;
            GameObject arrow = Instantiate(arrowPrefab, startPosition - direction * 2f, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            arrow.transform.localScale = new Vector3 (2*width, 2*width, 2*width);
            arrow.name = "Arrow_" + Edge.Id;
        }
        else
        {
            // No power flow, do not instantiate an arrow
            // If there is no flow we could change the color of the line
            Debug.LogWarning("No power flow in Edge_" + Edge.Id);
        }

    }
    

  
    float CalculateWidthMVALimit()
    {
        //TODO: change to suitable value
        float value = Edge.MaxLoad / 120f;
        return Mathf.Pow(value, 1.3f) * VisualizationSettings.Instance.EdgeWidthScaleFactor;
    }

    
}
