using UnityEngine;

public class EdgeSnapshot
{
    public float  Load {get; }
    public float PowerFrom {get;}
    public float PowerTo {get;}
    public float Direction {get; set;} //TODO: change datatype later if needed

    public EdgeSnapshot(float load, float p_from, float p_to)
    {
        this.Load = load;
        this.PowerFrom = p_from;
        this.PowerTo = p_to;
        this.Direction = p_from - p_to; 
    }
}
