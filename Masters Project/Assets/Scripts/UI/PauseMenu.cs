using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Tooltip("Frame of the pause menu")]
    [SerializeField] private GameObject pauseScreen;

    [Tooltip("On state change channel to listen for when to pause")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;


    /// <summary>
    /// Toggle the pause screen menu
    /// </summary>
    /// <param name="_newState">New state being set to</param>
    private void TogglePause(GameManager.States _newState)
    {
        if (_newState == GameManager.States.PAUSED)
        {
            pauseScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Function for resuming game. Called via UI buttons
    /// </summary>
    public void ResumeGame()
    {
        GameManager.instance.TogglePause();
    }

    /// <summary>
    /// Quit out the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Request the ability to quit the game
    /// </summary>
    public void RequestQuitGame()
    {
        FindObjectOfType<ConfirmationBox>(true).RequestConfirmation(QuitGame);
    }

    private void OnEnable()
    {
        onStateChangeChannel.OnEventRaised += TogglePause;
    }

    private void OnDisable()
    {
        onStateChangeChannel.OnEventRaised -= TogglePause;
    }
}
