using Masters.AI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LookAtTarget : BaseEnemyMovement
{
    [Tooltip("Only look at the last position they saw the player at")]
    [SerializeField] private bool lookAtLastPosition;
    [HideIf("@this.lookAtLastPosition == false")]
    [SerializeField] private LayerMask visionMask;

    private Vector3 lastPlayerPos;

    [Tooltip("In a frontal cone, how infront of the enemy does player need to be to be followed")]
    [SerializeField] private float trackConeRadius;
    [Tooltip("When target is lost, how long is this enemy stuned for. Stun ends early if player reenters range.")]
    [SerializeField] private float stunnedDuration;

    private ScaledTimer stunnedTracker;
    [SerializeField] private bool stunned;

    private ScaledTimer stunnedCooldown;
    [Tooltip("How long is the cooldown between stuns")]
    [SerializeField] private float stunCooldown;

    protected override void Awake()
    {
        base.Awake();
        stunnedTracker = new ScaledTimer(stunnedDuration);
        stunnedCooldown= new ScaledTimer(stunCooldown);
        stunned = false;
    }

    public void SetTarget(Transform t)
    {
        // give quick invulnerability to stun when starting
        target = t;
        stunnedCooldown.ResetTimer();
    }

    protected override void BehaviorFunction()
    {
        if (target == null)
            return;

        if(!stunned)
        {
            // If looking at target...
            if(LookingAtTarget() || !stunnedCooldown.TimerDone())
            {
                // Look at player or last position if no player found
                if (lookAtLastPosition)
                {
                    if (transform.HasLineOfSight(target, visionMask))
                    {
                        RotateToInUpdate(target);
                        lastPlayerPos = transform.position;
                    }
                    else
                    {
                        RotateToInUpdate(lastPlayerPos - transform.position);
                    }
                }
                // look at player
                else
                {
                    RotateToInUpdate(target);
                }
            }
            // If not looking at target, enter stun
            else if(stunnedCooldown.TimerDone())
            {
                EnterStun();
            }
        }
        else
        {
            // if timer done or player reentered vision, end stun
            if(stunnedTracker.TimerDone() || LookingAtTarget())
            {
                EndStun();
            }
        }
        
    }

    /// <summary>
    /// Suprise! The player isnt there anymore!
    /// be stunned for a moment. Put things like visual effects or SFX here. 
    /// </summary>
    public void EnterStun()
    {
        stunnedTracker.ResetTimer();
        stunned = true;
    }

    public void EndStun()
    {
        stunnedCooldown.ResetTimer();
        stunned = false;
    }

    public bool LookingAtTarget()
    {
        if(transform.InVisionCone(target, trackConeRadius))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
