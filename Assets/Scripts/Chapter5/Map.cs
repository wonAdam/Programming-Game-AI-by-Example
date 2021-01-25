using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : Graph<GraphNode, GraphEdge>
{
    public Map(bool isDigraph) : base(isDigraph) {}

}
