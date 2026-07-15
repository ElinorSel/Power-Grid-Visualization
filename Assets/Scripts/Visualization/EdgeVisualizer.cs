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

    private GraphLayout _layout;
    private GraphStyle _style; 


    public void Initialize(Edge data,TimeSpan time, int timeStepIndex, GraphLayout layout, GraphStyle style, Material edgeMaterial)
    { 
        Edge = data;
        Time = time;
        
        _layout = layout;
        _style = style; 
        
        TimeStepIndex = timeStepIndex;

        startPosition = layout.GetInitialNodePosition(Edge.Node1.DataSnapshots[time], timeStepIndex);
        endPosition = layout.GetInitialNodePosition(Edge.Node2.DataSnapshots[time], timeStepIndex);
        //RenderEdge(style.GetEdgeWidth(Edge, time), edgeMaterial);
        //Direction();  
        
    }

    void RenderEdge(float edgeWidth, Material edgeMaterial)
    {
        // Add a LineRenderer component
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

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
    


    
}
