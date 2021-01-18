using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OffsetPursuitMgr : MonoBehaviour
{
    [SerializeField] Button resetBtn;
    private void Start()
    {
        resetBtn.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if(v > Mathf.Epsilon || h > Mathf.Epsilon)
        {
            resetBtn.interactable = true;
        }
    }

    public void OnClick_ResetBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
