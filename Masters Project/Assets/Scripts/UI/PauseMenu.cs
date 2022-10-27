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
    private InputAction pause;
    private InputAction reset;

    /// <summary>
    /// Whether the game is paused
    /// </summary>
    private bool paused;

    [Tooltip("Frame of the pause menu")]
    [SerializeField] private GameObject pauseScreen;

    private void Awake()
    {
        controls = new GameControls();
        pause = controls.PlayerGameplay.Pause;
        pause.performed += TogglePause;
        pause.Enable();

        reset = controls.PlayerGameplay.Reset;
        reset.performed += Reload;
        reset.Enable();
    }

    /// <summary>
    /// Toggle the pause 
    /// </summary>
    /// <param name="c"></param>
    private void TogglePause(InputAction.CallbackContext c)
    {
        paused = !paused;

        if(paused)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void UnpauseGame()
    {
        paused = false;
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }

    /// <summary>
    /// Quit out the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        pause.Disable();
        reset.Disable();
    }

    public void Reload(InputAction.CallbackContext c)
    {
        Scene currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.name);
    }
}
