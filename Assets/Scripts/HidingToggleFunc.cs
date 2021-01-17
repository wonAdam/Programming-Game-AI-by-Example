using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingToggleFunc : MonoBehaviour
{
    [SerializeField] HidingMgr hidingMgr;
    [SerializeField] PlayerMover playerMover;
    [SerializeField] EnemyMover enemyMover;

    public void OnToggle_Auto(Toggle toggle)
    {
        if (toggle.isOn)
        {
            hidingMgr.enabled = true;
            playerMover.enabled = false;
            enemyMover.enabled = true;
        }
    }
    public void OnToggle_Control(Toggle toggle)
    {
        if(toggle.isOn)
        {
            hidingMgr.enabled = false;
            playerMover.enabled = true;
            enemyMover.enabled = false;
        }
    }
}

