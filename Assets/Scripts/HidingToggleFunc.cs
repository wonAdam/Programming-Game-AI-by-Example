using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingToggleFunc : MonoBehaviour
{
    [SerializeField] HidingMgr hidingMgr;
    [SerializeField] AIMover seeker;
    [SerializeField] ControllerMover seekerController;

    public void OnToggle_Auto(Toggle toggle)
    {
        if (toggle.isOn)
        {
            hidingMgr.enabled = true;
            seekerController.enabled = false;
            seeker.enabled = true;
        }
    }
    public void OnToggle_Control(Toggle toggle)
    {
        if(toggle.isOn)
        {
            hidingMgr.enabled = false;
            seekerController.enabled = true;
            seeker.enabled = false;
        }
    }
}

