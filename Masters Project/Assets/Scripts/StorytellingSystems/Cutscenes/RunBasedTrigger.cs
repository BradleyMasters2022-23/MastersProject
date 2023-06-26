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
            // disable while loading
            GameManager.controls.Disable();
        }
            
    }

    /// <summary>
    /// Only play if the minimum run length has been achived
    /// </summary>
    /// <returns>Whether the cutscene trigger can be used</returns>
    protected override bool CanPlay()
    {
        return base.CanPlay() && (!onlyPlayOnce || GlobalStatsManager.data.runsAttempted >= minRunLength);
    }
}
