using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Clicker : MonoBehaviour
{
    public enum State { SelectStart, SelectDestination, PathFinding }
    public State state = State.SelectStart;
    [SerializeField] LayerMask NodeMask;
    [SerializeField] GraphNode start;
    [SerializeField] GraphNode destination;
    [SerializeField] PathRequester pathRequester;
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClickLeft();
        }
        if(Input.GetMouseButtonDown(1))
        {
            OnClickRight();
        }
    }

    private void OnClickRight()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero, Mathf.Infinity, NodeMask);

        if (hit.collider != null)
        {
            switch (state)
            {
                case State.SelectStart:
                    
                    return;
                case State.SelectDestination:
                    
                    return;
                case State.PathFinding:

                    return;

            }
        }
    }

    private void OnClickLeft()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero, Mathf.Infinity, NodeMask);

        if (hit.collider != null)
        {
            switch (state)
            {
                case State.SelectStart:
                    start = hit.transform.GetComponent<GraphNode>();
                    hit.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
                    state = State.SelectDestination;
                    return;
                case State.SelectDestination:
                    destination = hit.transform.GetComponent<GraphNode>();
                    hit.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
                    state = State.PathFinding;

                    pathRequester.Request(start, destination, OnServe);
                    return;
                case State.PathFinding:
                    return;

            }
        }
    }

    public void OnServe(List<GraphNode> path)
    {
        StartCoroutine(Pathfind(path));
    }



    private IEnumerator Pathfind(List<GraphNode> path)
    {
        int idx = 0;
        while (path.Count > idx)
        {
            path[idx].GetComponent<SpriteRenderer>().color = Color.blue;
            idx++;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(2f);
        path.ForEach((n) => n.GetComponent<SpriteRenderer>().color = Color.black);
        state = State.SelectStart;
    }



    private void OnDrawGizmos()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;

            Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

            Debug.DrawLine(Camera.main.transform.position, screenPos, Color.yellow);
        }
    }
}
