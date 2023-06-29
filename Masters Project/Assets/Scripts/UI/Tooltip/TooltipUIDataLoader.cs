/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 15th, 2023
 * Last Edited - June 15th, 2023 by Ben Schuster
 * Description - Handles loading tooltip data into UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUIDataLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text data;
    [SerializeField] private Image img;
    /// <summary>
    /// The data currently loaded into the object
    /// </summary>
    private TooltipSO loadedData;

    public delegate void OnPress(TooltipSO data);
    private OnPress onPress;

    [SerializeField] ChannelControlScheme onControlSchemeSwapChannel;

    /// <summary>
    /// Populate this UI element with relevant data
    /// </summary>
    /// <param name="d">Data to implement</param>
    /// <param name="pressExecute">Function to execute on button press</param>
    public void LoadInData(TooltipSO d, OnPress pressExecute = null)
    {
        onPress = pressExecute;
        loadedData= d;

        // If invalid data passed in, clear
        if(loadedData == null)
        {
            ResetFields();
            return;
        }
        
        if (title != null)
            title.text = d.titleText;
        if (data != null)
        {
            if(d.inputReference.Length > 0)
            {
                string contentRef = d.GetSavedText();
                string temp = "";
                int inputIdx = 0;
                // iterate through content, adding any input bindings when necessary
                for (int i = 0; i < contentRef.Length; i++)
                {
                    if (contentRef[i] == TooltipManager.instance.inputDelimiter)
                    {
                        temp += InputManager.Instance.ActionKeybindLookup(d.inputReference[inputIdx]);
                        inputIdx++;
                    }
                    else
                    {
                        temp += contentRef[i];
                    }
                }
                data.text = temp;
            }
            else
            {
                data.text = d.GetSavedText();
            }
        }
            
       // if(img != null)
       //     img.sprite = d.icon
    }

    /// <summary>
    /// Reset all fields  for this display
    /// </summary>
    public void ResetFields()
    {
        onPress = null;
        loadedData = null;

        if (title != null)
            title.text = "";
        if (data != null)
            data.text = "";
        // if(img != null)
        //     img.sprite = d.icon
    }

    /// <summary>
    /// When the button is pressed, invoke the passed in action if any
    /// </summary>
    public void OnButtonPress()
    {
        onPress?.Invoke(loadedData);
    }

    private void OnEnable()
    {
        onControlSchemeSwapChannel.OnEventRaised += InstantReloadTooltip;
    }

    private void OnDisable()
    {
        onControlSchemeSwapChannel.OnEventRaised -= InstantReloadTooltip;
    }

    /// <summary>
    /// Instantly reload the body text with the appropriate input action string
    /// </summary>
    /// <param name="c"></param>
    private void InstantReloadTooltip(InputManager.ControlScheme c)
    {
        if (data != null && loadedData.inputReference.Length > 0)
        {
            string contentRef = loadedData.GetSavedText();
            string temp = "";
            int inputIdx = 0;
            // iterate through content, adding any input bindings when necessary
            for (int i = 0; i < contentRef.Length; i++)
            {
                if (contentRef[i] == TooltipManager.instance.inputDelimiter)
                {
                    temp += InputManager.Instance.ActionKeybindLookup(loadedData.inputReference[inputIdx]);
                    inputIdx++;
                }
                else
                {
                    temp += contentRef[i];
                }
            }
            data.text = temp;
        }
    }
}
