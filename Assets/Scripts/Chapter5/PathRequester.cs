using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequester: MonoBehaviour
{
    [SerializeField] BFSPathfinder _BFSPathfinder;
    [SerializeField] DFSPathfinder _DFSPathfinder;
    public enum RequesterState { Idle, Wait }
    public RequesterState rState = RequesterState.Idle;
    public enum Algorithm { BFS, DFS, Djikstra, AStar }
    public Algorithm algorithm;
    public void Request(GraphNode start, GraphNode destination, Action<List<GraphNode>> OnServe)
    {
        switch (algorithm)
        {
            case Algorithm.BFS:
                _BFSPathfinder.Request(this, new BFSPathfinder.ReqInput(start, destination), OnServe);
                break;
            case Algorithm.DFS:
                _DFSPathfinder.Request(this, new DFSPathfinder.ReqInput(start, destination), OnServe);
                break;
            case Algorithm.Djikstra:

                break;
            case Algorithm.AStar:

                break;
            default:
                break;
        
        }

    }
}
