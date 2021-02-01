using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeElapsed : MonoBehaviour
{
    [SerializeField] PathRequester pathRequester;
    [SerializeField] Text timeText;

    public float timeElapsed;
    public Pathfinder currPathfinder;
    bool startTik = true;


    // Update is called once per frame
    void Update()
    {
        UpdatePathfinder();
        

        if (currPathfinder.sState == Pathfinder.FinderState.Process)
        {
            if (startTik)
            {
                startTik = false;
                timeElapsed = 0f;
            }

            timeElapsed += Time.deltaTime;
            UpdateTimeText(timeElapsed);
        }

        else startTik = true;
    }

    public void UpdateTimeText(float time)
    {
        int milisec = (int)(time * 1000) % 1000;
        int second = (int)time % 10;
        int secten = (int)(time / 10) % 6;
        int minone = (int)(time / 60) % 10;
        int minten = (int)(time / 600) % 6;

        timeText.text = $"{minten}{minone}:{secten}{second}.{milisec}";
    }

    private void UpdatePathfinder()
    {
        switch (pathRequester.algorithm)
        {
            case PathRequester.Algorithm.BFS:
                if (currPathfinder == null || currPathfinder.GetType() != typeof(BFSPathfinder))
                    currPathfinder = FindObjectOfType<BFSPathfinder>();
                break;
            case PathRequester.Algorithm.DFS:
                if (currPathfinder == null || currPathfinder.GetType() != typeof(DFSPathfinder))
                    currPathfinder = FindObjectOfType<DFSPathfinder>();
                break;
            case PathRequester.Algorithm.Dijkstra:
                if (currPathfinder == null || currPathfinder.GetType() != typeof(DijkstraPathfinder))
                    currPathfinder = FindObjectOfType<DijkstraPathfinder>();
                break;
            case PathRequester.Algorithm.AStar:
                if (currPathfinder == null || currPathfinder.GetType() != typeof(AStarPathfiner))
                    currPathfinder = FindObjectOfType<AStarPathfiner>();
                break;
        }
    }
}
