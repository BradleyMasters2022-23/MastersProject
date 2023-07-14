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
using TMPro;
using System;
using UnityEngine.Windows;

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
    /// reference to the main player input system. Should be attached
    /// </summary>
    private PlayerInput playerInput;

    /// <summary>
    /// Prepare singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Controls = new GameControls();
        playerInput = GetComponent<PlayerInput>();
        GenerateOverrideDict(textOverrides);
    }

    /// <summary>
    /// Call all starts.
    /// For organization, I moved all region starts into their own functions and have them all called here.
    /// </summary>
    private void Start()
    {
        SchemeSwapStart();
    }

    #endregion

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
    [ReadOnly] public ControlScheme debugControlScheme;
    /// <summary>
    /// The string name of the mouse and keyboard scheme. Should match whats in GameplayControls.
    /// </summary>
    private const string mouseKeyboardSchemeName = "KeyboardMouse";
    /// <summary>
    /// The string name of the controller scheme. Should match whats in GameplayControls.
    /// </summary>
    private const string controllerSchemeName = "Gamepad";

    [Tooltip("Channel thats called when controller inputs and devices are changed.")]
    [SerializeField] private ChannelControlScheme controlSchemeSwapChannel;

    /// <summary>
    /// Unique initialization for swapping control schemes at runtime
    /// </summary>
    private void SchemeSwapStart()
    {
        // Prepare controls
        playerInput.onControlsChanged += OnControlsChanged;
        controlSchemeSwapChannel?.RaiseEvent(CurrControlScheme);
    }

    /// <summary>
    /// remove input controls and save bindings. Only do if it was properly initialized
    /// </summary>
    private void OnDisable()
    {
        if (Instance == this)
        {
            playerInput.onControlsChanged -= OnControlsChanged;
            SaveKeybindings();
        }
    }

    /// <summary>
    /// Swap the control schemes based on the current device. 
    /// </summary>
    /// <param name="input">Input data</param>
    private void OnControlsChanged(PlayerInput input)
    {
        if (GameManager.instance.CurrentState == GameManager.States.LOADING) return;
        //Debug.Log("Trying to change controls : " + input.currentControlScheme);

        // prepare target. use keyboard as default
        ControlScheme target = ControlScheme.KEYBOARD;

        // parse scheme name to get relevant enum
        switch(input.currentControlScheme)
        {
            case controllerSchemeName:
                {
                    target = ControlScheme.CONTROLLER;
                    break;
                }
            case mouseKeyboardSchemeName:
                {
                    target = ControlScheme.KEYBOARD;
                    break;
                }
            default:
                {
                    Debug.LogError($"[INPUTMANAGER] Controls change detected but the scheme wasn't recognized" +
                        $". Scheme is: {input.currentControlScheme}");
                    break;
                }
        }

        // set the target directily. 
        SetDirectControlscheme( target );
    }
    /// <summary>
    /// Directly set the control scheme. Should rarely be called by anything aside from input manager. 
    /// </summary>
    /// <param name="scheme">The enuim scheme to set</param>
    public void SetDirectControlscheme(ControlScheme scheme)
    {
        // do something for each input type. 
        switch (scheme)
        {
            case ControlScheme.CONTROLLER:
                {
                    SetToController();
                    break;
                }
            case ControlScheme.KEYBOARD:
                {
                    SetToKeyboard();
                    break;
                }
            default:
                {
                    Debug.LogError($"[INPUTMANAGER] Direct control scheme was set but no setting was prepared!" +
                        $"Was set to {scheme}");
                    break;
                }
        }

        // raise event to update game based on controls
        controlSchemeSwapChannel?.RaiseEvent(CurrControlScheme);
    }

    /// <summary>
    /// Swap the control scheme to keyboard and perform any necessary functions
    /// </summary>
    private void SetToKeyboard()
    {
        //Debug.Log("Switching to keyboard");
        CurrControlScheme = ControlScheme.KEYBOARD;
        debugControlScheme = CurrControlScheme;
    }
    /// <summary>
    /// Swap the control scheme to controller and perform any necessary functions
    /// </summary>
    private void SetToController()
    {
        //Debug.Log("Switching to controller");
        CurrControlScheme = ControlScheme.CONTROLLER;
        debugControlScheme = CurrControlScheme;
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
        Controls.UI.Back.Disable();
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
            Controls.UI.Back.Enable();
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
                    Debug.Log("Conflict detected");
                    // update prompt so player knows whats going on
                    PlayerTarget.p.GetComponentInChildren<TwoChoiceMenu>(true).SetMainPrompt(
                        $"{targetAction.GetBindingDisplayString(targetIdx)} is already in use by {action.action.name}." +
                        $"\n\nSwap keybinds for {targetAction.name} and {action.action.name}?");

                    // Open the two choice window always in rebind menu. Pass in the two options for the player
                    PlayerTarget.p.GetComponentInChildren<TwoChoiceMenu>(true).Open(
                        ()=> SwapControls(Controls.FindAction(action.action.id.ToString()), i, onComplete), 
                        "Swap Keybindings", 
                        ()=> UndoRebind(onComplete), 
                        "Cancel");

                    return;
                }
                i++;
            }
        }
        // since it returns earlier on conflict, this only happens if NO conflicts
        Controls.UI.Back.Enable();
        UpdateUIElements();
    }

    /// <summary>
    /// Swap bindings by applying new override to this option
    /// </summary>
    private void SwapControls(InputAction actionToSwap, int idx, VoidFunc onComplete)
    {
        actionToSwap.ApplyBindingOverride(idx, originalPath);

        Debug.Log($"Swapped controls: {actionToSwap.name}'s new effective path is {actionToSwap.bindings[idx].effectivePath}");
        //onComplete?.Invoke();

        Controls.UI.Back.Enable();
        UpdateUIElements();
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

        Controls.UI.Back.Enable();
        UpdateUIElements();
    }

    /// <summary>
    /// Revert all keybindings. Force update for any UI elements
    /// </summary>
    public void RevertKeybindings()
    {
        Controls.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(keybindDataKey);

        UpdateUIElements();
    }

    private void UpdateUIElements()
    {
        RebindUI[] temp = FindObjectsOfType<RebindUI>();
        foreach (var t in temp)
            t.ForceUpdate();

        controlSchemeSwapChannel.RaiseEvent();
    }

    #endregion

    #region KeybindingSaving

    private const string keybindDataKey = "keybindOverrides";

    public void LoadKeybindings()
    {
        string data = PlayerPrefs.GetString(keybindDataKey);

        if(data != null)
            Controls.LoadBindingOverridesFromJson(data);
    }

    private void OnEnable()
    {
        LoadKeybindings();
        UpdateUIElements();
    }
    
    public void SaveKeybindings()
    {
        PlayerPrefs.SetString(keybindDataKey, Controls.SaveBindingOverridesAsJson());
    }

    #endregion

    #region ActionLookup

    [SerializeField] public BindingTextOverride[] textOverrides;
    private Dictionary<string, string> overrideLookup;

    [SerializeField] private InputBinding.DisplayStringOptions displayOptions;

    [SerializeField] private TMP_SpriteAsset promptSheet;

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
        InputAction action = Controls.FindAction(actionRef.action.id.ToString());

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
                                    temp += action.GetBindingDisplayString(j, displayOptions);
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
                    temp = "";
                    int compositesPassed = 0;

                    Debug.Log("Trying to get : " + action.name);

                    // loop through all bindings, looking for the first available gamepad
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        try
                        {
                            // if a composite, add to count. composites shouldnt be counted so just go to next
                            if (action.bindings[i].isComposite)
                            {
                                compositesPassed++;
                                continue;
                            }

                            // make sure no composite, and check if its a gamepad interaction
                            if (!action.bindings[i].isComposite && action.bindings[i].groups.Contains("Gamepad"))
                            {
                                // add interaction name
                                if (action.bindings[i].interactions != null)
                                    temp += action.bindings[i].effectiveInteractions + " ";

                                Debug.Log("Trying to get control named : " + action.controls[i - compositesPassed].name + " at index " + (i - compositesPassed));

                                // on lookup, make sure to subtract the composites passed to convert from bindings to controls properly
                                temp += "<sprite="
                                    + promptSheet.GetSpriteIndexFromName(action.controls[i-compositesPassed].name)
                                    + ">";

                                break;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.Log($"Error while finding input {action.name} : \n{e}");
                            temp = "ERR";
                        }
                    }
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

        //Debug.Log($"Request for {actionRef.action.name}, returning {temp}");
        return temp;
    }

    #endregion
}
