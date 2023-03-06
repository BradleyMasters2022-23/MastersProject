using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class VolumeSlider : MonoBehaviour
{
    [InfoBox("Volume Range: [-80 , 0]")]
    [SerializeField] private float defaultVolume;
    [SerializeField] private AudioMixerGroup targetBus;
    [SerializeField] private AudioClipSO testClip;
    [SerializeField] private string keyName;
    [SerializeField] private Slider slider;

    private float volume = upperVolumeLimit;

    private const float lowerVolumeLimit = -80f;
    private const float upperVolumeLimit = 0f;

    /// <summary>
    /// Get the volume key and update the slider
    /// </summary>
    private void Awake()
    {
        SetVolume(PlayerPrefs.GetFloat(keyName, defaultVolume));
    }
    /// <summary>
    /// Update the volume bus. In start becaues it doesn't work in Awake
    /// </summary>
    private void Start()
    {
        targetBus.audioMixer.SetFloat(keyName, volume);
    }

    /// <summary>
    /// Set volume in code. Updates the slider.
    /// </summary>
    /// <param name="vol">new volume to set it to</param>
    private void SetVolume(float vol)
    {
        // Clamp audio between bounds, save
        volume = Mathf.Clamp(vol, lowerVolumeLimit, upperVolumeLimit);
        PlayerPrefs.SetFloat(keyName, volume);

        // Update slider visuals
        slider.minValue = lowerVolumeLimit;
        slider.maxValue = upperVolumeLimit;
        slider.value = volume;

        // Update the actual volume bus
        targetBus.audioMixer.SetFloat(keyName, volume);
    }

    /// <summary>
    /// Update the volume from the slider.
    /// </summary>
    public void SliderUpdate()
    {
        volume = slider.value;
        PlayerPrefs.SetFloat(keyName, volume);
        targetBus.audioMixer.SetFloat(keyName, volume);
    }

    /// <summary>
    /// Play a test of the clip.
    /// </summary>
    public void PlayTest()
    {
        if (testClip != null)
        {
            Debug.Log("Audio Played");
            testClip.volumeBus = targetBus;
            transform.PlayClip(testClip);
        }
    }

    /// <summary>
    /// Revert value to default value
    /// </summary>
    public void RevertToDefault()
    {
        SetVolume(defaultVolume);
    }
}
