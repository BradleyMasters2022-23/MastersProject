using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputTextSwap : MonoBehaviour
{
    [Tooltip("The target string to display")]
    [SerializeField, TextArea] string displayString;
    [SerializeField] TextMeshProUGUI displayArea;

    [Tooltip("Action to reference")]
    [SerializeField] InputActionReference action;
    [Tooltip("Channel called when control scheme changes")]
    [SerializeField] ChannelControlScheme onSchemeChange;

    private void OnEnable()
    {
        onSchemeChange.OnEventRaised += UpdateInputText;
        UpdateInputText();
    }
    private void OnDisable()
    {
        onSchemeChange.OnEventRaised -= UpdateInputText;
    }

    /// <summary>
    /// Update the text with the correct input lookup
    /// </summary>
    /// <param name="c"></param>
    private void UpdateInputText(InputManager.ControlScheme c = default)
    {
        //Debug.Log($"{gameObject.name} called to update text input");
        string temp = "";
        // iterate through content, adding any input bindings when necessary
        for (int i = 0; i < displayString.Length; i++)
        {
            if (displayString[i] == InputManager.Instance.inputDelimiter)
            {
                temp += InputManager.Instance.ActionKeybindLookup(action);
                //Debug.Log($"Retrieved input: {InputManager.Instance.ActionKeybindLookup(action)}");
            }
            else
            {
                temp += displayString[i];
            }
        }
        displayArea.text = temp;
    }
}
