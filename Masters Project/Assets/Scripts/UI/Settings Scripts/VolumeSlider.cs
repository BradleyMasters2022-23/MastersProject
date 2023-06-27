using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class VolumeSlider : MonoBehaviour
{
    [InfoBox("Volume Range: [-80 , 0]")]
    [SerializeField, Range(0.0001f, 1)] private float defaultVolume;
    [SerializeField, Required] private AudioMixerGroup targetBus;
    [SerializeField, Required] private AudioClipSO testClip;
    [SerializeField, Required] private string keyName;
    [SerializeField, Required] private Slider slider;

    private float volume;

    private const float lowerVolumeLimit = 0.0001f;
    private const float upperVolumeLimit = 1f;

    /// <summary>
    /// Update the volume bus. In start becaues it doesn't work in Awake
    /// </summary>
    private void OnEnable()
    {
        float retrievedVal = PlayerPrefs.GetFloat(keyName, defaultVolume);
        //Debug.Log("retrieved value : " + retrievedVal);
        SetVolume(retrievedVal);
    }

    /// <summary>
    /// Set volume in code. Updates the slider.
    /// </summary>
    /// <param name="vol">new volume to set it to</param>
    private void SetVolume(float vol)
    {
        // Update slider visuals
        if (slider != null)
        {
            slider.minValue = lowerVolumeLimit;
            slider.maxValue = upperVolumeLimit;
            slider.value = vol;
            //Debug.Log($"Slider value updated from {vol} to {slider.value}");
        }

        // call slider func that sets saving and bus
        SliderUpdate();
    }

    /// <summary>
    /// Update the volume from the slider.
    /// </summary>
    public void SliderUpdate()
    {
        PlayerPrefs.SetFloat(keyName, slider.value);

        // adjust the slider value to the converted value
        volume = Mathf.Log10(slider.value) * 20;
        targetBus.audioMixer.SetFloat(keyName, volume);
    }

    /// <summary>
    /// Play a test of the clip.
    /// </summary>
    public void PlayTest()
    {
        if (testClip != null)
        {
            testClip.volumeBus = targetBus;
            testClip.PlayClip();
        }
    }

    /// <summary>
    /// Revert value to default value
    /// </summary>
    public void RevertToDefault()
    {
        PlayerPrefs.DeleteKey(keyName);
        SetVolume(defaultVolume);
    }
}
