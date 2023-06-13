/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 9th, 2023
 * Last Edited - June 9th, 2023 by Ben Schuster
 * Description - Manages the 'call dots' on the UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContactLogUI : MonoBehaviour
{
    [Header("Image States")]

    private Conversation convoRef;
    private Image mImage;
    [SerializeField] private Sprite uncheckedSprite;
    [SerializeField] private Sprite checkedSprite;
    [SerializeField] private Sprite incomingSprite;

    [SerializeField] private GameObject interactButton;

    [Header("Flashing Data")]
    [SerializeField] private FlashProtocol flashComponent;

    [Header("Text Data")]
    [SerializeField] private TextMeshProUGUI descriptor;
    [SerializeField] private string uncheckedText;
    [SerializeField] private string incomingCallText;

    private int maxDescriptorLength = 50;

    #region Convo Status

    /// <summary>
    /// Load a conversation into this log option
    /// </summary>
    /// <param name="c"></param>
    public void SetConvo(Conversation c)
    {
        mImage = GetComponentInChildren<Image>();
        convoRef = c;
    }
    /// <summary>
    /// Check status of the loaded conversation
    /// </summary>
    public void CheckConvoStatus()
    {
        if (convoRef == null) return;

        
        flashComponent.StopFlash();
        if (CallManager.instance.HasNewCall(convoRef))
        {
            SetIncoming();
        }
        else if (CallManager.instance.CallInSave(convoRef))
        {
            SetFound();
        }
        else
        {
            SetNotFound();
        }
    }

    private void OnEnable()
    {
        CheckConvoStatus();
    }

    #endregion

    #region Status Functionality

    /// <summary>
    /// Set visual status to be found
    /// </summary>
    private void SetFound()
    {
        mImage.sprite = checkedSprite;

        if (descriptor != null)
        {
            // Get the descriptor. If too long, cut it off with 3 extra dots
            string display = convoRef.lines[0].text;
            if (display.Length > maxDescriptorLength)
            {
                display = display.Substring(0, maxDescriptorLength - 3);
                display += "...";
            }
            descriptor.text = display;
        }
    }
    /// <summary>
    /// Set visual status to be found
    /// </summary>
    private void SetNotFound()
    {
        mImage.sprite = uncheckedSprite;

        if(interactButton != null)
            interactButton.SetActive(false);

        if (descriptor != null)
            descriptor.text = uncheckedText;
    }
    /// <summary>
    /// Set visual status to be an incoming call 
    /// </summary>
    private void SetIncoming()
    {
        mImage.sprite = incomingSprite;
        flashComponent.BeginFlash();

        if (descriptor != null)
            descriptor.text = incomingCallText;
    }
    #endregion

    #region Screenflow Funcs

    /// <summary>
    /// Go to this loaded call
    /// </summary>
    public void OpenCall()
    {
        //Debug.Log("Going to new call");

        if (ConvoRefManager.instance == null || convoRef == null) return;

        ConvoRefManager.instance.GetCallUI().OpenScreen(convoRef, ConvoRefManager.instance.CheckCallStatuses);
        return;
    }

    #endregion
}
