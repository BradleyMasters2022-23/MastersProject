using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent((typeof(VisualEffect)))]
public class VFXTimeScaler : TimeAffectedEntity
{
    private VisualEffect effectRef;
    private float lastTimeScale = 1;
    public float fastForwardTime = 0.5f;

    private void Awake()
    {
        effectRef= GetComponent<VisualEffect>();

        if (Timescale <= TimeManager.TimeStopThreshold)
            FastForward();

    }

    // Update is called once per frame
    void Update()
    {
        effectRef.playRate = 1 * Timescale;
    }

    private void FastForward()
    {
        effectRef.Play();
        effectRef.Simulate(fastForwardTime);
    }
}
