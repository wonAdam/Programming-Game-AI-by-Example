using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneMgr : MonoBehaviour
{
    public void OnClick_Btn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
