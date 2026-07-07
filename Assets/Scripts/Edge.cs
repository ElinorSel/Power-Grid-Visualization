using UnityEngine;
using System.Collections.Generic;

public class Edge : MonoBehaviour
{
    public string id;
    public string type;
    public Node node1;
    public Node node2;
    public string voltageLevel1id;
    public string voltageLevel2id;
    public bool isConnected1;
    public bool isConnected2;
    public float power;
    public float reactivePower;
    public float current;
    public float normalMVALimit;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
        
    }

    public void DebugPrintData()
    {
        Debug.Log("Edge ID: " + id + ", Edge Type: " + type + ", Voltage Level 1 ID: " + voltageLevel1id + "Is Connected 1: " + isConnected1 + ", Voltage Level 2 ID: " + voltageLevel2id + ", Is Connected 2: " + isConnected2 + ", Power: " + power + ", Reactive Power: " + reactivePower + ", Current: " + current + ", Normal MVA Limit: " + normalMVALimit);

    }
}
