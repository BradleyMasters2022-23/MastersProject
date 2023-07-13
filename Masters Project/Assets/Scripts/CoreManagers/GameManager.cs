/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 27th, 2022
 * Last Edited - October 27th, 2022 by Ben Schuster
 * Description - Main game maanger
 * ================================================================================================
 */
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("===== Scene Management Info =====")]

    [Tooltip("Name of the hub scene")]
    [SerializeField] private string mainHubScene;

    [Tooltip("Name of the main menu scene")]
    [SerializeField] private string mainMenuScene;

    [Tooltip("Name of the main gameplay scene")]
    [SerializeField] private string mainGameplayScene;

    [Tooltip("Name of the tutorial scene")]
    [SerializeField] private string tutorialScene;

    #endregion

    #region Input Management Variables

    private GameObject lastSelectedObject;

    private PlayerInput input;

    /// <summary>
    /// Track the last state
    /// </summary>
    [SerializeField] private States lastState;
    [SerializeField] private ChannelControlScheme onControlSchemeSwapChannel;
    public static GameControls controls;
    private InputAction escape;
    //private InputAction checkController;
    //private InputAction checkCursor;

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

        // Set up controls. Get refernece to Input Manager for ease of updates
        controls = InputManager.Controls;
        escape = controls.PlayerGameplay.Pause;
        escape.performed += TogglePause;
        escape.Enable();

        backMenu = controls.UI.Back;
        backMenu.performed += CloseTopMenu;
        backMenu.Enable();

        // Subscribe change state function to channel
        requestStateChangeChannel.OnEventRaised += ChangeState;

        // Set correct control scheme
        UpdateControlMode();

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
                    UnPause();

                    break;
                }
            case States.PAUSED:
                {
                    Pause();

                    break;
                }
            case States.HUB:
                {
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

        UpdateControlMode();
        UpdateMouseMode();
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
                    // Main menu can only go to hub gameplay OR tutorial gameplay
                    return (_newState == States.HUB
                        || _newState == States.GAMEPLAY);
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
        Cursor.lockState = CursorLockMode.Confined;
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

    /// <summary>
    /// Close all menus possible
    /// </summary>
    public void CloseToTop()
    {
        while(menuStack.Count > 0 && menuStack.Peek().Closable)
        {
            CloseTopMenu();
        }
    }

    #endregion

    #region Controller & Mouse Swapping

    private void OnEnable()
    {
        onControlSchemeSwapChannel.OnEventRaised += UpdateControlScheme;
    }
    private void OnDisable()
    {
        onControlSchemeSwapChannel.OnEventRaised -= UpdateControlScheme;
    }

    private void OnApplicationFocus(bool focus)
    {
        UpdateMouseMode();
    }

    private void UpdateControlScheme(InputManager.ControlScheme scheme)
    {
        //if (scheme == InputManager.ControlScheme.KEYBOARD)
        //    ShowCursor();
        //else if(scheme == InputManager.ControlScheme.CONTROLLER)
        //    HideCursor();
    }

    private void HideCursor()
    {
        // Hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ClearPointer();

        // Set the controller select to the screen
        if (menuStack != null && menuStack.Count > 0)
            menuStack.Peek().TopStackFunction();
    }

    private void ShowCursor()
    {
        // Reenable cursor, set appropriate lock state
        UpdateMouseMode();

        // Save the last thing the controller was on 
        if(menuStack != null && menuStack.Count > 0)
            menuStack.Peek().StackSave();

        // hide the controller's selected option
        //EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Update the cursor lock status. Update the controller cursor manager
    /// </summary>
    public void UpdateMouseMode()
    {
        //Debug.Log("Mouse mode updated");
        // Currently, should be confined for everything except the main gameplay
        switch (currentState)
        {
            case States.HUB:
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    ControllerCursor.instance.SetUIState(false);
                    break;
                }
            case States.GAMEPLAY:
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    ControllerCursor.instance.SetUIState(false);
                    break;
                }
            default:
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;

                    ControllerCursor.instance.SetUIState(true);
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
        //EventSystem.current.SetSelectedGameObject(null);
        //Debug.Log("New menu added");
        if(menuStack.Count > 0)
        {
            menuStack.Peek().StackSave();
            menuStack.Peek().SetBackground();
        }

        // initialize it when on top of stack
        menuStack.Push(menu);
        menuStack.Peek().TopStackFunction();
    }

    /// <summary>
    /// Close the current top menu, if possible
    /// </summary>
    public void CloseTopMenu(InputAction.CallbackContext c = default)
    {
        // Check if there are no options available
        if (menuStack.Count <= 0)
        {
            //Debug.Log("[GameManager] Close menu called, but no menu to close!");
            return;
        }
        else if(!menuStack.Peek().Closable)
        {
            Debug.Log($"[GameManager] Tried to close the menu {menuStack.Peek().name}, but its marked as permenant!");
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);

        // Save the stack select before continuing
        if (menuStack.Count > 0)
        {
            menuStack.Peek().StackSave();
        }

        // Close the top menu and remove from stack
        UIMenu menu = menuStack.Pop();
        menu.Close();
        //Debug.Log($"Closing menu named {menu.name}");

        // Tell the stack below it to load its select
        if (menuStack.Count > 0)
        {
            Debug.Log($"Calling top stack on {menuStack.Peek().name}");
            menuStack.Peek().TopStackFunction();
        }

        // Check if all menus are closed and if should return to appropriate state
        if (menuStack.Count <= 0)
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

    /// <summary>
    /// continually close menus until it reaches the target
    /// </summary>
    /// <param name="target">target menu to close UNTIL</param>
    public void CloseToMenu(UIMenu target)
    {
        if (target == null) return;

        while (menuStack.Count > 0 && menuStack.Peek().Closable && menuStack.Peek() != target)
        {
            CloseTopMenu();
        }
    }

    /// <summary>
    /// Update the current control scheme based on the current state
    /// </summary>
    private void UpdateControlMode()
    {
        switch (currentState)
        {
            case States.MAINMENU:
                {
                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.PAUSED:
                {
                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.HUB:
                {
                    controls.PlayerGameplay.Enable();
                    controls.UI.Disable();

                    break;
                }
            case States.GAMEPLAY:
                {
                    controls.PlayerGameplay.Enable();
                    controls.UI.Disable();

                    break;
                }
            case States.GAMEMENU:
                {
                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.GAMEOVER:
                {
                    controls.UI.Enable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
            case States.LOADING:
                {
                    controls.UI.Disable();
                    controls.PlayerGameplay.Disable();

                    break;
                }
        }
    }


    /// <summary>
    /// Clear any pointer hover effects
    /// </summary>
    public void ClearPointer()
    {
        //Debug.Log("Trying to clear pointer effects");

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            // make sure its accurate based on the type
            foreach (RaycastResult raycastResult in raycastResults)
            {
                //Debug.Log(raycastResult.gameObject.name);
                GameObject hoveredObj = raycastResult.gameObject;

                if (hoveredObj.GetComponent<Button>() != null)
                {
                    hoveredObj.GetComponent<Button>().OnPointerExit(pointer);
                }
                else if (hoveredObj.GetComponent<Toggle>() != null)
                {
                    hoveredObj.GetComponent<Toggle>().OnPointerExit(pointer);
                }
                else if (hoveredObj.GetComponent<Slider>() != null)
                {
                    hoveredObj.GetComponent<Slider>().OnPointerExit(pointer);
                }
            }

        }
    }

    public void TempLockUIFlow(bool lockUI)
    {
        if(lockUI)
        {
            backMenu.Disable();
        }
        else
        {
            backMenu.Enable();
        }

    }

    #endregion

    #region SceneLoading

    public void GoToHub()
    {
        LoadToScene(mainHubScene);
        ChangeState(States.HUB);
    }

    public void GoToMainMenu()
    {
        LoadToScene(mainMenuScene);
        ChangeState(States.MAINMENU);
    }

    public void GoToMainGame()
    {
        LoadToScene(mainGameplayScene);
        ChangeState(States.GAMEPLAY);
    }

    public void GoToTutorial()
    {
        LoadToScene(tutorialScene);
        ChangeState(States.GAMEPLAY);
    }

    public Coroutine LoadToScene(string name)
    {
        // if none of the main scenes, dont load. 
        // Useful for testing scenes and pausing/unpausing
        string currScene = SceneManager.GetActiveScene().name;
        try
        {
            // attempt to save global stats whenver loading
            if (currScene != mainMenuScene && GlobalStatsManager.Instance != null)
                GlobalStatsManager.SaveData();
        }
        catch(Exception e)
        {
            Debug.Log($"Saving stats between scenes has failed. {e}");
        }

        UnPause();
        menuStack.Clear();
        controls.UI.Disable();

        // if loading to tutorial, don't enable controls on load complete
        if(name == tutorialScene)
            return Loader.instance.LoadToScene(name, false);
        else
            return Loader.instance.LoadToScene(name);

    }



    #endregion
}
