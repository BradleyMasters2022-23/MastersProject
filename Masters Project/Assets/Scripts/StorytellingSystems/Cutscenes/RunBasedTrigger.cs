using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBasedTrigger : CutsceneTrigger
{
    [SerializeField] private int minRunLength;
    
    /// <summary>
    /// Try to play the cutscene ASAP
    /// </summary>
    private void Start()
    {
        TryCutscene();
    }

    /// <summary>
    /// Only play if the minimum run length has been achived
    /// </summary>
    /// <returns>Whether the cutscene trigger can be used</returns>
    protected override bool CanPlay()
    {
        Debug.Log($"curent runs : {GlobalStatsManager.data.runsAttempted}");
        return base.CanPlay() && GlobalStatsManager.data.runsAttempted >= minRunLength;
    }
}
