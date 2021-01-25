using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BFSPathfinder : Pathfinder
{
    public override IEnumerator Search(ReqInput input, Action<List<GraphNode>> OnServe, Action OnBegin, Action OnEnd)
    {
        OnBegin();

        Queue<PathSnapShot> queue = new Queue<PathSnapShot>();
        input.start.OutEdges.ForEach(e => queue.Enqueue(new PathSnapShot(e.To, new List<GraphNode>() { input.start })));

        while (queue.Count > 0)
        {
            PathSnapShot currPathSnapShot = queue.Dequeue();
            GraphNode currNode = currPathSnapShot.currNode;
            List<GraphNode> currPath = currPathSnapShot.path;
            Debug.Log("currNode " + currNode.gameObject.name);

            currPath.Add(currPathSnapShot.currNode);

            if (currPathSnapShot.currNode == input.destination)
            {
                sState = FinderState.Idle;
                OnServe(currPathSnapShot.path);
                Debug.Log("Pathfind Success");
                yield break ;
            }

            currNode.OutEdges
                .Where(e => !currPath.Contains(e.To))
                .ToList().ForEach(e => queue.Enqueue(new PathSnapShot(e.To, new List<GraphNode>(currPath))));

            yield return null;
        }

        OnEnd();
        OnServe(null);
    }

}
