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
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audioclip Data", menuName = "Core/Audioclip Data")]
public class AudioClipSO : ScriptableObject
{
    [InfoBox("Insert all potential audio clips you want to be played when this sound is called." +
        " When this sound is called, it will randomly select one of these clips to play. " +
        "The higher the weight of a clip, the more likely it will be selected.")]
  
    [SerializeField] private GenericWeightedList<AudioClip> clips;

    [Tooltip("Which volume bus to play this audio within.")]
    public AudioMixerGroup volumeType;

    [Header("=== Volume ===")]

    [Tooltip("Base volume of this audio clip.")]
    public float volume = 1;
    [Tooltip("Possible pitches to play audio at.")]
    [MinMaxSlider(minValue:0.1f, maxValue:3, ShowFields = true)] 
    public Vector2 pitch;


    [SerializeField] private GenericWeightedList<float> pitches;


    [Tooltip("Whether or not this audio clip should play looped.")]
    public bool loop = false;

    [Header("=== Distance ===")]
    [Tooltip("How 3D is this audio. 1 is full 3D.")]
    [Range(0, 1)] public float spatialBlend = 1;
    [Tooltip("How the distance scales, if being used. Linear recommended.")]
    public AudioRolloffMode distanceMode = AudioRolloffMode.Linear;
    [Tooltip("Minimum distance required to hear the audio at max volume.")]
    public float minDistance = 0;
    [Tooltip("Maximum distance required to hear the audio at all.")]
    public float maxDistance = 75;
    [Tooltip("Intensity of doppler. Determines pitch based on distance.")]
    [Range(0, 5)] public float doppler;
    
    public AudioClip GetClip()
    {
        return clips.Pull();
    }
    public float GetPitch()
    {
        return Random.Range(pitch.x, pitch.y);
    }
}
