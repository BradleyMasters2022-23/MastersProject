using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayAudio : MonoBehaviour
{
    [SerializeField] AudioClipSO data;
    [SerializeField] AudioSource source;

    /// <summary>
    /// play a clip. Intended to be called via events
    /// </summary>
    public void PlayClip()
    {
        data.PlayClip(source);
    }
}
