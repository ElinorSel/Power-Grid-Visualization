using UnityEngine;

public class EdgeVisualizer : MonoBehaviour
{
    public Edge Data {get; set;}
    private Vector3 startPosition;
    private Vector3 endPosition;

    [SerializeField] private float width = 0.5f;
    [SerializeField] private GameObject arrowPrefab;

    public void Initialize(Edge data)
    {
        Data = data;
        startPosition = new Vector3(data.Node1.Coordinates.x, data.Node1.ZOffset, data.Node1.Coordinates.y);
        endPosition = new Vector3(data.Node2.Coordinates.x, data.Node2.ZOffset, data.Node2.Coordinates.y);

        // [Width Settings]
        switch (VisualizationSettings.Instance.EdgeWidthMapping)
        {
            case VisualizationSettings.EdgeWidthMappingOption.None:
                width = VisualizationSettings.Instance.EdgeWidthScaleFactor; 
                break;
            case VisualizationSettings.EdgeWidthMappingOption.MVALimit:
                 width = CalculateWidthMVALimit(data); 
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented width mapping option for Edges.");
                break;
        }

        // [Width Settings]
        switch (VisualizationSettings.Instance.EdgeWidthMapping)
        {
            case VisualizationSettings.EdgeWidthMappingOption.None:
                width = VisualizationSettings.Instance.EdgeWidthScaleFactor; 
                break;
            case VisualizationSettings.EdgeWidthMappingOption.MVALimit:
                 width = CalculateWidthMVALimit(data); 
                break;
            default:
                Debug.LogWarning("Unknown / Unimplemented width mapping option for Edges.");
                break;
        }


        RenderEdge();
        Direction(data);
        
    }

    void RenderEdge()
    {
        // Add a LineRenderer component
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;

        // Set the material
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the color
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;

        // Set the width
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        // Set the number of vertices
        lineRenderer.positionCount = 2;

        // Set the positions of the vertices
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    } 

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

    float CalculateWidthMVALimit(Edge edge)
    {
        return (edge.NormalMVALimit / 100f) * VisualizationSettings.Instance.EdgeWidthScaleFactor; // Scale factor can be adjusted as needed
    }
    
}
