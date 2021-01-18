using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMainBtn : MonoBehaviour
{
    public void OnClick_ToMain()
    {
        SceneManager.LoadScene("Main");
    }
}
