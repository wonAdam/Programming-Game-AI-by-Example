using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pathfinder : MonoBehaviour
{
    public struct ReqInput
    {
        public GraphNode start;
        public GraphNode destination;
        public ReqInput(GraphNode start, GraphNode destination)
        {
            this.start = start;
            this.destination = destination;
        }
    }

    public struct PathSnapShot
    {
        public GraphNode currNode;
        public List<GraphNode> path;
        public PathSnapShot(GraphNode currNode, List<GraphNode> path)
        {
            this.currNode = currNode;
            this.path = path;
        }
    }
    public enum FinderState { Idle, Process }
    public FinderState sState = FinderState.Idle;

    public virtual void Request(PathRequester requester, ReqInput input, Action<List<GraphNode>> OnServe)
    {
        StartCoroutine(Search(input, OnServe));
    }

    protected void OnSearchBegin() => sState = FinderState.Process;
    protected void OnSearchEnd() => sState = FinderState.Idle;

    public abstract IEnumerator Search(ReqInput input, Action<List<GraphNode>> OnServe);
}
