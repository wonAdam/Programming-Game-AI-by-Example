using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DFSPathfinder : Pathfinder
{

    public override IEnumerator Search(ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();

        Stack<PathSnapShot> stack = new Stack<PathSnapShot>();
        input.start.OutEdges.ForEach(e => stack.Push(new PathSnapShot(e.To, new List<GraphNode>() { input.start })));

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
                OnEnd();
                OnServe(currPathSnapShot.path);
                yield break;
            }

            currNode.OutEdges
                .Where(e => !currPath.Contains(e.To))
                .ToList().ForEach(e => stack.Push(new PathSnapShot(e.To, new List<GraphNode>(currPath))));
        }

        OnSearchEnd();
        OnServe(null);

    }

}
