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
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    public enum ControllerType
    {
        MOUSE,
        CONTROLLER
    }

    #region Core Variables

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
    /// Type of controller being used
    /// </summary>
    public static ControllerType controllerType;

    [Header("===== Scene Management Info =====")]

    [Tooltip("Name of the hub scene")]
    [SerializeField] private string mainHubScene;

    [Tooltip("Name of the main menu scene")]
    [SerializeField] private string mainMenuScene;

    [Tooltip("Name of the main gameplay scene")]
    [SerializeField] private string mainGameplayScene;

    #endregion

    #region Input Management Variables

    private GameObject lastSelectedObject;

    private PlayerInput input;

    /// <summary>
    /// Track the last state
    /// </summary>
    private States lastState;

    public static GameControls controls;
    private InputAction escape;
    private InputAction checkController;
    private InputAction checkCursor;

    #endregion

    #region Channel Variables

    [Header("=====Game Flow Channels=====")]

    [Tooltip("Channel that handles requests to change the state")]
    [SerializeField] private ChannelGMStates requestStateChangeChannel;
    [Tooltip("Channel that handles on state change actions")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;
    [Tooltip("Channel that handles game over actions")]
    [SerializeField] private ChannelVoid onGameOverChannel;

    #endregion

    #region Menu Variables

    /// <summary>
    /// Stack of opened UI menus
    /// </summary>
    private Stack<UIMenu> menuStack;

    private InputAction backMenu;

    #endregion

    /// <summary>
    /// Last health of the player. Outdated, but keeping for backup
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
            return;
        }

        // Set up controls
        controls = new GameControls();
        escape = controls.PlayerGameplay.Pause;
        escape.performed += TogglePause;
        escape.Enable();

        checkController = controls.PlayerGameplay.Controller;
        checkController.performed += HideCursor;
        checkController.Enable();

        checkCursor = controls.PlayerGameplay.Mouse;
        checkCursor.performed += ShowCursor;
        checkCursor.Enable();

        backMenu = controls.UI.Back;
        backMenu.performed += CloseTopMenu;
        backMenu.Enable();

        // Subscribe change state function to channel
        requestStateChangeChannel.OnEventRaised += ChangeState;

        //controls.PlayerGameplay.Disable();
        controls.PlayerGameplay.Enable();
        controls.UI.Disable();
        menuStack = new Stack<UIMenu>();
    }


    #region State Management

    /// <summary>
    /// Try to change the state to the new state, trigger any on-state actions
    /// </summary>
    /// <param name="_newState">New state to move to</param>
    public void ChangeState(States _newState)
    {
        // If an illegal state change, notify and exit. 
        if(!ValidateStateChange(_newState))
        {
            Debug.LogError($"[Game Manager] Something is trying to go from state {currentState} to {_newState}!");
            return;
        }

        // perform any actions when moving to the new state
        switch (_newState)
        {
            case States.MAINMENU:
                {
                    SceneManager.LoadScene(mainMenuScene);

                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.PAUSED:
                {
                    Pause();

                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.HUB:
                {
                    if(SceneManager.GetActiveScene().name != mainHubScene)
                    {
                        SceneManager.LoadScene(mainHubScene);
                    }

                    // Reset player health in hub
                    lastPlayerHealth = 0;

                    UnPause();

                    // TODO - switch to hub bindings, incase different
                    controls.UI.Disable();
                    controls.PlayerGameplay.Enable();

                    break;
                }
            case States.GAMEPLAY:
                {
                    if(SceneManager.GetActiveScene().name != mainGameplayScene) 
                    {
                        SceneManager.LoadScene(mainGameplayScene);
                    }
                    UnPause();

                    controls.UI.Disable();
                    controls.PlayerGameplay.Enable();

                    break;
                }
            case States.GAMEMENU:
                {
                    Pause();

                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.GAMEOVER:
                {
                    Pause();

                    onGameOverChannel.RaiseEvent();

                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.LOADING:
                {
                    Pause();

                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

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
                        || _newState == States.GAMEMENU
                        || _newState == States.GAMEPLAY);
                }
            case States.GAMEPLAY:
                {
                    // Gameplay can go into paused, game menu, game over, and loading
                    return (_newState == States.PAUSED
                        || _newState == States.GAMEMENU
                        || _newState == States.HUB
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
                    return (_newState == States.MAINMENU
                        || _newState == States.HUB);
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
    public void TogglePause(InputAction.CallbackContext c = default)
    {
        // try pausing
        if (ValidateStateChange(States.PAUSED))
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
    /// Pause the game time and unlock cursor
    /// </summary>
    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;

        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
    }
    /// <summary>
    /// Unpause the game time and lock cursor
    /// </summary>
    private void UnPause()
    {
        Cursor.lockState = CursorLockMode.None;

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    #endregion

    #region Controller & Mouse Swapping

    private void OnApplicationFocus(bool focus)
    {
        UpdateMouseMode();
    }

    private void Update()
    {
        EventSystem t = FindObjectOfType<EventSystem>();
        if (t != null)
            lastSelectedObject = t.currentSelectedGameObject;
    }

    private void HideCursor(InputAction.CallbackContext c)
    {
        if (controllerType == ControllerType.MOUSE
            && checkController.ReadValue<Vector2>() != Vector2.zero)
        {
            //Debug.Log("Controller detected, hiding cursor");

            controllerType = ControllerType.CONTROLLER;

            Cursor.lockState = CursorLockMode.None;
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Set active button to either the default or last selected button
            EventSystem t = FindObjectOfType<EventSystem>();
            if (t != null && lastSelectedObject != null && lastSelectedObject.activeInHierarchy)
                t.SetSelectedGameObject(lastSelectedObject);
            else if (t != null && t.firstSelectedGameObject != null)
                t.SetSelectedGameObject(t.firstSelectedGameObject);
        }
    }

    private void ShowCursor(InputAction.CallbackContext c)
    {
        if (controllerType == ControllerType.CONTROLLER
            && checkCursor.ReadValue<Vector2>() != Vector2.zero)
        {
            //Debug.Log("Mouse detected, showing cursor");

            controllerType = ControllerType.MOUSE;

            // Reenable cursor, set appropriate lock state
            Cursor.visible = true;
            UpdateMouseMode();
        }
    }

    private void UpdateMouseMode()
    {
        Cursor.lockState = CursorLockMode.None;

        switch (currentState)
        {
            case States.MAINMENU:
                {
                    Cursor.lockState = CursorLockMode.Confined;

                    break;
                }
            case States.PAUSED:
                {
                    Cursor.lockState = CursorLockMode.Confined;

                    break;
                }
            case States.HUB:
                {
                    Cursor.lockState = CursorLockMode.Locked;

                    break;
                }
            case States.GAMEPLAY:
                {
                    Cursor.lockState = CursorLockMode.Locked;

                    break;
                }
            case States.GAMEMENU:
                {
                    Cursor.lockState = CursorLockMode.Confined;

                    break;
                }
            case States.GAMEOVER:
                {
                    Cursor.lockState = CursorLockMode.Confined;

                    onGameOverChannel.RaiseEvent();
                    break;
                }
            case States.LOADING:
                {
                    Cursor.lockState = CursorLockMode.Confined;


                    break;
                }
        }
    }

    #endregion

    #region Menu Management

    /// <summary>
    /// Push a new menu onto the stack
    /// </summary>
    /// <param name="menu">newely opened menu to add to stack</param>
    public void PushMenu(UIMenu menu)
    {
        // Debug.Log($"[GameManager] added menu {menu.name} to UI stack");
        menuStack.Push(menu);
    }

    /// <summary>
    /// Close the current top menu, if possible
    /// </summary>
    public void CloseTopMenu(InputAction.CallbackContext c = default)
    {
        // Check if there are no options available
        if(menuStack.Count <= 0)
        {
            Debug.Log("[GameManager] Close menu called, but no menu to close!");
            return;
        }

        // Close the top menu and remove from stack
        UIMenu menu = menuStack.Pop();
        menu.Close();

        // Check if all menus are closed and if should return to appropriate scene
        if(menuStack.Count <= 0)
        {
            if(currentState == States.PAUSED)
            {
                TogglePause();
            }
            else if (currentState == States.GAMEMENU)
            {
                // Change it back to its previous state (Gameplay or HUB)
                ChangeState(lastState);
            }

        }
    }

    #endregion
}
