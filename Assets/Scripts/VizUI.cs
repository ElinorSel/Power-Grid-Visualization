using UnityEngine;
using TMPro;

public class VizUI : MonoBehaviour
{
    [SerializeField] TMP_Text minLabel;
    [SerializeField] TMP_Text maxLabel;
   public void ToggleLabels()
    {
        Debug.Log("VIZ UI: trying to sett labels to " + !VisualizationSettings.Instance.ShowLabels);
        VisualizationSettings.Instance.SetShowLabels(!VisualizationSettings.Instance.ShowLabels);
    }

    public void SetLayoutInitial()
    {
        VisualizationSettings.Instance.SetNodeLayoutAlgorithm(
            VisualizationSettings.NodeLayoutAlgorithOption.InitialData);
    }

    public void SetLayoutForce()
    {
        VisualizationSettings.Instance.SetNodeLayoutAlgorithm(
            VisualizationSettings.NodeLayoutAlgorithOption.ForceDirected);
    }

    public void SetHeightScaleFactor(float value)
    {
        VisualizationSettings.Instance.SetHeightScaleFactor(value);
    }
        public void SetSizeFactor(float value)
    {
        VisualizationSettings.Instance.SetSizeScaleFactor(value);
    }
    public void SetTimeStepZSize(float value)
    {
        VisualizationSettings.Instance.SetTimeStepZSize(value);
    }
    public void SetEdgeWidthScaleFactor(float value)
    {
        VisualizationSettings.Instance.SetEdgeWidthScaleFactor(value);
    }

    public void SetSizeScaleFactor(float value)
    {
        VisualizationSettings.Instance.SetSizeScaleFactor(value);
    }

    public void SetTimeRange(float minValue, float maxValue)
    {
        Debug.Log("On values changed min: " + minValue + "max: " + maxValue);
        
    }

    public void UpdateTimeRangeMinLabel(float value)
    {
        minLabel.text = value.ToString();
    }

    public void UpdateTimeRangeMaxLabel(float value)
    {
        maxLabel.text = value.ToString();
    }



}
