using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


public class NodeVisualizer : MonoBehaviour
{
    
    [SerializeField] public GameObject nodeIDLabelGO;
    [SerializeField] private TextMeshPro nodeIDLabel;

    [SerializeField] private GameObject generatorCylinderPrefab;
    [SerializeField] private Material maxPowerMaterial;
    [SerializeField] private Material currentPowerMaterial;
    [SerializeField] private Material loadMaterial;

    public Node Node {get; private set;}
    public NodeSnapshot Snapshot { get; private set; }
    public TimeSpan Time { get; private set; }
    public int TimeStepIndex { get; private set; }
    private GraphLayout _layout;
    private GraphStyle _style;
    private MaterialPropertyBlock _propertyBlock; 
    private MeshRenderer _renderer;  

    private GameObject _generatorMaxCylinder;
    private GameObject _generatorCurrentCylinder;
    private bool _generatorCylindersCreated => _generatorMaxCylinder != null && _generatorCurrentCylinder != null;
    private const float DefaultCylinderHeight = 2f;
    private GameObject _loadCylinder;
    private bool _loadCylindersCreated => _loadCylinder != null;
    private float load;
    


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
        if (_generatorCylindersCreated)
        {
            UpdateGeneratorCylinders();
        }
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
    

    // Generator Power Visualization

    public void ShowGeneratorPower(bool show)
    {

        if (show)
        {
            if(Node.IsGenerator)
            {
                CreateGeneratorCylinders();
                UpdateGeneratorCylinders();
            }
            
            CreateLoadCylinders();
            UpdateLoadCylinders();

            SetCylindersVisible(true);
        }
        else
        {
            SetCylindersVisible(false);
        }
    }

    private void CreateGeneratorCylinders()
    {
        if (_generatorCylindersCreated)
            return;


        _generatorMaxCylinder = Instantiate(generatorCylinderPrefab, transform);
        _generatorMaxCylinder.name = "GeneratorMaxCylinder";

        _generatorCurrentCylinder = Instantiate(generatorCylinderPrefab, transform);
        _generatorCurrentCylinder.name = "GeneratorCurrentPower";

        if (_generatorMaxCylinder.TryGetComponent<MeshRenderer>(out var maxRenderer) &&
            maxPowerMaterial != null)
        {
            maxRenderer.material = maxPowerMaterial;
        }

        if (_generatorCurrentCylinder.TryGetComponent<MeshRenderer>(out var currentRenderer) &&
            currentPowerMaterial != null)
        {
            currentRenderer.material = currentPowerMaterial;
        }

        _generatorMaxCylinder.transform.localPosition = Vector3.zero;
        _generatorCurrentCylinder.transform.localPosition = Vector3.zero;
        
    }

    private void UpdateGeneratorCylinders()
    {
        if (!_generatorCylindersCreated || Snapshot.GeneratorData == null)
        return;

        float maxHeight = ConvertPowerToHeight(Snapshot.GeneratorData.MaxPower);
        float currentHeight = ConvertPowerToHeight(Snapshot.GeneratorData.Power);

        float maxScaleY = maxHeight * 0.5f;
        float currentScaleY = currentHeight * 0.5f;

        _generatorMaxCylinder.transform.localScale = new Vector3(0.5f, maxScaleY, 0.5f);
        _generatorCurrentCylinder.transform.localScale = new Vector3(0.5f, currentScaleY, 0.5f);

        float nodeTopLocalY = transform.localScale.y * 0.5f;
        float gap = 0.15f;
        float bottomLocalY = nodeTopLocalY + gap;
        float nodeOffset = 0.75f;

        _generatorMaxCylinder.transform.localPosition = new Vector3(0f, nodeOffset + bottomLocalY + maxHeight * 0.5f, 0f);

        _generatorCurrentCylinder.transform.localPosition = new Vector3(0f, nodeOffset + bottomLocalY + currentHeight * 0.5f, 0f);
    }

    private float ConvertPowerToHeight(float powerValue)
    {
        return Mathf.Clamp(powerValue * 0.01f, 0.05f, 2f);
    }


    private void SetCylindersVisible(bool visible)
    {
        if (!_generatorCylindersCreated && _loadCylindersCreated)
            return;
        
        if (_generatorCylindersCreated)
        {
            _generatorMaxCylinder.SetActive(visible);
            _generatorCurrentCylinder.SetActive(visible);
        }

        if(_loadCylindersCreated)
        {
            _loadCylinder.SetActive(visible);
        }
    }

    private void CreateLoadCylinders()
    {
        if (_loadCylindersCreated)
            return;

        //no load
        if (Node.DataSnapshots[Time].Power == 0)
            return;


        _loadCylinder = Instantiate(generatorCylinderPrefab, transform);
        _loadCylinder.name = "LoadCylinder";
        Debug.Log("instantiated load" + _loadCylinder);

        if (_loadCylinder.TryGetComponent<MeshRenderer>(out var loadRenderer) &&
            loadMaterial != null)
        {
            loadRenderer.material = loadMaterial;
        }

        _loadCylinder.transform.localPosition = Vector3.zero;
        
    }

    private void UpdateLoadCylinders()
    {
        if (!_loadCylindersCreated)
            return;

        if (Node.IsGenerator)
        {
            load = Snapshot.GeneratorData.Power + Node.DataSnapshots[Time].Power;
        }
        else
        {
            load = Node.DataSnapshots[Time].Power;
        }

        float loadHeight = ConvertPowerToHeight(load);

        float loadScaleY = loadHeight * 0.5f;

        _loadCylinder.transform.localScale = new Vector3(0.5f, loadScaleY, 0.5f);

        float nodeTopLocalY = transform.localScale.y * 0.5f;
        float gap = 0.15f;
        float bottomLocalY = nodeTopLocalY + gap;
        float nodeOffset = 0.75f;
        float loadOffset = 0.75f;

        _loadCylinder.transform.localPosition = new Vector3(loadOffset, nodeOffset + bottomLocalY + loadHeight * 0.5f, 0f);

    }
}
