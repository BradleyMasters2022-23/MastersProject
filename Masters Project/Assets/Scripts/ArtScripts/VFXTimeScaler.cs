/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October, 2022
 * Last Edited - June 5th, 2023 by Ben Schuster
 * Description - Adjust any VFX based on time and handle pool returning.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXTimeScaler : TimeAffectedEntity, IPoolable
{
    /// <summary>
    /// Effect manager
    /// </summary>
    private VisualEffect effectRef;

    [Tooltip("If time is frozen, how much to fast forwards by")]
    public float fastForwardTime = 0.5f;

    [Tooltip("How long does this VFX last")]
    public float lifetime = 1;

    /// <summary>
    /// Reference tracking lifetime
    /// </summary>
    private LocalTimer lifeTimer;
    /// <summary>
    /// Whether or not this VFX is playing
    /// </summary>
    private bool playing;

    public bool keepReserved = false;

    public bool alwaysFastForward = false;

    private void Awake()
    {
        // start playing for the VFX that uses them normally
        effectRef = GetComponentInChildren<VisualEffect>(true);
        effectRef.Play();
        playing = true;

        if(!keepReserved)
            lifeTimer = GetTimer(lifetime);
    }

    /// <summary>
    /// Update playback rate, determine if it should end
    /// </summary>
    void Update()
    {
        // Update playrate based on timescale
        effectRef.playRate = 1 * Timescale;

        // If set to reserved, then this is being managed by another entity
        if (keepReserved)
            return;

        // When life ends, stop spawning VFX
        if (playing && lifeTimer.TimerDone())
        {
            playing = false;
            effectRef.Stop();
        }
        // When not playing, return to pool once the the last living particle ends
        // If failed to return, just destroy self instead
        else if (!playing && effectRef.aliveParticleCount <= 0)
        {
            if(!VFXPooler.instance.ReturnVFX(gameObject))
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// Fast forward the simulation by the simulation time
    /// </summary>
    private void FastForward()
    {
        effectRef.Simulate(fastForwardTime);
    }

    /// <summary>
    /// On init, prepare variables and stop
    /// </summary>
    public void PoolInit()
    {
        effectRef.Stop();
        playing = false;
    }

    /// <summary>
    /// On pull, reset lifetime, determine if it should fast forward
    /// </summary>
    public void PoolPull()
    {
        effectRef.Play();
        float life = lifetime;
        if ((Affected && Slowed) || alwaysFastForward)
        {
            FastForward();
            life -= fastForwardTime;
        }
        playing = true;
        lifeTimer.ResetTimer(life);
    }

    /// <summary>
    /// On return, stop and return
    /// </summary>
    public void PoolPush()
    {
        effectRef.Reinit();
        effectRef.Stop();
    }
}
