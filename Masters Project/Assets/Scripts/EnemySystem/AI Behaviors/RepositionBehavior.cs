/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 6, 2022
 * Last Edited - December 6, 2022 by Ben Schuster
 * Description - Concrete behavior for choosing and moving into a random direction to strafe
 * ================================================================================================
 */
using Masters.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RepositionBehavior : BaseEnemyMovement
{
    [Header("=== Reposition Data ===")]

    [SerializeField] private Vector2 sideToSideDistance;
    [SerializeField] private Vector2 forwardAndBackDistance;

    [SerializeField] private int moveProfile;

    private Vector3 dest;

    [SerializeField] private float maxRange;
    [SerializeField] private float minRange;

    [SerializeField] private bool relinquishRotation;
    [SerializeField] private Vector2 repositionCooldown;
    private ScaledTimer strafeCooldownTracker;

    private bool complete;

    /*
    public override void StartBehavior(Transform t)
    {
        dest = DeterminLocation();
        target = t;
        stopDist = agent.stoppingDistance;
        agent.stoppingDistance = 0;
    }
    */

    protected override void Awake()
    {
        base.Awake();
        strafeCooldownTracker = new ScaledTimer(Random.Range(repositionCooldown.x, repositionCooldown.y), true);
        strafeCooldownTracker.ResetTimer();
        
    }

    public void BeginStrafe(Transform t)
    {
        complete = false;
        target = t;
        agent.stoppingDistance = 0;
        agent.velocity = Vector3.zero;

        strafeCooldownTracker.ResetTimer();
        manager.SetMoveProfile(moveProfile);
        dest = DeterminLocation();
    }

    public bool CanStrafe()
    {
        return strafeCooldownTracker.TimerDone();
    }

    private Vector3 DeterminLocation()
    {
        NavMeshHit hit;
        Vector3 temp;

        int c = 0;

        // Try to get a new position thats still on navmesh
        do
        {
            // get dist to target
            float distToTarget = Vector3.Distance(transform.position, target.position);

            // Get a random left and right modifier, apply random sign
            float sideMod = Random.Range(sideToSideDistance.x, sideToSideDistance.y);
            if(Random.Range(0, 2) == 0)
                sideMod *= -1;

            // Get a random forward and backward modifier, apply random sign (unless outside ranges)
            float forMod = Random.Range(forwardAndBackDistance.x, forwardAndBackDistance.y);
            if(distToTarget <= minRange)
            {
                forMod *= -1;
            }
            else if(distToTarget < maxRange && Random.Range(0, 2) == 0)
            {
                forMod *= -1;
            }


            // apply random modifier
            temp = transform.position + (transform.right * sideMod) + (transform.forward * forMod);

            // check wall collision
            if(CheckForCollision(temp - transform.position))
            {
                temp = transform.position + (transform.right * sideMod * -1) + (transform.forward * forMod);

                // check again if its going to be colliding something. If so, dont apply left or right
                if(CheckForCollision(temp - transform.position))
                {
                    //Debug.Log("Wall found on each side!!!");
                    temp = transform.position + (transform.forward * forMod);
                }
            }

            // TODO - later on, maybe try changing directions for the front/back?
            // but this works right now since it seems to let player corner enemies which imo is good
            c++;
            if(c >= 10000)
            {
                //Debug.Log("Searching for reposition determine location infinite looping");
                return transform.position;
            }


        } while (!NavMesh.SamplePosition(temp, out hit, 1f, agent.areaMask));

        Debug.DrawLine(hit.position, hit.position + Vector3.up, Color.red, 10f);

        return hit.position;
    }

    protected override void BehaviorFunction()
    {
        if (target == null)
        {
            agent.ResetPath();
            return;
        }

        // go to the target position
        GoToTarget(dest);

        // Check if the agent is on an offlink
        CheckOfflinkConnection();

        // rotate to the current target
        if(!relinquishRotation)
        {
            // if at the end of the path, look at target
            if(agent.remainingDistance <= .1f)
            {
                transform.RotateToInUpdate(target, rotationSpeed);
            }
            // Otherwise, look at target location
            else
            {
                Vector3 dir = dest - transform.position;
                transform.RotateToInUpdate(dir, rotationSpeed);
            }
        }

        if (agent.remainingDistance <= 0.05f)
        {
            FinishStrafe();
        }
    }

    public bool IsStrafeDone()
    {
        return complete;
    }

    protected override void OnDisable()
    {
        complete = true;


        manager.SetMoveProfile(0);
        state = MoveState.Standby;
    }

    private void FinishStrafe()
    {
        float dur = Random.Range(repositionCooldown.x, repositionCooldown.y);
        strafeCooldownTracker.ResetTimer(dur);
        complete = true;


        manager.SetMoveProfile(0);
        state = MoveState.Standby;
        agent.ResetPath();
    }
}
