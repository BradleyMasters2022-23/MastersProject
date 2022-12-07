using Masters.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RepositionBehavior : BaseEnemyMovement
{
    [Header("=== Reposition Data ===")]

    [SerializeField] private Vector2 sideToSideDistance;
    [SerializeField] private Vector2 forwardAndBackDistance;

    private Vector3 dest;


    [SerializeField] private float maxRange;
    [SerializeField] private float minRange;

    [SerializeField] private bool lookAtTarget;

    [SerializeField] private float maxDist;

    private float stopDist;

    private void Start()
    {
        agent.updateRotation = false;
        InvokeRepeating("Test", 3f, 2.5f);

        
    }

    private void Test()
    {
        StartBehavior(target);
    }

    public override void StartBehavior(Transform t)
    {
        dest = DeterminLocation();
        target = t;
        stopDist = agent.stoppingDistance;
        agent.stoppingDistance = 0;
    }

    private Vector3 DeterminLocation()
    {
        NavMeshHit hit = default;
        Vector3 temp;

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
                    Debug.Log("Wall found on each side!!!");
                    temp = transform.position + (transform.forward * forMod);
                }
            }

            // TODO - later on, maybe try changing directions for the front/back?
            // but this works right now since it seems to let player corner enemies which imo is good

        } while (!NavMesh.SamplePosition(temp, out hit, 1f, agent.areaMask));

        return hit.position;
    }

    protected override void BehaviorFunction()
    {
        if (target == null)
        {
            agent.ResetPath();
            return;
        }

        if (agent.remainingDistance <= 1)
        {
            agent.ResetPath();
            agent.stoppingDistance = stopDist;
        }

        // go to the target position
        GoToTarget(dest);

        // Check if the agent is on an offlink
        CheckOfflinkConnection();


        // rotate to the current target
        if (lookAtTarget)
        {
            RotateToInUpdate(target);
        }
        // Look in end location
        else
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
    }

}
