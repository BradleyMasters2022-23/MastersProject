/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 16th, 2023
 * Last Edited - June 16th, 2023
 * Description - Global manager for player inputs
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class InputManager : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// Global instance of input manager
    /// </summary>
    public static InputManager Instance;
    /// <summary>
    /// Global instance of the controls
    /// </summary>
    public static GameControls Controls;

    /// <summary>
    /// Prepare singleton
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Controls = new GameControls();


    }
    #endregion

    private void Start()
    {
        SchemeSwapStart();
    }

    #region Action Map Swapping

    /// <summary>
    /// Swap to a new action map. Other action maps are automatically disabled
    /// </summary>
    /// <param name="newMap">New map to swap to</param>
    public static void SwapActionMap(InputActionMap newMap)
    {
        // Don't do anything if its already enabled
        if (newMap.enabled)
            return;

        // Disable ALL action maps for core gameplay, then only turn on the original
        Controls.Disable();
        newMap.Enable();
    }

    #endregion

    #region Scheme Swapping

    /// <summary>
    /// All states for controls the player can be in
    /// </summary>
    public enum ControlScheme
    {
        KEYBOARD,
        CONTROLLER
    }

    [Tooltip("Current control scheme for the player")]
    public static ControlScheme CurrControlScheme;
    /// <summary>
    /// Reference to the controls that observe all options
    /// </summary>
    private SchemeControls schemeObserver;

    [SerializeField] private ChannelControlScheme controlSchemeSwapChannel;

    /// <summary>
    /// Unique initialization for swapping control schemes at runtime
    /// </summary>
    private void SchemeSwapStart()
    {
        // Prepare controls
        schemeObserver = new SchemeControls();
        schemeObserver.ObserveGamePad.SwapControls.performed += SwapControls;
        schemeObserver.ObserveMK.SwapControls.performed += SwapControls;

        // Set both enabled so it correctly updates regardless which input they use first
        //schemeObserver.ObserveGamePad.SwapControls.Enable();
        //schemeObserver.ObserveMK.SwapControls.Enable();
    }
    private void OnDisable()
    {
        schemeObserver.ObserveGamePad.SwapControls.performed -= SwapControls;
        schemeObserver.ObserveMK.SwapControls.performed -= SwapControls;
    }

    /// <summary>
    /// Swap the controls to the relevant type
    /// </summary>
    /// <param name="c">Context of the input</param>
    private void SwapControls(InputAction.CallbackContext c)
    {
        if(inputCD) // If on cooldown, ignore
        {
            return;
        }
        else // Otherwise, reset cooldown
        {
            inputCD = true;
            StartCoroutine(FrameCooldown());
        }

        bool swapped = false;

        schemeObserver.Disable();

        //Debug.Log("Swap control checking for : " + c.control.displayName);
        // If currently using controller and M&K detected, switch to keyboard
        if (CurrControlScheme == ControlScheme.CONTROLLER)
        {
            swapped = true;
            SetToKeyboard();
        }
        else if (CurrControlScheme == ControlScheme.KEYBOARD)
        {
            swapped = true;
            SetToController();
        }

        // If controls were swapped, trigger the channel
        if (swapped)
            controlSchemeSwapChannel?.RaiseEvent(CurrControlScheme);
    }
    /// <summary>
    /// Swap the control scheme to keyboard and perform any necessary functions
    /// </summary>
    private void SetToKeyboard()
    {
        Debug.Log("Switching to keyboard");
        CurrControlScheme = ControlScheme.KEYBOARD;

        // Update observer to only register gamepad inputs to reduce load
        schemeObserver.ObserveGamePad.Enable();
    }
    /// <summary>
    /// Swap the control scheme to controller and perform any necessary functions
    /// </summary>
    private void SetToController()
    {
        Debug.Log("Switching to controller");
        CurrControlScheme = ControlScheme.CONTROLLER;

        // Update observer to only register M&K inputs to reduce load
        schemeObserver.ObserveMK.Enable();
    }
    private bool inputCD = false;
    private IEnumerator FrameCooldown()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        inputCD = false;
    }
    #endregion

    #region Rebinding

    public void DoRebind(string actionName, int rebindIdx)
    {
        // Get ref to script version of binding, validate
        InputAction actionToRebind = Controls.FindAction(actionName);
        if (actionToRebind == null || rebindIdx < 0)
        {
            Debug.Log($"[InputManager] Could not find action to rebind");
            return;
        }
        Debug.Log("Rebinding called");

        actionToRebind.Disable();

        // SO: Rebinding works by creating a task, preparing it, and then executing it, so this
        // will create the task to rebind
        var rebind = actionToRebind.PerformInteractiveRebinding(rebindIdx);

        // Asign events to happen when rebinding completes or cancels
        rebind.OnComplete(op =>
        {
            actionToRebind.Enable();
            op.Dispose();
        });
        rebind.OnCancel(op =>
        {
            actionToRebind.Enable();
            op.Dispose();
        });

        // This is where the rebinding actually happens
        rebind.Start();
    }

    /// <summary>
    /// Get the proper name for this binding
    /// </summary>
    /// <param name="actionName">Action name to utilize</param>
    /// <param name="bindingIdx">Index of the binding</param>
    /// <returns>Actual name of the action</returns>
    public static string GetBindingName(string actionName, int bindingIdx)
    {
        return Controls.asset.FindAction(actionName).GetBindingDisplayString(bindingIdx);
    }

    #endregion
}
