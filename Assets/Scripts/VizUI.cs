using UnityEngine;
using TMPro;
using UIRangeSliderNamespace;
using System.Runtime.Serialization;

public class VizUI : MonoBehaviour
{
    [SerializeField] TMP_Text minLabel;
    [SerializeField] TMP_Text maxLabel;
    [SerializeField] UnityEngine.UI.Toggle NodePowerToggle;
    [SerializeField] UIRangeSlider timeRangeSlider;
    [SerializeField] GameObject hideBackground;


   public void ToggleLabels()
    {
        //Debug.Log("VIZ UI: trying to sett labels to " + !VisualizationSettings.Instance.ShowLabels);
        VisualizationSettings.Instance.SetShowLabels(!VisualizationSettings.Instance.ShowLabels);
    }

    public void ToggleHideLowLoad()
    {
        VisualizationSettings.Instance.SetHideLowLoad(!VisualizationSettings.Instance.HideLowLoad);
        
    }

    public void ToggleGeneratorPower()
    {
        VisualizationSettings.Instance.SetShowGeneratorPower(!VisualizationSettings.Instance.ShowGeneratorPower);
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
        VisualizationSettings.Instance.SetTimeRange(minValue, maxValue);  
    }

    public void UpdateTimeRangeMinLabel(float value)
    {
        if (value<10)
        {
            minLabel.text = "0"+ value + ":00"; //value.ToString();
        }
        else
        {
            minLabel.text = value + ":00";
        }
    }

    public void UpdateTimeRangeMaxLabel(float value)
    {
        if (value<10)
        {
            maxLabel.text = "0"+ value + ":00"; //value.ToString();
        }
        else
        {
            maxLabel.text = value + ":00";
        }
    }

    public void HideSettings(bool show)
    {
        hideBackground.SetActive(!show);
        
    }

    public void SetSingleTimePreset(){
        VisualizationSettings.Instance.SetShowGeneratorPower(true);
        NodePowerToggle.SetIsOnWithoutNotify(true);

        SetHeightScaleFactor(0.015f);
        SetEdgeWidthScaleFactor(0.01f);
        SetSizeScaleFactor(0.09f);

        timeRangeSlider.SetValueWithoutNotify(1f, 1f);
        SetTimeRange(1f,1f);
        UpdateTimeRangeMinLabel(1f);
        UpdateTimeRangeMaxLabel(1f);
        
    }




}
