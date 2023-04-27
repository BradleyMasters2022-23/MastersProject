/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 2, 2022
 * Last Edited - December 2, 2022 by Ben Schuster
 * Description - Concrete behavior that will try to chase a given target
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Masters.AI;

public class ChaseTarget : BaseEnemyMovement
{
    [SerializeField] private bool alwaysFaceTarget;
    [SerializeField] private LayerMask mask;

    private bool reachedTarget;
    private float targetDistance;

    //public override void StartBehavior(Transform t)
    //{
    //    target = t;
    //}

    public void SetChase(Transform t, float dist)
    {
        target = t;
        targetDistance = dist;
        reachedTarget = false;
    }


    /// <summary>
    /// Behavior for chasing the player.
    /// This is called every frame
    /// </summary>
    protected override void BehaviorFunction()
    {
        if (!agent.enabled)
        {
            StopChase();
            return;
        }

        if (target == null || reachedTarget)
        {
            agent.ResetPath();
            return;
        }

        // go to the target position
        GoToTarget(target);

        // Check if the agent is on an offlink
        CheckOfflinkConnection();

        // check if completed
        ReachedTargetDistance();

        // rotate to the current target
        if (alwaysFaceTarget)
        {
            RotateToInUpdate(target);
        }
        // Look in direction if no line of sight on player
        else
        {
            // check if line of sight, otherwise look forwards 
            if(transform.HasLineOfSight(target, mask))
            {
                transform.RotateToInUpdate(target, rotationSpeed);
            }
            else
            {
                transform.RotateToInUpdate(agent.velocity.normalized, rotationSpeed);
            }
        }
    }

    public void StopChase()
    {
        if(agent.enabled && agent.isOnNavMesh)
            agent.ResetPath();
        reachedTarget = true;
    }

    protected override void OnDisable()
    {
        StopChase();
    }

    public bool ReachedTargetDistance()
    {
        // dont let behavior end on offlink
        if (state == MoveState.Offlink)
            return false;

        float dist = Vector3.Distance(transform.position, target.position);

        if((dist <= targetDistance && transform.HasLineOfSight(target, manager.visionLayer)) || reachedTarget)
        {
            reachedTarget = true;
            return true;
        }
        else
        {
            return false;
        }
    }

}
