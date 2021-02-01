using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BFSPathfinder : Pathfinder
{
    public override IEnumerator Search(Graph<GraphNode, GraphEdge> graph, ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();
        Dictionary<GraphNode, bool> visited = new Dictionary<GraphNode, bool>();
        foreach(var node in graph.Nodes)
            visited.Add(node, false);

        Queue<PathSnapShot> queue = new Queue<PathSnapShot>();
        input.start.OutEdges.ForEach(e => queue.Enqueue(new PathSnapShot(e.To, new List<GraphNode>() { input.start })));

        int count = 0;
        int calcPerCount = 100;
        while (queue.Count > 0)
        {
            PathSnapShot currPathSnapShot;
            do
            {
                currPathSnapShot = queue.Dequeue();
            } while (visited[currPathSnapShot.currNode] == true);
            GraphNode currNode = currPathSnapShot.currNode;
            List<GraphNode> currPath = currPathSnapShot.path;
            visited[currNode] = true;
            
            Debug.Log("currNode " + currNode.gameObject.name + " / " + queue.Count);

            currPath.Add(currPathSnapShot.currNode);

            if (currPathSnapShot.currNode == input.destination)
            {
                OnSearchEnd();
                OnServe(currPathSnapShot.path);
                Debug.Log("Pathfind Success");
                yield break ;
            }

            currNode.OutEdges
                .Where(e => !currPath.Contains(e.To) && visited[e.To] == false)
                .ToList().ForEach(e => queue.Enqueue(new PathSnapShot(e.To, new List<GraphNode>(currPath))));

            count++;
            if (count >= calcPerCount)
            {
                count = 0;
                yield return null;

            }
        }

        OnSearchEnd();
        OnServe(null);
    }

}
