using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneChange()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeNewGame();
        }
        SceneManager.LoadScene("MainScene");
    }
}
