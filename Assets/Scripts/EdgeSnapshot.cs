using UnityEngine;

public class EdgeSnapshot
{
    public float  Load {get; }
    public float power_from {get;}
    public float power_to {get;}
    public float direction {get; set;} //TODO: change datatype later if needed

    public EdgeSnapshot(float load, float p_from, float p_to)
    {
        this.Load = load;
        this.power_from = p_from;
        this.power_to = p_to;
        this.direction = p_from - p_to; 
    }
}
