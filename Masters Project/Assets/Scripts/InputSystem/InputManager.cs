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

[System.Serializable]
public struct BindingTextOverride
{
    public string originalText;
    public string newText;
}

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
        GenerateOverrideDict(textOverrides);
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
        Debug.Log("Enabled map of " + newMap.name);
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

    [Tooltip("Inputs that cannot be used in rebinding")]
    [SerializeField] private string[] globalKeybindBlacklist;

    [Tooltip("List of all actions that can be rebound")]
    [SerializeField] InputActionReference[] bindableActions;
    /// <summary>
    /// The target action being overriden
    /// </summary>
    private InputAction targetAction;
    /// <summary>
    /// The target binding index for the target action
    /// </summary>
    private int targetIdx;
    /// <summary>
    /// Original path of the target action before the binding
    /// </summary>
    private string originalPath;
    /// <summary>
    /// Whether the target action's effective path was an override before the binding
    /// </summary>
    private bool originalOverride;


    public delegate void VoidFunc();

    public void DoRebind(string actionName, int rebindIdx, string[] keyBlacklist, VoidFunc onComplete = null, VoidFunc onCancel = null)
    {
        // Get ref to script version of binding, validate
        InputAction actionToRebind = Controls.FindAction(actionName);
        if (actionToRebind == null || rebindIdx < 0)
        {
            Debug.Log($"[InputManager] Could not find action to rebind");
            return;
        }

        actionToRebind.Disable();

        // store the target action to be rebind
        targetAction = actionToRebind;
        targetIdx = rebindIdx;
        originalPath = actionToRebind.bindings[targetIdx].effectivePath;
        originalOverride = actionToRebind.bindings[targetIdx].hasOverrides;

        // SO: Rebinding works by creating a task, preparing it, and then executing it, so this
        // will create the task to rebind
        var rebind = actionToRebind.PerformInteractiveRebinding(rebindIdx);
        // Asign events to happen when rebinding completes or cancels
        rebind.OnComplete(op =>
        {
            actionToRebind.Enable();
            op.Dispose();
            ValidateNewKeybind(onComplete);
            //InputAlreadyTaken(actionToRebind.bindings[rebindIdx]);
            onComplete?.Invoke();
        });
        rebind.OnCancel(op =>
        {
            actionToRebind.Enable();
            op.Dispose();
            onCancel?.Invoke();
            Debug.Log("Rebind canceled");
        });

        // Allow canceling via escape key
        rebind.WithCancelingThrough("<Keyboard>/escape");


        // Restrict keybinding to mouse and keyboard
        rebind.WithControlsHavingToMatchPath("<Keyboard>");
        rebind.WithControlsHavingToMatchPath("<Mouse>");
        rebind.WithTimeout(8f);

        // Apply global and passed in blacklists
        foreach(string s in globalKeybindBlacklist)
        {
            rebind.WithControlsExcluding(s);
        }
        foreach(string s in keyBlacklist)
        {
            rebind.WithControlsExcluding(s);
        }


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

    /// <summary>
    /// Validate the new keybind to ensure its not conflicting with others
    /// </summary>
    public void ValidateNewKeybind(VoidFunc onComplete)
    {
        //Debug.Log("Override test applied " + Controls.FindAction(targetAction.id.ToString()).bindings[targetIdx].hasOverrides);
        //Debug.Log("New binding path => " + Controls.FindAction(targetAction.id.ToString()).bindings[targetIdx].effectivePath);

        // Update reference to the C# reference instead of SO reference, as the rebind was executed on C# 
        targetAction = Controls.FindAction(targetAction.id.ToString());
        
        // loop through each action
        foreach (var action in bindableActions)
        {
            // Ignore the target action that was just rebound
            if (action.action.id == targetAction.id)
                continue;

            // use i to track index because im too lazy to convert this foreach to a proper for loop
            int i = 0;
            // Check each binding in the action
            foreach (var bind in Controls.FindAction(action.action.id.ToString()).bindings)
            {
                // if conflict is found, ask the player to validate the new change
                if(bind.effectivePath == targetAction.bindings[targetIdx].effectivePath)
                {
                    // update prompt so player knows whats going on
                    FindObjectOfType<TwoChoiceMenu>(true).SetMainPrompt(
                        $"{targetAction.GetBindingDisplayString(targetIdx)} is already in use by {action.action.name}." +
                        $"\nIf you continue, these bindings will be switched.");

                    // Open the two choice window always in rebind menu. Pass in the two options for the player
                    FindObjectOfType<TwoChoiceMenu>(true).Open(
                        ()=> SwapControls(Controls.FindAction(action.action.id.ToString()), i, onComplete), 
                        "Swap Keybindings", 
                        ()=> UndoRebind(onComplete), 
                        "Undo Keybind");

                    break;
                }
                i++;
            }
        }
    }

    /// <summary>
    /// Swap bindings by applying new override to this option
    /// </summary>
    private void SwapControls(InputAction actionToSwap, int idx, VoidFunc onComplete)
    {
        actionToSwap.ApplyBindingOverride(idx, originalPath);

        Debug.Log($"Swapped controls: {actionToSwap.name}'s new effective path is {actionToSwap.bindings[idx].effectivePath}");
        //onComplete?.Invoke();

        RebindUI[] temp = FindObjectsOfType<RebindUI>();
        foreach(var t in temp)
            t.ForceUpdate();
    }

    /// <summary>
    /// Reapply the previous binding status onto the previous target action
    /// </summary>
    private void UndoRebind(VoidFunc onComplete)
    {
        if(originalOverride)
            targetAction.ApplyBindingOverride(targetIdx, originalPath);
        else
            targetAction.RemoveBindingOverride(targetIdx);
        Debug.Log($"Undid bind: {targetAction.name}'s new effective path is {targetAction.bindings[targetIdx].effectivePath}");
        onComplete?.Invoke();

        RebindUI[] temp = FindObjectsOfType<RebindUI>();
        foreach (var t in temp)
            t.ForceUpdate();
    }

    #endregion

    #region ActionLookup

    [SerializeField] public BindingTextOverride[] textOverrides;
    private Dictionary<string, string> overrideLookup;
    /// <summary>
    /// Build the dictionary used for text overrides
    /// </summary>
    /// <param name="overrideData">Data to generate override dict with</param>
    private void GenerateOverrideDict(BindingTextOverride[] overrideData)
    {
        overrideLookup = new Dictionary<string, string>();
        foreach (var data in overrideData)
        {
            overrideLookup.Add(data.originalText, data.newText);
        }
    }
    /// <summary>
    /// Get the string representation of an action, control type sensitive
    /// </summary>
    /// <param name="actionRef">Input action to lookup</param>
    /// <returns>String of the binding for the action</returns>
    public string ActionKeybindLookup(InputActionReference actionRef)
    {
        // get the actual action
        InputAction action = actionRef.action;

        // Do some edge checking
        if (action == null)
            return "ERR-NOACTION";
        else if (action.bindings.Count == 0)
            return "UNBOUND";

        string temp;
        switch(CurrControlScheme)
        {
            case ControlScheme.KEYBOARD:
                {
                    // Check if input is composite. Use 0 because 0 is always M&K 
                    if (action.bindings[0].isComposite)
                    {
                        // get exact composite, then loop through all composits, combinding them
                        temp = "";
                        for (int i = 0; i < action.bindings.Count; i++)
                        {
                            if (action.bindings[i].isComposite)
                            {
                                int j = i + 1;
                                while (j < action.bindings.Count && action.bindings[j].isPartOfComposite)
                                {
                                    temp += action.GetBindingDisplayString(j);
                                    j++;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        temp = action.GetBindingDisplayString(group: "KeyboardMouse");
                    }
                    break;
                }
            case ControlScheme.CONTROLLER:
                {
                    // shouldnt be any composits for controllers
                    Debug.Log("Attempting to get gamepad");
                    temp = ":" + action.GetBindingDisplayString(group: "Gamepad") + ":";
                    break;
                }
            default:
                {
                    temp = "NOT FOUND";
                    break;
                }
        }

        // if theres an override, apply it
        if(overrideLookup.ContainsKey(temp))
            temp = overrideLookup[temp];

        return temp;
    }

    #endregion
}
