/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 12th, 2023
 * Last Edited - June 12th, 2023 by Ben Schuster
 * Description - Handles the UI screen that retrieves a random call for the player to use.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomCallUI : MonoBehaviour
{
    [Header("Functionality")]
    [SerializeField] private SecretConversationInteract interactRef;
    private Conversation loadedConveration;

    [Header("Visuals")]
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI nameBox;
    [SerializeField] private FlashProtocol flashOverlay;

    private AudioClipSO ringtone;
    AudioSource ringtoneSource;

    /// <summary>
    /// On enable, initialize it and load in data
    /// </summary>
    private void OnEnable()
    {
        if(ringtoneSource == null)
            ringtoneSource= GetComponent<AudioSource>();

        // Try to get conversation. If it fails, close immediately
        if (loadedConveration == null)
            loadedConveration = GetConversation();
        if (loadedConveration == null)
        {
            GetComponent<GameObjectMenu>().CloseButton();
            return;
        }

        // Load in the data
        if (portrait != null)
            portrait.sprite = loadedConveration.nonPennyCharacter.characterThumbnail;
        if (nameBox != null)
            nameBox.text = loadedConveration.nonPennyCharacter.characterName;
        ringtone = loadedConveration.nonPennyCharacter.ringtone;
        ringtone.PlayClip(ringtoneSource);

        // Begin flashing 
        flashOverlay.BeginFlash();
    }
    /// <summary>
    /// Stop flashing when turned off
    /// </summary>
    private void OnDisable()
    {
        flashOverlay.StopFlash();
    }

    /// <summary>
    /// Play the conversation, close this screen. Called via UI
    /// </summary>
    public void PlayConversation()
    {
        GetComponent<GameObjectMenu>().CloseButton();
        ConvoRefManager.instance.GetCallUI().OpenScreen(loadedConveration, interactRef.OnCallComplete);
        ringtoneSource.Stop();
    }

    /// <summary>
    /// Get a conversation from the manager. 
    /// </summary>
    /// <returns>New conversation</returns>
    public Conversation GetConversation()
    {
        return CallManager.instance.GetRandomAvailableConversation();
    }
}
