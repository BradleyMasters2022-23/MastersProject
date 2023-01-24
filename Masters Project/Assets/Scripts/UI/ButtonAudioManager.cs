/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 31th, 2022
 * Last Edited - December 8th, 2022 by Ben Schuster
 * Description - Hold audio data for manager
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip onHoverSound;
    [SerializeField] float hoverVolume = 0.5f;

    [SerializeField] private AudioClip onPressSound;
    [SerializeField] private float pressVolume = 0.5f;

    private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        if(onHoverSound != null)
        {
            source.Stop();
            source.volume= hoverVolume;
            source.clip = onHoverSound;
            source.Play();
        }
    }

    public void PlayClickSound()
    {
        if (onPressSound != null)
        {
            source.Stop();
            source.volume = pressVolume;
            source.clip = onPressSound;
            source.Play();
        }
    }
}
