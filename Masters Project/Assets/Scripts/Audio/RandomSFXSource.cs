/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 28th, 2023
 * Last Edited - July 28th, 2023 by Ben Schuster
 * Description - SFX player that waits a randomized delay between each play
 * ================================================================================================
 */
using System.Collections;
using UnityEngine;

public class RandomSFXSource : TimeAffectedEntity
{
    [Header("Random SFX Timing")]

    [Tooltip("Audio clip to play")]
    [SerializeField] AudioClipSO audioClip;
    [Tooltip("Time range to randomly use between each play")]
    [SerializeField] Vector2 intervalRangeMinMax;
    [Tooltip("Audio source to use. Will play overlapped")]
    [SerializeField] AudioSource source;
    [Tooltip("Whether to immediately start once the object is enabled")]
    [SerializeField] private bool startOnEnable;
    /// <summary>
    /// Timer used to track intervals
    /// </summary>
    private LocalTimer timer;
    /// <summary>
    /// Current routine being used
    /// </summary>
    private Coroutine routine;

    private void OnEnable()
    {
        if (startOnEnable)
            Play();
    }

    private void OnDisable()
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    /// <summary>
    /// Play the clip
    /// </summary>
    public void Play()
    {
        if(routine == null)
            routine = StartCoroutine(SFXTime());
    }

    /// <summary>
    /// Play the clip with a random time interval between each play
    /// </summary>
    /// <returns></returns>
    private IEnumerator SFXTime()
    {
        while (true)
        {
            // get delay time, update timer
            float time = Random.Range(intervalRangeMinMax.x, intervalRangeMinMax.y);
            if (timer == null)
            {
                timer = GetTimer(time);
            }
            else
            {
                timer.ResetTimer(time);
            }
            yield return new WaitUntil(timer.TimerDone);

            // play clip, wait for it to finish. Do length instead if isplaying since its overlapping
            // and may be sharing a source
            audioClip.PlayClip(source, true);
            yield return new WaitForSeconds(audioClip.GetClip().length);
            yield return null;
        }
    }
}
