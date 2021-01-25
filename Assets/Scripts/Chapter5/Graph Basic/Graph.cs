using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Graph<TNode, TEdge> 
    where TNode : GraphNode
    where TEdge : GraphEdge
{
    protected HashSet<TNode> nodes;
    protected HashSet<TEdge> edges;

    public HashSet<TNode> Nodes { get => nodes; set => nodes = value; }
    public HashSet<TEdge> Edges { get => edges; set => edges = value; }
    protected bool isDigraph;

    public Graph(bool isDigraph)
    {
        Nodes = new HashSet<TNode>();
        Edges = new HashSet<TEdge>();
        this.isDigraph = isDigraph;
    }

    public virtual void AddNode(TNode node) => Nodes.Add(node);
    public virtual void AddEdge(TEdge edge) => Edges.Add(edge);
    public virtual void RemoveEdge(TEdge edge) => Edges.Remove(edge);
    public virtual void RemoveNode(TNode node) => Nodes.Remove(node);

}
