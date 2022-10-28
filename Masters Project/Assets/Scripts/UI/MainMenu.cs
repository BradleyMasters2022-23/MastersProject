using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadSettings()
    {
        // TODO add settings
    }

    public void StartGame()
    {
        GameManager.instance.ChangeState(GameManager.States.HUB);
    }
}
