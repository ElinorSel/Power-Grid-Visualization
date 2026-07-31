using UnityEngine;

public class GeneratorSnapshot
{
    public float Power {get; }
    public float MaxPower  {get; }
    

    //for generators
    public GeneratorSnapshot GeneratorData { get; set; }
    public GeneratorSnapshot(float power, float maxPower)
    {
        Power = power;
        MaxPower = maxPower;

    }

}
