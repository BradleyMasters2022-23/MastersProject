/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 8th, 2022
 * Last Edited - December 12, 2022 by Ben Schuster
 * Description - Manage all the audio - needs vastly more work spring semester
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

[System.Serializable]
struct AudioBusLoad
{
    public AudioMixerGroup target;
    public string key;
}

public class AudioManager : MonoBehaviour
{
    [Tooltip("Channel to check for settings change")]
    [SerializeField] private ChannelVoid onSettingsChangedChannel;

    [SerializeField] private AudioBusLoad[] buses;


    private void Awake()
    {
        // load in settings
        //UpdateSettings();
    }

    private void Update()
    {
        //if(masterVolume != AudioListener.volume)
        //    UpdateSettings();
    }

    private void UpdateSettings()
    {
        //masterVolume = Settings.masterVolume;
        //AudioListener.volume = masterVolume;

        // update each pair's volume
        foreach(var busKey in buses)
        {
            // Clamp audio between bounds, save
            PlayerPrefs.SetFloat(busKey.key, 0);

            // Update the actual volume bus
            busKey.target.audioMixer.SetFloat(busKey.key, PlayerPrefs.GetFloat(busKey.key, 0));
        }
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
