using UnityEngine;

public class NodeSnapshot
{
    public float Power {get; }
    public float VAngle  {get; }
    public Vector2 Coordinates {get; set;}
    
    public float ZOffset {get; set;} //not from data CSV and is soley for vixualization purposes, to offset the node in the z direction based on its voltage angle

    public NodeSnapshot(float power, float vAngle)
    {
        this.Power = power;
        this.VAngle = vAngle;
        this.ZOffset = 0f; 
        this.Coordinates = Vector2.zero; //no more random coordinates >:() new Vector2(UnityEngine.Random.Range(-20f,20f), UnityEngine.Random.Range(-20f,20f));
    }

}
