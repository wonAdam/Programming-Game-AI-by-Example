using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphEdge
{
    protected GraphNode from;
    protected GraphNode to;
    protected int cost;
    public GraphNode From { get => from; set => from = value; }
    public GraphNode To { get => to; set => to = value; }
    public int Cost { get => cost; set => cost = value; }

    public GraphEdge(GraphNode from, GraphNode to, int cost)
    {
        From = from;
        To = to;
        Cost = cost;
    }

}
