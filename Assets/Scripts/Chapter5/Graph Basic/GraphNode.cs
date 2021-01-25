using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphNode : MonoBehaviour
{

    [SerializeField] protected List<GraphEdge> outEdges = new List<GraphEdge>();
    [SerializeField] protected List<GraphEdge> inEdges = new List<GraphEdge>();
    public List<GraphEdge> OutEdges { get => outEdges; set => outEdges = value; }
    public List<GraphEdge> InEdges { get => outEdges; set => outEdges = value; }
}
