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

    public void LoadToTutorial()
    {
        GameManager.instance.GoToTutorial();
    }

    public void StartGame()
    {
        StartCoroutine(DelayedStart());
    }

    /// <summary>
    /// add a minor delay for smoother transitions, and to allow difficulty settings to properly apply
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        // player gets 1 convo tick on first go to hub post-tutorial, so check that
        if (GlobalStatsManager.data != null && GlobalStatsManager.data.convoTicks > 0)
            GameManager.instance.GoToHub();
        else
            GameManager.instance.GoToTutorial();
    }
}
