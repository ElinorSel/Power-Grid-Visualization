using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    private string voltageLevelId;

    private float vMagnitude;

    private Vector2 coordinates;

    [SerializeField] float vAngle = 0;
    [SerializeField] public List<Edge> edges;

}
