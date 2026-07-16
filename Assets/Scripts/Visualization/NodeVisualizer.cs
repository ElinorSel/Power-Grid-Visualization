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

        // [Show Labels]
        nodeIDLabel.text = Node.Id;
        nodeIDLabelGO.SetActive(VisualizationSettings.Instance.ShowLabels);

    }

    public void RefreshPosition()
    {
        transform.position = _layout.GetNodePosition(Node.Id, Time);
    }

    public void RefreshStyling()
    {
        transform.localScale = Vector3.one * _style.GetNodeSize(Node, Time);
    }
    
    public void RefreshLabel()
    {
        nodeIDLabelGO.SetActive(VisualizationSettings.Instance.ShowLabels);
    }
}
