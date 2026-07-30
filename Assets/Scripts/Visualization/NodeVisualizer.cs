using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class NodeVisualizer : MonoBehaviour
{
    
    [SerializeField] public GameObject nodeIDLabelGO;
    [SerializeField] private TextMeshPro nodeIDLabel;

    public Node Node {get; private set;}
    public NodeSnapshot Snapshot { get; private set; }
    public TimeSpan Time { get; private set; }
    public int TimeStepIndex { get; private set; }
    private GraphLayout _layout;
    private GraphStyle _style;
    private MaterialPropertyBlock _propertyBlock; 
    private MeshRenderer _renderer;  


    //reads the style and layout data, and then renders the node / edge from this data
    public void Initialize(Node data, TimeSpan time, int timeStepIndex, GraphLayout layout, GraphStyle style)
    {
        
        Node = data;
        Time = time;
  
        _layout = layout;
        _style = style; 

        Snapshot = Node.DataSnapshots[time]; //TODO: can be removed
        TimeStepIndex = timeStepIndex; //TODO: can be removed

        transform.position = layout.GetNodePosition(data.Id, time);   
        transform.localScale = Vector3.one * _style.GetNodeSize(data, time);

        //Add a property block so nodes can share one material instance
         _propertyBlock = new MaterialPropertyBlock();
         _renderer = GetComponent<MeshRenderer>();
         RefreshNodeColor(); //set color based on vAngle

        

        // [Show Labels]
        //Node ID Label needs +1 because the node ID is 0-indexed but the label is 1-indexed
        int nodeIDINT = int.Parse(Node.Id); //the data had wrong indicies so we add +1
        nodeIDINT++;
        string nodeID = nodeIDINT.ToString();

        nodeIDLabel.text = "" + nodeID;
        nodeIDLabelGO.SetActive(VisualizationSettings.Instance.ShowLabels);

    }

    public void RefreshPosition()
    {
        transform.position = _layout.GetNodePosition(Node.Id, Time);
    }

    public void RefreshNodeSize()
    {
        transform.localScale = Vector3.one * _style.GetNodeSize(Node, Time);
    }
    
    public void RefreshLabel()
    {
        Debug.Log("Node visualiser: Setting labels to " + VisualizationSettings.Instance.ShowLabels);
        nodeIDLabelGO.SetActive(VisualizationSettings.Instance.ShowLabels);
    }
    public void RefreshNodeColor()
    {
        //TODO: not implemented yet

        float nodeAngle = _style.GetNodeAngle(Node.DataSnapshots[Time]);
        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat("_NodeAngle", nodeAngle);
        _renderer.SetPropertyBlock(_propertyBlock);

    }
    
}
