using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequester: MonoBehaviour
{
    [SerializeField] BFSPathfinder _BFSPathfinder;
    [SerializeField] DFSPathfinder _DFSPathfinder;
    [SerializeField] DijkstraPathfinder _DijstraPathfinder;
    public enum Algorithm { BFS, DFS, Dijkstra, AStar }
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
            case Algorithm.Dijkstra:
                _DijstraPathfinder.Request(this, new DijkstraPathfinder.ReqInput(start, destination), OnServe);
                break;
            case Algorithm.AStar:

                break;
            default:
                break;
        
        }

    }
}
