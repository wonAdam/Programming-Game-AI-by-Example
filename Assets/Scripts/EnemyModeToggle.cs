using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyModeToggle : MonoBehaviour
{
    [SerializeField] Enemy enemy;

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
}
