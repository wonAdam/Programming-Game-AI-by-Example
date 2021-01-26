using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfindToggleFunc : MonoBehaviour
{
    [SerializeField] PathRequester pathRequester;
    [SerializeField] Toggle[] toggles;
    public PathRequester.Algorithm algorithm;
    public Pathfinder currPathfinder;
    private void Update()
    {
        if(currPathfinder != null && currPathfinder.sState == Pathfinder.FinderState.Process && toggles[0].interactable)
        {
            foreach (var t in toggles) t.interactable = false;
        }
        else if (currPathfinder != null && currPathfinder.sState == Pathfinder.FinderState.Idle && !toggles[0].interactable)
        {
            foreach (var t in toggles) t.interactable = true;
        }
    }

    public void OnToggle_BFS(Toggle toggle)
    {
        if(toggle.isOn)
        {
            pathRequester.algorithm = PathRequester.Algorithm.BFS;
            algorithm = PathRequester.Algorithm.BFS;

            if (currPathfinder == null || currPathfinder.GetType() != typeof(BFSPathfinder))
                currPathfinder = FindObjectOfType<BFSPathfinder>();
        }
    }
    public void OnToggle_DFS(Toggle toggle)
    {
        if (toggle.isOn)
        {
            algorithm = PathRequester.Algorithm.DFS;

            pathRequester.algorithm = PathRequester.Algorithm.DFS;

            if (currPathfinder == null || currPathfinder.GetType() != typeof(DFSPathfinder))
                currPathfinder = FindObjectOfType<DFSPathfinder>();
        }
    }
    public void OnToggle_Dijkstra(Toggle toggle)
    {
        if (toggle.isOn)
        {
            algorithm = PathRequester.Algorithm.Dijkstra;
            pathRequester.algorithm = PathRequester.Algorithm.Dijkstra;

            if (currPathfinder == null || currPathfinder.GetType() != typeof(DijkstraPathfinder))
                currPathfinder = FindObjectOfType<DijkstraPathfinder>();
        }
    }
    public void OnToggle_AStar(Toggle toggle)
    {
        if (toggle.isOn)
        {
            algorithm = PathRequester.Algorithm.AStar;
            pathRequester.algorithm = PathRequester.Algorithm.AStar;

            if (currPathfinder == null || currPathfinder.GetType() != typeof(BFSPathfinder))
                currPathfinder = FindObjectOfType<BFSPathfinder>();
        }
    }
}
