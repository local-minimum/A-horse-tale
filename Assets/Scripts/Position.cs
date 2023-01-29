using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    [SerializeField]
    List<Direction> edges = new List<Direction>();

    public bool HasEdge(Direction direction)
    {
        return edges.Contains(direction);
    }
}
