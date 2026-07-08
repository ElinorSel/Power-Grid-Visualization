using UnityEngine;

public class EdgeVisualizer : MonoBehaviour
{
    public Edge Data {get; set;}
    private Vector3 startPosition;
    private Vector3 endPosition;

    [SerializeField] private float width = 0.5f;

    public void Initialize(Edge data)
    {
        Data = data;
        startPosition = new Vector3(data.Node1.Coordinates.x, 0, data.Node1.Coordinates.y);
        endPosition = new Vector3(data.Node2.Coordinates.x, 0, data.Node2.Coordinates.y);
        RenderEdge();
        
    }

    void RenderEdge()
    {
        // Add a LineRenderer component
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

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
    
}
