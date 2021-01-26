﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfiner : Pathfinder
{
    public class Node
    {
        public readonly GraphNode node;
        public Node prevNode;
        public int gCost;
        public int hCost;
        public int fCost { get => hCost + gCost; }

        public Node(GraphNode node, Node node_via, int gCost, GraphNode destinationNode)
        {
            this.node = node;
            this.prevNode = node_via;
            this.gCost = gCost;
            this.hCost = (int)Vector2.Distance(node.transform.position, destinationNode.transform.position);
        }
    }

    
    public override IEnumerator Search(Graph<GraphNode, GraphEdge> graph, ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();

        List<Node> openList = new List<Node>();
        openList.Add(new Node(input.start, null, 0, input.destination));
        List<Node> closedList = new List<Node>();

        while (openList.Count > 0)
        {
            Node currNode = openList.Aggregate((accum, curr) => accum.fCost > curr.fCost ? curr : accum);
            Debug.Log("currNode " + currNode.node.gameObject.name);


            if (currNode.node == input.destination)
            {
                List<GraphNode> path = new List<GraphNode>() { currNode.node };
                Node curr = currNode.prevNode;
                while(curr != null)
                {
                    path.Add(curr.node);
                    curr = curr.prevNode;
                }
                path.Reverse();
                OnServe(path);
                OnSearchEnd();
                yield break;
            }

            openList.Remove(currNode);
            closedList.Add(currNode);

            foreach(var edge in currNode.node.OutEdges)
            {
                GraphNode neighbor = edge.To;
                if (closedList.Any((node) => node.node == neighbor)) continue;
                if (openList.Any((node) => node.node == neighbor)) continue;

                Node neighborNode = new Node(neighbor, currNode, currNode.gCost + edge.Cost, input.destination);
                openList.Add(neighborNode);
            }

             yield return null;
        }

        OnServe(null);
        OnSearchEnd();
    }

}
