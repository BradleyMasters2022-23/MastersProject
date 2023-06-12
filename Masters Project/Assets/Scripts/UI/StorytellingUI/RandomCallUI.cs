using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomCallUI : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI nameBox;
    [SerializeField] private FlashProtocol flashOverlay;

    private Conversation loadedConveration;

    private AudioClipSO ringtone;
    AudioSource ringtoneSource;

    private void OnEnable()
    {
        if(ringtoneSource == null)
            ringtoneSource= GetComponent<AudioSource>();

        if (loadedConveration == null)
            loadedConveration = GetConversation();

        ringtone.PlayClip(ringtoneSource);

        flashOverlay.BeginFlash();
    }

    private void OnDisable()
    {
        flashOverlay.StopFlash();
    }

    public void PlayConversation()
    {
        ConvoRefManager.instance.GetCallUI().OpenScreen(loadedConveration);
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
