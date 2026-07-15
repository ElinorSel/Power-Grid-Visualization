using UnityEngine;
using System;
public class GraphStyle 
{
    /*
    could have these variables if we later make a class for nodestyle and edge style
    public float NodeSize;
    public float NodeHeight;
    public Color NodeColor;

    public float EdgeWidth;
    public Color EdgeColor;
    */

    //This class contains all visual styling calculations derived from the data.
    //The node and edge visualizer class only needs to read this class to know what to render.



    //______NODE SETTINGS_____
    public float GetNodeSize(Node node, TimeSpan time)
    {
        //[Size Mapping]
        switch (VisualizationSettings.Instance.NodeSizeMapping)
        {
            case VisualizationSettings.NodeSizeMappingOption.None:
                return VisualizationSettings.Instance.NodeSizeScaleFactor;
            case VisualizationSettings.NodeSizeMappingOption.VoltageMagnitude:
                return CalculateSizeMappingVoltageMagnitude(node, time);
            case VisualizationSettings.NodeSizeMappingOption.VoltageAngle:
                return CalculateSizeMappingVoltageAngle(node, time);
            default:
                Debug.LogWarning("Unknown / Unimplemented size mapping option.");
                return 0f;
        }
    }

    public float GetNodeHeight(NodeSnapshot nodeSnapshot, int timeStepIndex)
    {
        // [Height Mapping]
        switch (VisualizationSettings.Instance.NodeHeightMapping)
        {
            case VisualizationSettings.NodeHeightMappingOption.None:
                return 0f + VisualizationSettings.Instance.TimeStepZSize * timeStepIndex;
            case VisualizationSettings.NodeHeightMappingOption.VoltageAngle:
                return CalculateZOffsetVoltageAngle(nodeSnapshot) +  VisualizationSettings.Instance.TimeStepZSize  * timeStepIndex;
            default:
                Debug.LogWarning("Unknown / Unimplementedheight mapping option for Node Height Offset.");
                return 0f;
        }
    }

    //______EDGE SETTINGS_____
    public float GetEdgeWidth(Edge edge, TimeSpan time)
    {
        // [Width Settings]
        switch (VisualizationSettings.Instance.EdgeWidthMapping)
        {
            case VisualizationSettings.EdgeWidthMappingOption.None:
                return VisualizationSettings.Instance.EdgeWidthScaleFactor; 
            case VisualizationSettings.EdgeWidthMappingOption.MVALimit:
                return CalculateWidthMVALimit(edge); 
            default:
                Debug.LogWarning("Unknown / Unimplemented width mapping option for Edges.");
                return 0f;
        }
    }

/*

    public Color GetNodeColor(Node node, TimeSpan time)
    {
    
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


    */

    //________________________NODE CALCULATIONS__________________________________



        public float CalculateZOffsetVoltageAngle(NodeSnapshot nodeSnapshot)
    {
        //TODO: Implement a more sophisticated method to calculate the zOffset
        return nodeSnapshot.VAngle * VisualizationSettings.Instance.NodeHeightScaleFactor;
    }

    public float CalculateSizeMappingVoltageMagnitude(Node node, TimeSpan time)
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        return node.DataSnapshots[time].Power * 0.01f * VisualizationSettings.Instance.NodeSizeScaleFactor;
    }

        public float CalculateSizeMappingVoltageAngle(Node node, TimeSpan time)
    {
        //TODO: Implement a more sophisticated method to calculate the sizeMapping
        return node.DataSnapshots[time].VAngle * 0.01f * VisualizationSettings.Instance.NodeSizeScaleFactor;
    }
      
    //________________________EDGE CALCULATIONS__________________________________
    float CalculateWidthMVALimit(Edge edge)
    {
        //TODO: change to suitable value
        float value = edge.MaxLoad / 120f;
        return Mathf.Pow(value, 1.3f) * VisualizationSettings.Instance.EdgeWidthScaleFactor;
    }
    

}