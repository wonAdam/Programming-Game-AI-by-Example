using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DFSPathfinder : Pathfinder
{

    public override IEnumerator Search(Graph<GraphNode, GraphEdge> graph, ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();

        Stack<PathSnapShot> stack = new Stack<PathSnapShot>();
        input.start.OutEdges.ForEach(e => stack.Push(new PathSnapShot(e.To, new List<GraphNode>() { input.start })));

        int count = 0;
        int calcPerCount = 100;
        while (stack.Count > 0)
        {
            PathSnapShot currPathSnapShot = stack.Pop();
            GraphNode currNode = currPathSnapShot.currNode;
            List<GraphNode> currPath = currPathSnapShot.path;
            Debug.Log("currNode " + currNode.gameObject.name);

            currPath.Add(currPathSnapShot.currNode);

            // success
            if (currPathSnapShot.currNode == input.destination)
            {
                Debug.Log("Pathfind Success");
                OnSearchEnd();
                OnServe(currPathSnapShot.path);
                yield break;
            }

            currNode.OutEdges
                .Where(e => !currPath.Contains(e.To))
                .ToList().ForEach(e => stack.Push(new PathSnapShot(e.To, new List<GraphNode>(currPath))));

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
