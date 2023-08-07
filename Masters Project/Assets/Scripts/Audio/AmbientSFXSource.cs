/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 22nd, 2022
 * Last Edited - June 22nd, 2022 by Ben Schuster
 * Description - Controls local ambiance audio
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientSFXSource : MonoBehaviour
{
    /// <summary>
    /// Main source of the object
    /// </summary>
    private AudioSource source;

    [Tooltip("SFX data for the main ambient SFX")]
    [SerializeField] AudioClipSO ambientSFX;

    [Tooltip("The initial sound clip that plays before the ambiance"), Space(5)]
    [SerializeField] AudioClipSO ambientStartSFX;
    [Tooltip("The ending sound clip that plays when ambiance is told to end")]
    [SerializeField] AudioClipSO ambientEndSFX;

    [Tooltip("Should this player begin on start"), Space(5)]
    [SerializeField] bool playOnStart;

    [Tooltip("Should this player begin once enabled"), Space(5)]
    [SerializeField] bool playOnEnable;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if(playOnEnable)
        {
            Play();
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    /// <summary>
    /// Play the initial SFX (if any), then play ambiance
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForStartingClip()
    {
        if(ambientStartSFX != null)
        {
            ambientStartSFX.PlayClip(source);
            source.loop = false;
            yield return new WaitUntil(() => !source.isPlaying);
        }

        ambientSFX.PlayClip(source);
    }

    /// <summary>
    /// Start playing loaded ambiance 
    /// </summary>
    public void Play()
    {
        StartCoroutine(WaitForStartingClip());
    }
    /// <summary>
    /// Stop playing loaded ambiance
    /// </summary>
    public void Stop()
    {
        StopAllCoroutines();
        source.Stop();
        if (ambientEndSFX != null)
        {
            ambientEndSFX.PlayClip(source);
            source.loop = false;
        }
    }
    /// <summary>
    /// Stop playing loaded ambiance. Will skip ending SFX if there is any
    /// </summary>
    public void FullStop()
    {
        StopAllCoroutines();
        source.Stop();
    }

}
