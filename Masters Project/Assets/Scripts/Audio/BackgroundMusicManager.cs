/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 6th, 2022
 * Last Edited - March 6th, 2022 by Ben Schuster
 * Description - Manager for background music
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Mono.Cecil;

public enum Music
{
    None, 
    Hub, 
    Menu, 
    NonCombat, 
    Combat
}

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance;

    [SerializeField] private AudioClipSO menuMusic;
    [SerializeField] private AudioClipSO nonCombatMusic;
    [SerializeField] private AudioClipSO combatMusic;
    [SerializeField] private AudioClipSO hubMusic;

    [SerializeField, Required] private AudioSource mainMusicSource;
    [SerializeField, Required] private AudioSource secondaryMusicSource;

    private AudioClipSO currentlyLoaded;
    private Coroutine transitionRoutine;

    /// <summary>
    /// Initialize instance
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance= this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void StopMusic()
    {
        StartCoroutine(SlowStop());
    }

    /// <summary>
    /// Set the music type and transition to it. Useful for common themes
    /// </summary>
    /// <param name="type">Type of music to play</param>
    /// <param name="transitionTime">How fast the transition should be</param>
    public void SetMusic(Music type, float transitionTime = 1f)
    {
        AudioClipSO chosenMusic;
        switch (type)
        {
            case Music.Menu:
                {
                    chosenMusic = menuMusic;
                    break;
                }
            case Music.NonCombat:
                {
                    chosenMusic = nonCombatMusic;
                    break;
                }
            case Music.Combat:
                {
                    chosenMusic = combatMusic;
                    break;
                }
            case Music.Hub:
                {
                    chosenMusic = hubMusic;
                    break;
                }
            default:
                {
                    chosenMusic = null;
                    break;
                }
        }

        SetMusic(chosenMusic, transitionTime);
    }

    /// <summary>
    /// Pass in new music to play
    /// </summary>
    /// <param name="music">Music data to utilize</param>
    /// <param name="transitionTime">time to transition between</param>
    public void SetMusic(AudioClipSO music, float transitionTime = 1f)
    {
        // Select music
        // dont transition if its already playing, otherwise record the new type
        if (currentlyLoaded == music && (mainMusicSource.isPlaying || secondaryMusicSource.isPlaying))
            return;
        else
            currentlyLoaded = music;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        if (currentlyLoaded == null)
        {
            StopMusic();
            return;
        }

        // Determine which music player to transition out of and into
        AudioSource oldSource, newSource;
        if (mainMusicSource.isPlaying)
        {
            oldSource = mainMusicSource;
            newSource = secondaryMusicSource;
        }
        else
        {
            oldSource = secondaryMusicSource;
            newSource = mainMusicSource;
        }

        // check for error
        if (oldSource == null || newSource == null)
        {
            Debug.LogError("[BackgroundMusicManager] Error playing music! Either main or secondary sources not set!");
        }

        // Prepare new music into the buffer
        currentlyLoaded.PlayClip(newSource);

        // transition from old source to new source
        transitionRoutine = StartCoroutine(Transition(transitionTime, oldSource, newSource));
    }

    /// <summary>
    /// Smoothly transition between two audio sources
    /// </summary>
    /// <param name="transitionTime">Time it takes to transition</param>
    /// <param name="oldSource">The audio source being transitioned out of </param>
    /// <param name="newSource">The audio source being transitioned into</param>
    /// <returns></returns>
    private IEnumerator Transition(float transitionTime, AudioSource oldSource, AudioSource newSource)
    {
        // get original volume settings to scale
        float oldOriginalVol = oldSource.volume;
        float newOriginalVol = newSource.volume;

        newSource.volume = 0;
        newSource.Play();

        float t = 0;
        while(t < transitionTime)
        {
            oldSource.volume = (1 - (t / transitionTime)) * oldOriginalVol;
            newSource.volume = (t / transitionTime) * newOriginalVol;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        newSource.volume = newOriginalVol;
        oldSource.volume = 0;
        oldSource.Stop();

        yield return null;
    }

    private IEnumerator SlowStop(float transitionTime = 1f)
    {
        // get timer for smooth scaling using its timer progress func
        ScaledTimer timer = new ScaledTimer(transitionTime, false);

        // get original volume settings to scale
        float mainVol = mainMusicSource.volume;
        float secondVol = secondaryMusicSource.volume;


        while (!timer.TimerDone())
        {
            mainMusicSource.volume = (1 - timer.TimerProgress()) * mainVol;
            secondaryMusicSource.volume = (1 - timer.TimerProgress()) * secondVol;

            yield return null;
        }

        mainMusicSource.volume = 0;
        secondaryMusicSource.volume = 0;
        mainMusicSource.Stop();
        secondaryMusicSource.Stop();

        yield return null;
    }

    /// <summary>
    /// Resume the music player from whatever it was previously
    /// </summary>
    public void ResumePlayer()
    {
        SetMusic(currentlyLoaded);
    }
}
