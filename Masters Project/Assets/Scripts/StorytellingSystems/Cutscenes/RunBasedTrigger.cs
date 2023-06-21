using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBasedTrigger : CutsceneTrigger
{
    [SerializeField] private int minRunLength;

    protected override void Awake()
    {
        base.Awake();

        HUDFadeManager.instance.SetImmediate(true);
    }

    /// <summary>
    /// Try to play the cutscene ASAP if possible
    /// </summary>
    private void Start()
    {
        if (!CanPlay())
        {
            //Debug.Log("Cant play, disabling");
            onVideoFinishEvents.Invoke();
            onCutsceneFadeFinishEvents.Invoke();
        }
        else
        {
            TryCutscene();
        }
            
    }

    /// <summary>
    /// Only play if the minimum run length has been achived
    /// </summary>
    /// <returns>Whether the cutscene trigger can be used</returns>
    protected override bool CanPlay()
    {
        // Debug.Log($"curent runs : {GlobalStatsManager.data.runsAttempted}");
        return base.CanPlay() && GlobalStatsManager.data.runsAttempted >= minRunLength;
    }
}
