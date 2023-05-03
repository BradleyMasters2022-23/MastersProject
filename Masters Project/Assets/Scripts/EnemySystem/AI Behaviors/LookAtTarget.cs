using Masters.AI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private bool acquiredTargetOnce;

    [SerializeField] private Transform lineOfSightOrigin;

    protected override void Awake()
    {
        base.Awake();
        stunnedTracker = new ScaledTimer(stunnedDuration, affected);
        stunnedCooldown= new ScaledTimer(stunCooldown, affected);

        if (lineOfSightOrigin == null)
            lineOfSightOrigin = transform;

        stunned = false;
        acquiredTargetOnce = false;
    }

    public void SetTarget(Transform t)
    {
        // give quick invulnerability to stun when starting
        target = t;
        //Debug.Log("Looking at " + t.name);
        if(stunnedCooldown != null)
            stunnedCooldown.ResetTimer();
    }

    protected override void BehaviorFunction()
    {
        if (target == null)
            return;


        if (!stunned)
        {
            // If looking at target...
            if (LookingAtTarget() || !stunnedCooldown.TimerDone())
            {
                // Look at player or last position if no player found
                if (lookAtLastPosition)
                {
                    if (lineOfSightOrigin.HasLineOfSight(target, visionMask))
                    {
                        //Debug.Log("Rotate to target not last pos");
                        RotateToInUpdate(target);
                        lastPlayerPos = transform.position;
                    }
                    else if(lastPlayerPos != null)
                    {
                        RotateToInUpdate(lastPlayerPos - transform.position);
                    }
                    else
                    {
                        RotateToInUpdate(target);
                        lastPlayerPos = transform.position;
                    }
                }
                // look at player
                else
                {
                    //Debug.Log("Rotate to target");
                    RotateToInUpdate(target);
                }
            }
            // If not looking at target, enter stun
            else if(stunnedCooldown.TimerDone() && acquiredTargetOnce)
            {
                EnterStun();
            }


            // acquired target when looking at them once in a tight cone
            if (!acquiredTargetOnce && transform.InVisionCone(target, 5))
                acquiredTargetOnce = true;
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

    protected override void OnDisable()
    {
        target = null;
    }

    /// <summary>
    /// Suprise! The player isnt there anymore!
    /// be stunned for a moment. Put things like visual effects or SFX here. 
    /// </summary>
    public void EnterStun()
    {
        //Debug.Log("Entering stun!");
        stunnedTracker.ResetTimer();
        stunned = true;
        acquiredTargetOnce = false;
    }

    public void EndStun()
    {
        //Debug.Log("Ending stun!");
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
