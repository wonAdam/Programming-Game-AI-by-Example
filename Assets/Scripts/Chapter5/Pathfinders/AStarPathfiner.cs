using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfiner : Pathfinder
{
    public class Node : IComparable<Node>
    {
        public readonly GraphNode node;
        public Node prevNode;
        public int gCost;
        public float hCost;
        public float fCost { get => hCost + gCost; }

        public Node(GraphNode node, Node node_via, int gCost, GraphNode destinationNode)
        {
            this.node = node;
            this.prevNode = node_via;
            this.gCost = gCost;
            this.hCost = Vector2.Distance(node.transform.position, destinationNode.transform.position);
        }


        public int CompareTo(Node other)
        {
            return (int)(fCost - other.fCost);
        }
    }

    public class ByFCost : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            int result = x.CompareTo(y);
            return result == 0 ? 1 : result;
        }
    }


    public override IEnumerator Search(Graph<GraphNode, GraphEdge> graph, ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();

        LinkedList<Node> openList = new LinkedList<Node>();
        Node start = new Node(input.start, null, 0, input.destination);
        openList.AddLast(start);
        List<Node> closedList = new List<Node>();

        int count = 0;
        int calcPerCount = 100;
        while (openList.Count > 0)
        {
            Node currNode = openList.First();
            openList.RemoveFirst();
            //Node currNode = openList.Aggregate((accum, curr) => accum.fCost > curr.fCost ? curr : accum);
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

            closedList.Add(currNode);

            foreach(var edge in currNode.node.OutEdges)
            {
                GraphNode neighbor = edge.To;
                if (closedList.Any((node) => node.node == neighbor)) continue;
                if (openList.Any((node) => node.node == neighbor)) continue;

                Node neighborNode = new Node(neighbor, currNode, currNode.gCost + edge.Cost, input.destination);

                bool insert = false;
                for(var it = openList.First; it != null; it = it.Next)
                {
                    if (it.Value.fCost > neighborNode.fCost)
                    {
                        openList.AddBefore(it, neighborNode);
                        insert = true;
                        break;
                    }
                }
                if(insert == false)
                    openList.AddLast(neighborNode);
            }

            count++;
            if (count >= calcPerCount)
            {
                count = 0;
                yield return null;

            }
        }

        OnServe(null);
        OnSearchEnd();
    }

}
