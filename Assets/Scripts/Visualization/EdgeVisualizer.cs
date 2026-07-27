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

    private LineRenderer lineRenderer;
    [SerializeField] private GameObject arrowPrefab;

    private GraphLayout _layout;
    private GraphStyle _style; 

    private GameObject directionArrow;


    public void Initialize(Edge data,TimeSpan time, int timeStepIndex, GraphLayout layout, GraphStyle style, Material edgeMaterial)
    { 
        Edge = data;
        Time = time;
        
        _layout = layout;
        _style = style; 
        
        TimeStepIndex = timeStepIndex;

        startPosition = layout.GetNodePosition(Edge.Node1.Id, time);
        endPosition = layout.GetNodePosition(Edge.Node2.Id, time);
        RenderEdge(style.GetEdgeWidth(Edge, time), edgeMaterial);
        RenderDirectionArrow();  //TODO: add back again once fixxed
        
    }

    void RenderEdge(float edgeWidth, Material edgeMaterial)
    {
        // Add a LineRenderer component
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;

        // Set the material
        lineRenderer.material = edgeMaterial;

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

    public void RefreshPosition()
    {
        startPosition = _layout.GetNodePosition(Edge.Node1.Id, Time);
        endPosition = _layout.GetNodePosition(Edge.Node2.Id, Time);
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    public void RefreshWidth()
    {
        float width = _style.GetEdgeWidth(Edge, Time);
        lineRenderer.startWidth  = width;
        lineRenderer.endWidth = width;
    }
    public void RefreshColor()
    {
        //TODO: not implented yet
    }



    public void RenderDirectionArrow()
    {
        //TODO: idk if the direction is correct
        if (Edge.Node1.DataSnapshots[Time].Power>0) //flowing from Node1 to Node2 
        {
            Vector3 direction = (endPosition - startPosition).normalized;
            float size = _style.GetNodeSize(Edge.Node2, Time);
            directionArrow = Instantiate(arrowPrefab, endPosition - direction * size, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            float width = _style.GetEdgeWidth(Edge, Time);
            directionArrow.transform.localScale = new Vector3 (4*width, 5*width, 4*width);
            directionArrow.name = "Arrow_" + Edge.Id;
        }
        else if (Edge.Node1.DataSnapshots[Time].Power<0) //flowing from Node2 to Node1
        {
            Vector3 direction = (startPosition - endPosition).normalized;
            float size = _style.GetNodeSize(Edge.Node1, Time);
            directionArrow = Instantiate(arrowPrefab, startPosition - direction * size, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            float width = _style.GetEdgeWidth(Edge, Time);
            directionArrow.transform.localScale = new Vector3 (4*width, 5*width, 4*width);
            directionArrow.name = "Arrow_" + Edge.Id;
        }
        else
        {
            // No power flow, do not instantiate an arrow
            // If there is no flow we could change the color of the line
            Debug.LogWarning("No power flow in Edge_" + Edge.Id);
        }

    }

    public void RefreshDirectionArrow()
    {   
        if (Edge.Node1.DataSnapshots[Time].Power>0)
        {
            Vector3 direction = (endPosition - startPosition).normalized;
            float size = _style.GetNodeSize(Edge.Node2, Time);
            directionArrow.transform.position = endPosition - direction * size; 
            directionArrow.transform.rotation = Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0);
            
        }
        else if (Edge.Node1.DataSnapshots[Time].Power<0) //flowing from Node2 to Node1
        {
            Vector3 direction = (startPosition - endPosition).normalized;
            float size = _style.GetNodeSize(Edge.Node1, Time);
            directionArrow.transform.position = startPosition - direction * size;
            directionArrow.transform.rotation = Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0);
        }
        
    }
    


    
}
