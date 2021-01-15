using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFuncs : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform enemy;
    [SerializeField] FollowCamera followCam;

    public void OnClick_Seek(Toggle toggle)
    {
        if(toggle.isOn)
            enemy.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.Seek;
    }
    public void OnClick_Flee(Toggle toggle)
    {
        if (toggle.isOn)
            enemy.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.Flee;
    }
    public void OnClick_Arrive(Toggle toggle)
    {
        if (toggle.isOn)
            enemy.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.Arrive;
    }
    public void OnClick_Pursuit(Toggle toggle)
    {
        if (toggle.isOn)
            enemy.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.Pursuit;
    }
    public void OnClick_Evade(Toggle toggle)
    {
        if(toggle.isOn)
            enemy.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.Evade;
    }
    public void OnClick_Wander(Toggle toggle)
    {
        if (toggle.isOn)
            enemy.GetComponent<SteeringBehaviors>().state = SteeringBehaviors.State.Wander;
    }

    public void OnClick_CamToEnemy(Toggle toggle) 
    {
        if (toggle.isOn)
            followCam.target = enemy;
    }

    public void OnClick_CamToPlayer(Toggle toggle)
    {
        if(toggle.isOn)
            followCam.target = player;

    }
    public void OnClick_CamMoveWithWASD(Toggle toggle)
    {
        if (toggle.isOn)
            followCam.target = null;

    }
}
