/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 16th, 2023
 * Last Edited - June 16th, 2023
 * Description - UI Element that rebinds M&K Inputs. 
 * Based on - https://youtu.be/TD0R5x0yL0Y
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Sirenix.OdinInspector;

public class RebindUI : MonoBehaviour
{
    [Header("Target Action")]

    [SerializeField] private InputActionReference inputActionRef;
    [SerializeField] private bool excludeMouse;
    [Tooltip("Index to try and rebind")]
    [SerializeField, Range(0, 15)] private int bindingIdx = 0;

    [Space(5)]
    [Tooltip("What keys are NOT ALLOWED to be chosen during rebinding")]
    [SerializeField] private string[] blacklistKeys;

    [Header("Binding Info")]
    [SerializeField, ReadOnly] private InputBinding targetBinding;
    /// <summary>
    /// Actual valid index to utilize
    /// </summary>
    private int targetIndex;
    private string actionName;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI bindingText;
    [SerializeField] private InputBinding.DisplayStringOptions displayOptions;

    #region Get Binding

    private void OnEnable()
    {
        if (inputActionRef != null)
        {
            GetTargetBind();
            UpdateUI();
        }
    }

    /// <summary>
    /// When validating on inspector change, update the UI elements accordingly for QoL
    /// </summary>
    private void OnValidate()
    {
        GetTargetBind();
        UpdateUI();
    }

    /// <summary>
    /// Get the actual bind that should be avaialble to rebind
    /// </summary>
    private void GetTargetBind()
    {
        if (inputActionRef == null)
        {
            targetBinding = default;
            actionName = "";
            targetIndex = 0;
            return;
        }
        else
        {
            actionName = inputActionRef.action.name;
        }

        if(bindingIdx < inputActionRef.action.bindings.Count)
        {
            targetIndex = bindingIdx;
            targetBinding = inputActionRef.action.bindings[targetIndex];
        }
        else
        {
            targetBinding = default;
        }
    }

    /// <summary>
    /// Update the UI automatically to match the target bind, if possible
    /// </summary>
    private void UpdateUI()
    {
        // build the appropriate string, setting to null if no action passed in
        string nameTxt;
        string bindingTxt;
        if(inputActionRef == null || targetBinding == null)
        {
            nameTxt = "NO ACTION FOUND";
            bindingTxt = "NULL";
        }
        else
        {
            nameTxt = actionName;

            if(Application.isPlaying && InputManager.Instance != null)
                bindingTxt = InputManager.GetBindingName(actionName, targetIndex);
            else
                bindingTxt = inputActionRef.action.GetBindingDisplayString(targetIndex, displayOptions);
        }

        // Update UI elements if they're available
        if(actionNameText!= null)
        {
            actionNameText.text = nameTxt;
        }
        if(bindingText != null)
        {
            bindingText.text = bindingTxt;
        }
    }

    #endregion

    #region Button Functions

    /// <summary>
    /// Rebind the binding for this action
    /// </summary>
    public void DoRebind()
    {
        //Debug.Log("Rebinding - not implemented");
        bindingText.text = "Awaiting Input";
        InputManager.Instance.DoRebind(actionName, targetIndex, blacklistKeys, OnValidate, OnValidate);
    }
    /// <summary>
    /// Reset the binding for this action
    /// </summary>
    public void DoReset()
    {
        InputManager.Instance.RevertKeybindings();
    }

    public void ForceUpdate()
    {
        OnValidate();
    }

    #endregion
}
