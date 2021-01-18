using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowPathMgr : MonoBehaviour
{
    [SerializeField] GameObject agentPrefab;
    private GameObject agent;
    public bool isSimulating = false;
    [SerializeField] Waypoint waypointPrefab;

    [SerializeField] Button[] btnsToInteractable;
    [SerializeField] Toggle[] togglesToInsteractable;

    public void OnClick_Simulation(Text buttonText)
    {
        if(!isSimulating)
        {
            Path path = FindObjectOfType<Path>();
            agent = Instantiate(agentPrefab, path.waypoints[0].transform.position, Quaternion.identity);

            agent.GetComponent<MovingEntity>().path = path;
            agent.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.FollowPath;


            foreach (var btn in btnsToInteractable)
                btn.interactable = false;
            foreach (var toggle in togglesToInsteractable)
                toggle.interactable = false;

            isSimulating = true;
            buttonText.text = "Stop Simulation";



        }
        else
        {
            Destroy(agent);

            foreach (var btn in btnsToInteractable)
                btn.interactable = true;
            foreach (var toggle in togglesToInsteractable)
                toggle.interactable = true;

            isSimulating = false;
            buttonText.text = "Start Simulation";
        }
    }

    public void OnClick_ClosedPathToggle(Toggle toggle)
    {
        if(toggle.isOn)
        {
            FindObjectOfType<Path>().IsClosedPath = true;
        }
        else
        {
            FindObjectOfType<Path>().IsClosedPath = false;
        }
    }

    public void OnClick_AddWaypoint()
    {
        FindObjectOfType<Path>().AddWaypoint(Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity));
    }
    public void OnClick_RemoveWaypoint()
    {
        FindObjectOfType<Path>().RemoveWaypoint();
    }
}
