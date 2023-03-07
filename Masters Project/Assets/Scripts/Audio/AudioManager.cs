/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 12, 2022
 * Last Edited - December 12, 2022 by Ben Schuster
 * Description - Manage all the audio - needs vastly more work spring semester
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Channel to check for settings change")]
    [SerializeField] private ChannelVoid onSettingsChangedChannel;
    private float masterVolume;

    public GameObject tempAudioContainer;

    private void Awake()
    {
        
        // load in settings
        UpdateSettings();
    }

    private void Update()
    {
        if(masterVolume != AudioListener.volume)
            UpdateSettings();
    }

    private void UpdateSettings()
    {
        masterVolume = Settings.masterVolume;
        AudioListener.volume = masterVolume;
    }

    private void OnEnable()
    {
        if(onSettingsChangedChannel!= null) 
            onSettingsChangedChannel.OnEventRaised += UpdateSettings;
    }
    private void OnDisable()
    {
        if (onSettingsChangedChannel != null)
            onSettingsChangedChannel.OnEventRaised -= UpdateSettings;
    }
}
