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

public enum VolumeSettings
{
    Effect, 
    UI, 
    Music
}

[System.Serializable]
public class SoundOverride
{
    public AudioClipSO source;
    [Range(0, 25)] public float volumeMod = 1;

    public void PlayAudio(Transform parent)
    {
        PlayAudio(parent.position);
    }

    public void PlayAudio(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(source.clip, position, source.baseVolume * volumeMod);
    }
}

[CreateAssetMenu(fileName = "New Audioclip Data", menuName = "Core/Audioclip Data")]
public class AudioClipSO : ScriptableObject
{
    public AudioClip clip;
    public float baseVolume = 1;
    public VolumeSettings volumeType;
    
    public void PlayAudio(Transform parent)
    {
        PlayAudio(parent.position);
    }

    public void PlayAudio(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position, baseVolume);
    }
}
