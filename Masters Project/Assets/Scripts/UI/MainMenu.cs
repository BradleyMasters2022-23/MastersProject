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
        if(DataManager.instance.hasSaveData)
            GameManager.instance.GoToHub();
        else
            GameManager.instance.GoToTutorial();
    }
}
