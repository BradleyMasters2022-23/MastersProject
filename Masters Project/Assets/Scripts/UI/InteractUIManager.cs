/* ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 8th, 2023
 * Last Edited - June 8th, 2023 by Ben 
 * Description - Handles the UI element for interact prompt being shown and hidden when an interact is in range
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class InteractUIManager : MonoBehaviour
{
    [Tooltip("Channel used when for sending interact change prompt")]
    [SerializeField] private ChannelBool onInteractChange;
    [Tooltip("Prompt object itself that should be shown/hidden when in range of an interact")]
    [SerializeField] private GameObject promptContainer;

    /// Make sure the prompt is hidden by defualt
    /// </summary>
    public void Awake()
    {
        promptContainer.SetActive(false);
    }

    /// <summary>
    /// Subscribe to the channel
    /// </summary>
    private void OnEnable()
    {
        onInteractChange.OnEventRaised += UpdatePrompt;
    }
    /// <summary>
    /// Unsubscribe to the channel
    /// </summary>
    private void OnDisable()
    {
        onInteractChange.OnEventRaised -= UpdatePrompt;

    }

    /// <summary>
    /// Update the prompt
    /// </summary>
    /// <param name="show">Whether or not to show it</param>
    private void UpdatePrompt(bool show)
    {
        promptContainer.SetActive(show);
    }
}
