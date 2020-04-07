using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public void GotoGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GotoMapTool()
    {
        SceneManager.LoadScene("MapTool");
    }
}
