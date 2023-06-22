using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTriggerTooltipHook : MonoBehaviour
{
    [SerializeField] TooltipEventHolder[] tooltipsPerWave;
    [SerializeField] SpawnTriggerField spawnTrigger;
    /// <summary>
    /// current wave to track
    /// </summary>
    private int waveIdx = 0;

    /// <summary>
    /// Start the event.
    /// </summary>
    private void Start()
    {
        StartCoroutine(WaitForMapLoad());
    }
    /// <summary>
    /// Delay event so map finishes loading
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForMapLoad()
    {
        yield return new WaitUntil(()=> MapLoader.instance.LoadState != LoadState.Loading);
        yield return new WaitForSecondsRealtime(0.6f);
        IncrementWaveTooltip();
        spawnTrigger.SubmitEvent(IncrementWaveTooltip);
    }

    /// <summary>
    /// Submit the next tooltip available
    /// </summary>
    public void IncrementWaveTooltip()
    {
        // Only do this as long as tooltips exist 
        if(waveIdx < tooltipsPerWave.Length)
        {
            tooltipsPerWave[waveIdx].CallTooltip();
        }
        // Otherwise if out of waves, remove this listener 
        else if(waveIdx == tooltipsPerWave.Length) 
        {
            spawnTrigger.RemoveEvent(IncrementWaveTooltip);
        }

        // increment
        waveIdx++;
    }
}
