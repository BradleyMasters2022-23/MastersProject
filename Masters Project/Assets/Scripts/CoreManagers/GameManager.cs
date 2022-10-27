/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 27th, 2022
 * Last Edited - October 27th, 2022 by Ben Schuster
 * Description - Main game maanger
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum States
    {
        MAINMENU,
        PAUSED,
        HUB,
        GAMEPLAY,
        GAMEMENU,
        GAMEOVER,
        LOADING
    }

    /// <summary>
    /// Public instance of game manager
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// Current state of the manager
    /// </summary>
    [SerializeField] private States currentState;
    /// <summary>
    /// Current state of the manager
    /// </summary>
    public States CurrentState
    {
        get { return currentState; }
    }

    /// <summary>
    /// Track the last state
    /// </summary>
    private States lastState;

    private GameControls controls;
    private InputAction escape;

    [Tooltip("Channel that handles requests to change the state")]
    [SerializeField] private ChannelGMStates requestStateChangeChannel;
    [Tooltip("Channel that handles on state change actions")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;
    [Tooltip("Channel that handles game over actions")]
    [SerializeField] private ChannelVoid onGameOverChannel;

    /// <summary>
    /// Last health of the player
    /// </summary>
    [HideInInspector] public int lastPlayerHealth;

    /// <summary>
    /// Initialize internal systems
    /// </summary>
    private void Start()
    {
        // Create singleton, initialize instane
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        controls = new GameControls();
        escape = controls.PlayerGameplay.Pause;
        escape.performed += TogglePause;
        escape.Enable();

        Cursor.lockState = CursorLockMode.Locked;

        // Subscribe change state function to channel
        requestStateChangeChannel.OnEventRaised += ChangeState;

    }

    #region State Management

    /// <summary>
    /// Try to change the state to the new state, trigger any on-state actions
    /// </summary>
    /// <param name="_newState">New state to move to</param>
    private void ChangeState(States _newState)
    {
        // If an illegal state change, notify and exit. 
        if(!ValidateStateChange(_newState))
        {
            Debug.LogError($"[Game Manager] Something is trying to go from state {currentState} to {_newState}!");
            return;
        }

        // perform any actions inside this script
        switch (_newState)
        {
            case States.MAINMENU:
                {
                    break;
                }
            case States.PAUSED:
                {
                    Pause();
                    break;
                }
            case States.HUB:
                {
                    // Reset player health in hub
                    lastPlayerHealth = 0;

                    UnPause();
                    break;
                }
            case States.GAMEPLAY:
                {
                    UnPause();
                    break;
                }
            case States.GAMEMENU:
                {
                    Pause();

                    break;
                }
            case States.GAMEOVER:
                {
                    Pause();

                    onGameOverChannel.RaiseEvent();
                    break;
                }
            case States.LOADING:
                {
                    Pause();
                    break;
                }
        }


        // perform any new events outside this script
        onStateChangeChannel.RaiseEvent(_newState);

        // Change state
        lastState = currentState;
        currentState = _newState;
    }

    /// <summary>
    /// Validate that the requested state change can happen
    /// </summary>
    /// <param name="_newState">State to check against current state</param>
    private bool ValidateStateChange(States _newState)
    {
        // Based on current state, check if its valid. 
        // Based on the 'GameManger State Diagram' in drive
        switch(currentState)
        {
            case States.MAINMENU:
                {
                    // Main menu can only go to hub gameplay
                    return (_newState == States.HUB);
                }
            case States.PAUSED:
                {
                    // Paused can go into main menu, hub gameplay, and gameplay
                    return (_newState == States.MAINMENU
                        || _newState == States.HUB
                        || _newState == States.GAMEPLAY);
                }
            case States.HUB:
                {
                    // Hub can go into paused and a game menu
                    return (_newState == States.PAUSED
                        || _newState == States.GAMEMENU);
                }
            case States.GAMEPLAY:
                {
                    // Gameplay can go into paused, game menu, game over, and loading
                    return (_newState == States.PAUSED
                        || _newState == States.GAMEMENU
                        || _newState == States.GAMEOVER
                        || _newState == States.LOADING);
                }
            case States.GAMEMENU:
                {
                    // Game Menu can go into Hub, Gameplay, and loading
                    return (_newState == States.HUB
                        || _newState == States.GAMEPLAY
                        || _newState == States.LOADING);
                }
            case States.GAMEOVER:
                {
                    // Game over can go into game menu
                    return (_newState == States.GAMEMENU);
                }
            case States.LOADING:
                {
                    // Loading can go into Hub and Gameplay
                    return (_newState == States.HUB
                        || _newState == States.GAMEPLAY);
                }
        }

        return false;
    }

    #endregion

    #region Pause

    /// <summary>
    /// Input for toggling pause, if possible
    /// </summary>
    /// <param name="c"></param>
    private void TogglePause(InputAction.CallbackContext c)
    {
        // try pausing
        if(ValidateStateChange(States.PAUSED))
        {
            ChangeState(States.PAUSED);
        }
        // try unpausing
        else if(currentState == States.PAUSED
            && ValidateStateChange(lastState))
        {
            ChangeState(lastState);
        }
    }

    /// <summary>
    /// Public call to toggle pause
    /// </summary>
    public void TogglePause()
    {
        TogglePause(new InputAction.CallbackContext());
    }

    /// <summary>
    /// Pause the game time and unlock cursor
    /// </summary>
    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }
    /// <summary>
    /// Unpause the game time and lock cursor
    /// </summary>
    private void UnPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    private void OnApplicationFocus(bool focus)
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion

}
