using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Music
{
    None, 
    Hub, 
    NonCombat, 
    Combat
}

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance;

    [SerializeField] private AudioClipSO hubMusic;
    [SerializeField] private AudioClipSO nonCombatMusic;
    [SerializeField] private AudioClipSO combatMusic;

    [SerializeField, Required] private AudioSource mainMusicSource;
    [SerializeField, Required] private AudioSource secondaryMusicSource;

    private bool transitioning;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        if(instance == null)
        {
            instance= this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        SetMusic(Music.NonCombat, 0f);
        Invoke("TestMusic", 5f);
        Invoke("TestMusic2", 10f);
    }

    private void TestMusic()
    {
        Debug.Log("Testing music");
        SetMusic(Music.Combat);
    }
    private void TestMusic2()
    {
        Debug.Log("Testing music");
        SetMusic(Music.NonCombat);
    }

    /// <summary>
    /// Set the music type and transition to it
    /// </summary>
    /// <param name="type">Type of music to play</param>
    /// <param name="transitionTime">How fast the transition should be</param>
    public void SetMusic(Music type, float transitionTime = 1f)
    {
        // Select music
        AudioClipSO chosenMusic;
        switch (type)
        {
            case Music.Hub:
                {
                    chosenMusic = hubMusic;
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
            default:
                {
                    chosenMusic = null;
                    break;
                }
        }
        if (chosenMusic == null)
            return;

        // Determine which music player to transition out of and into
        AudioSource oldSource, newSource;
        if(mainMusicSource.isPlaying)
        {
            oldSource = mainMusicSource;
            newSource = secondaryMusicSource;
        }
        else
        {
            oldSource = secondaryMusicSource;
            newSource = mainMusicSource;
        }

        if(oldSource == null || newSource == null)
        {
            Debug.LogError("[BackgroundMusicManager] Error playing music! Either main or secondary sources not set!");
        }

        // Prepare new music into the buffer
        transform.PlayClip(chosenMusic, newSource);

        StartCoroutine(Transition(transitionTime, oldSource, newSource));
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
        ScaledTimer timer = new ScaledTimer(transitionTime, false);

        newSource.volume = 0;
        newSource.Play();

        while(!timer.TimerDone())
        {
            oldSource.volume = 1 - timer.TimerProgress();
            newSource.volume = timer.TimerProgress();

            yield return null;
        }

        newSource.volume = 1;
        oldSource.volume = 0;
        oldSource.Stop();

        yield return null;
    }
}
