using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingResetRecTxt : MonoBehaviour
{
    [SerializeField] Text myTxt;
    [SerializeField] HidingMgr hidingMgr;

    private void Update()
    {
        myTxt.text = "ResetLeft : " + (7 - (int)hidingMgr.resetSec).ToString();
    }
}
