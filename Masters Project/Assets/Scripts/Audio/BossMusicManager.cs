/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - May 1st, 2022
 * Last Edited - May 1st, 2022 by Ben Schuster
 * Description - Manager for multi-stage boss music
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource phase1Music;
    [SerializeField] private AudioSource phase2Music;
    [SerializeField] private AudioSource phase3Music;
    [SerializeField] float phaseInSpeed;
    [SerializeField, ReadOnly] float currentTracker;

    int phase = 0;

    private void Awake()
    {
        phase = 0;
    }

    private void Update()
    {
        currentTracker = phase1Music.time;
    }

    /// <summary>
    /// Increment music to next phase
    /// </summary>
    public void IncrementMusic()
    {
        phase++;

        switch(phase)
        {
            case 1:
                {
                    phase1Music.Play();
                    // start phase 1 music
                    StartCoroutine(Transition(phaseInSpeed, phase1Music));
                    break;
                }
            case 2:
                {
                    // start phase 2 music using phase 1's time
                    phase2Music.Play();
                    phase2Music.time = phase1Music.time;
                    StartCoroutine(Transition(phaseInSpeed, phase2Music));
                    break;
                }
            case 3:
                {
                    // start phase 3 music using phase 1's time
                    phase3Music.Play();
                    phase3Music.time = phase1Music.time;
                    StartCoroutine(Transition(phaseInSpeed, phase3Music));
                    break;
                }
            default:
                {
                    Debug.Log("Trying to increment to an invalud music state");
                    break;
                }
        }
    }

    /// <summary>
    /// Smoothly turns on music
    /// </summary>
    /// <param name="transitionTime">Time it takes to transition</param>
    /// <param name="oldSource">The audio source being transitioned out of </param>
    /// <param name="newSource">The audio source being transitioned into</param>
    /// <returns></returns>
    private IEnumerator Transition(float transitionTime, AudioSource source)
    {
        // get timer for smooth scaling using its timer progress func
        ScaledTimer timer = new ScaledTimer(transitionTime, false);

        // get original volume settings to scale
        float originalVol = source.volume;
        source.volume = 0;

        // lerp in the new sound
        while (!timer.TimerDone())
        {
            source.volume = timer.TimerProgress() * originalVol;
            yield return null;
        }

        source.volume = originalVol;

        yield return null;
    }

    /// <summary>
    /// Smoothly stop music
    /// </summary>
    /// <param name="transitionTime">Time to take to stop it</param>
    /// <param name="source">Source to stop</param>
    /// <returns></returns>
    private IEnumerator SlowStop(float transitionTime, AudioSource source)
    {
        // get timer for smooth scaling using its timer progress func
        ScaledTimer timer = new ScaledTimer(transitionTime, false);

        // get original volume settings to scale
        float originalVol = source.volume;

        // lerp in the new sound
        while (!timer.TimerDone())
        {
            source.volume = (1 - timer.TimerProgress()) * originalVol;
            yield return null;
        }

        source.volume = 0;
        source.Stop();

        yield return null;
    }

    /// <summary>
    /// Stop the boss music
    /// </summary>
    public void StopBossMusic()
    {
        StartCoroutine(SlowStop(phaseInSpeed, phase1Music));
        StartCoroutine(SlowStop(phaseInSpeed, phase2Music));
        StartCoroutine(SlowStop(phaseInSpeed, phase3Music));
    }

    public void StopMainMusic()
    {
        BackgroundMusicManager.instance.StopMusic();
    }
}
