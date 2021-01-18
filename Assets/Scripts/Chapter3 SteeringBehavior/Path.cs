using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] bool isClosedPath = false;
    [SerializeField] public List<Waypoint> waypoints;
    [SerializeField] GameObject linePrefab;

    [HideInInspector]
    public List<LineRenderer> lines = new List<LineRenderer>();

    public bool IsClosedPath { get => isClosedPath; set { isClosedPath = value; Init(); } }

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        foreach (var l in lines)
            Destroy(l.gameObject);

        foreach(var w in waypoints)
        {
            w.line1 = null;
            w.line2 = null;
        }

        lines.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            GameObject lineInst = Instantiate(linePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            LineRenderer lineRenderer = lineInst.GetComponent<LineRenderer>();

            if (i < waypoints.Count - 1)
            {
                lineRenderer.SetPosition(0, waypoints[i].transform.position);
                lineRenderer.SetPosition(1, waypoints[i + 1].transform.position);
                waypoints[i].line2 = lineRenderer;
                waypoints[i+1].line1 = lineRenderer;
                lines.Add(lineRenderer);
            }

            else
            {
                if (IsClosedPath)
                {
                    lineRenderer.SetPosition(0, waypoints[i].transform.position);
                    lineRenderer.SetPosition(1, waypoints[0].transform.position);
                    waypoints[i].line2 = lineRenderer;
                    waypoints[0].line1 = lineRenderer;
                    lines.Add(lineRenderer);
                }
                else 
                    Destroy(lineInst); 
            }
        }
    }
    private void OnDrawGizmos()
    {
        for(int i = 0; i < waypoints.Count; i++)
        {
            if(i < waypoints.Count - 1)
                Debug.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position, Color.red);
            else
            {
                if(IsClosedPath)
                    Debug.DrawLine(waypoints[i].transform.position, waypoints[0].transform.position, Color.red);
            }
        }
    }

    public Waypoint GetNextWaypoint(Waypoint currWaypoint)
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == currWaypoint)
            {
                if(i + 1 < waypoints.Count)
                {
                    return waypoints[i + 1];
                }
                else
                {
                    if (IsClosedPath) return waypoints[0];
                    else return null;
                }
            }
        }

        return null;
    }

    public Waypoint GetNearestWaypoint (Vector2 pos)
    {
        Waypoint waypoint = waypoints[0];
        foreach (var wp in waypoints)
        {
            if(Vector2.Distance(pos, wp.transform.position) < Vector2.Distance(pos, waypoint.transform.position))
            {
                waypoint = wp;
            }
        }

        return waypoint;
    }

    public void AddWaypoint(Waypoint waypoint)
    {
        waypoints.Add(waypoint);
        Init();
    }

    public void RemoveWaypoint()
    {
        if (waypoints.Count - 1 < 0) return;

        GameObject inst = waypoints[waypoints.Count - 1].gameObject;
        Destroy(inst);
        waypoints.RemoveAt(waypoints.Count - 1);
        Init();
    }

}
