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

    [SerializeField] private AudioClip onPressSound;

    public void PlayHoverSound()
    {
        // Debug.Log("Play hover sound called");

        if(onHoverSound != null)
            AudioSource.PlayClipAtPoint(onHoverSound, Camera.main.transform.position);
    }

    public void PlayClickSound()
    {
        // Debug.Log("Play click sound called");

        if (onPressSound != null)
            AudioSource.PlayClipAtPoint(onHoverSound, Camera.main.transform.position);
    }
}
