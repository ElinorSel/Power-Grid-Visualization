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
    private MaterialPropertyBlock _propertyBlock;
    private MaterialPropertyBlock _propertyBlockArrow;
    private MeshRenderer _arrowRenderer;  

    private LineRenderer lineRenderer;
    [SerializeField] private GameObject arrowPrefab;
    private Transform arrowTip;

    private GraphLayout _layout;
    private GraphStyle _style; 

    private GameObject directionArrow;

    private int _bundleIndex;
    private int _bundleSize;
    private float _offset;


    public void Initialize(Edge data,TimeSpan time, int timeStepIndex, GraphLayout layout, GraphStyle style, Material edgeMaterial, int bundleIndex, int bundleSize)
    { 
        Edge = data;
        Time = time;
        
        _layout = layout;
        _style = style; 
        
        TimeStepIndex = timeStepIndex;

        _bundleIndex = bundleIndex;
        _bundleSize = bundleSize;


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

        //Add a property block so line renderers can share one material instance
         _propertyBlock = new MaterialPropertyBlock();
         _propertyBlockArrow = new MaterialPropertyBlock();

        lineRenderer.GetPropertyBlock(_propertyBlock);

        _propertyBlock.SetFloat("_EdgeLoad", _style.GetEdgeLoad(Edge.DataSnapshots[Time]));

        lineRenderer.SetPropertyBlock(_propertyBlock);


        // Set the color
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // Set the width
        lineRenderer.startWidth = edgeWidth;
        lineRenderer.endWidth = edgeWidth;

        // Set the number of vertices
        lineRenderer.positionCount = 2;

        //Offset for when node pairs have multiple connections
        Vector3 offset = CalculateOffset();
        // Set the positions of the vertices
        lineRenderer.SetPosition(0, startPosition + offset);
        lineRenderer.SetPosition(1, endPosition + offset);

            
    } 

    public void RefreshPosition()
    {
        startPosition = _layout.GetNodePosition(Edge.Node1.Id, Time);
        endPosition = _layout.GetNodePosition(Edge.Node2.Id, Time);

        Vector3 offset = CalculateOffset();
        lineRenderer.SetPosition(0, startPosition + offset);
        lineRenderer.SetPosition(1, endPosition + offset);
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
        if (Edge.DataSnapshots[Time].Direction>0) //flowing from Node1 to Node2 
        {
    
            Vector3 direction = (endPosition - startPosition).normalized;

            directionArrow = Instantiate(arrowPrefab, endPosition, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            directionArrow.name = "Arrow_" + Edge.Id;

            Vector3 nodePosition = endPosition;
            float nodeSize = _style.GetNodeSize(Edge.Node1, Time);
            UpdateDirectionArrow(nodePosition, direction, nodeSize);

        }

        else if (Edge.DataSnapshots[Time].Direction<0) //flowing from Node2 to Node1
        {
            Vector3 direction = (startPosition - endPosition).normalized;
            
            directionArrow = Instantiate(arrowPrefab, startPosition, Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0), transform);
            directionArrow.name = "Arrow_" + Edge.Id;

            Vector3 nodePosition = startPosition;
            float nodeSize = _style.GetNodeSize(Edge.Node1, Time);
            UpdateDirectionArrow(nodePosition, direction, nodeSize);

        }
        else
        {
            // No power flow, do not instantiate an arrow
            // If there is no flow we could change the color of the line
            Debug.LogWarning("No power flow in Edge_" + Edge.Id + "with power of" + Edge.Node1.DataSnapshots[Time].Power);
        }

        if(directionArrow != null)
         {
            _arrowRenderer = directionArrow.GetComponent<MeshRenderer>();
            _arrowRenderer.GetPropertyBlock(_propertyBlockArrow);
            _propertyBlockArrow.SetFloat("_EdgeLoad", _style.GetEdgeLoad(Edge.DataSnapshots[Time]));
            _arrowRenderer.SetPropertyBlock(_propertyBlockArrow);
         }


    }

    public void RefreshDirectionArrow()
    {   
        if (directionArrow == null)
        {
            return;
        }
        if (Edge.DataSnapshots[Time].Direction>0) //flowing from Node1 to Node2
        {
            Vector3 nodePosition = endPosition;
            Vector3 direction = (endPosition - startPosition).normalized;
            float nodeSize = _style.GetNodeSize(Edge.Node1, Time);
            UpdateDirectionArrow(nodePosition, direction, nodeSize);

        }
        else if (Edge.DataSnapshots[Time].Direction<0) //flowing from Node2 to Node1
        {
            Vector3 nodePosition = startPosition;
            Vector3 direction = (startPosition - endPosition).normalized;
            float nodeSize = _style.GetNodeSize(Edge.Node1, Time);
            UpdateDirectionArrow(nodePosition, direction, nodeSize);

        }
        
    }

    private void UpdateDirectionArrow(Vector3 nodePosition, Vector3 direction, float nodeSize)
    {
        arrowTip = directionArrow.transform.Find("Tip");
        if (arrowTip == null)
        {
            Debug.LogError($"Arrow prefab for Edge_{Edge.Id} is missing a 'Tip' child.");
            return;
        }

        float width = _style.GetEdgeWidth(Edge, Time);
        directionArrow.transform.localScale = new Vector3 (4*width, 5*width, 4*width);

        float tipDistance = Vector3.Dot(arrowTip.position - directionArrow.transform.position, direction);

        float nodeRadius = Mathf.Max(nodeSize * 0.5f, 0.05f);

        float offset = nodeRadius + tipDistance;
        Vector3 edgeOffset = CalculateOffset();

        directionArrow.transform.position = (nodePosition - direction * offset) - edgeOffset;
        directionArrow.transform.rotation = Quaternion.LookRotation(direction)*Quaternion.Euler(90,0,0);

    }
    
    private Vector3 CalculateOffset()
    {
        //if there is only one edge, there is no offset
        if (_bundleSize<=1) 
        {
            return Vector3.zero;
        }

        Vector3 direction = (endPosition - startPosition);
        if (direction == Vector3.zero)
        {
            return Vector3.zero;
        }

        direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        float spacing = 0.2f;
        float offsetAmount = (_bundleIndex - (_bundleSize - 1) / 2f) * spacing;

        return perpendicular * offsetAmount;
        
    }

    
}
