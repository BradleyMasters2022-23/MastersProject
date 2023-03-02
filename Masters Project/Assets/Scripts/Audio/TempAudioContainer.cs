using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class TempAudioContainer : MonoBehaviour
{
    public AudioClipSO clip;
    private AudioSource audioSource;
    private float lifetime;

    private void Awake()
    {
        // Get audio source, detatch from any parent
        audioSource = GetComponent<AudioSource>();
        transform.parent = null;

        audioSource.minDistance = 0;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    
}
