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
    [SerializeField] private AudioBusLoad[] buses;


    private void Start()
    {
        UpdateSettings();
    }

    private void UpdateSettings()
    {
        // update each pair's volume
        foreach(var busKey in buses)
        {
            float updatedVol = Mathf.Log10(PlayerPrefs.GetFloat(busKey.key, 0.5f)) * 20;
            // Update the actual volume bus]
            busKey.target.audioMixer.SetFloat(busKey.key, updatedVol);
        }
    }
}
