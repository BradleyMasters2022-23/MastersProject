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
        StartCoroutine(DelayedTutorial());
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

        GameManager.instance.GoToHub();
    }

    private IEnumerator DelayedTutorial()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        
        GameManager.instance.GoToTutorial();
    }
}
