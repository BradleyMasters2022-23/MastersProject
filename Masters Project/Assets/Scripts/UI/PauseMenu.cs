using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Controls
    /// </summary>
    private GameControls controls;
    private InputAction reset;

    [Tooltip("Frame of the pause menu")]
    [SerializeField] private GameObject pauseScreen;

    [Tooltip("On state change channel to listen for when to pause")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;

    private void Awake()
    {
        controls = new GameControls();
        

        reset = controls.PlayerGameplay.Reset;
        reset.performed += Reload;
        reset.Enable();

        
    }

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

    private void OnEnable()
    {
        onStateChangeChannel.OnEventRaised += TogglePause;
    }

    private void OnDisable()
    {
        onStateChangeChannel.OnEventRaised -= TogglePause;

        reset.Disable();
    }

    public void Reload(InputAction.CallbackContext c)
    {
        Scene currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.name);
    }
}
