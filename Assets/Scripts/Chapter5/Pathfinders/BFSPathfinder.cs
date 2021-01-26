using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BFSPathfinder : Pathfinder
{
    public override IEnumerator Search(ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();

        Queue<PathSnapShot> queue = new Queue<PathSnapShot>();
        input.start.OutEdges.ForEach(e => queue.Enqueue(new PathSnapShot(e.To, new List<GraphNode>() { input.start })));

        int calcPerFrame = 100;
        int count = 0;
        while (queue.Count > 0)
        {
            PathSnapShot currPathSnapShot = queue.Dequeue();
            GraphNode currNode = currPathSnapShot.currNode;
            List<GraphNode> currPath = currPathSnapShot.path;
            Debug.Log("currNode " + currNode.gameObject.name + " / " + queue.Count);

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


            count++;
            if(calcPerFrame <= count)
            {
                calcPerFrame = 0;
                yield return null;
            }
        }

        OnSearchEnd();
        OnServe(null);
    }

}
