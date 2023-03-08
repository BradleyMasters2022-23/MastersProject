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
    [SerializeField] private AudioClipSO onHoverSound;

    [SerializeField] private AudioClipSO onPressSound;

    private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        onHoverSound.PlayClip(source);
    }

    public void PlayClickSound()
    {
        onPressSound.PlayClip();
    }
}
