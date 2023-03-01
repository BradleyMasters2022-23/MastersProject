/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22nd, 2022
 * Last Edited - February 22nd, 2022 by Ben Schuster
 * Description - Start of backend work for sound system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum VolumeBus
{
    Effect, 
    UI, 
    Music
}


[CreateAssetMenu(fileName = "New Audioclip Data", menuName = "Core/Audioclip Data")]
public class AudioClipSO : ScriptableObject
{
    [InfoBox("Insert all potential audio clips you want to be played when this sound is called." +
        " When this sound is called, it will randomly select one of these clips to play. " +
        "The higher the weight of a clip, the more likely it will be selected.")]
  
    [SerializeField] private GenericWeightedList<AudioClip> clips;
    
    [Tooltip("Base volume of this audio clip.")]
    public float baseVolume = 1;
    [Tooltip("For each repeat of this clip, how much the volume get reduced by.")]
    public float diminishingReturnRate = 0.05f;
    [Tooltip("Minimum volume for this clip when applied with diminishing returns.")]
    public float diminishedCap = 0.2f;

    [Tooltip("Whether or not this audio clip should play looped.")]
    public bool loop = false;

    [EnumToggleButtons] public VolumeBus volumeType;
    
    public void PlayAudio(Transform parent)
    {
        PlayAudio(parent.position);
    }

    public void PlayAudio(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clips.Pull(), position, baseVolume);
    }

    /// <summary>
    /// Get the volume diminished based on count of this playing
    /// </summary>
    /// <param name="count">The current count of this audio effect</param>
    /// <returns>The adjusted volume float</returns>
    public float DiminishedVolume(int count)
    {
        return Mathf.Clamp(baseVolume - (Mathf.Abs(diminishingReturnRate) * count), diminishedCap, 10);
    }
}
