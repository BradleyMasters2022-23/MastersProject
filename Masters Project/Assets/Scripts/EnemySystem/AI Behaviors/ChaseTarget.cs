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
        agent.ResetPath();
        reachedTarget = true;
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

    #region Old Stuff [MOVED TO BASEENEMYMOVEMENT]

    //private enum MoveState
    //{
    //    Moving,
    //    Offlink
    //}

    //[SerializeField] private MoveState state;

    //[SerializeField] private Transform target;

    //Vector3 lastTargetPos;

    //private NavMeshAgent agent;

    //[SerializeField] private AnimationCurve jumpCurve;
    //[SerializeField] private float jumpDuration;

    //[SerializeField] private AnimationCurve fallCurve;
    //[SerializeField] private float fallDuration;

    //private float defSpeed;
    //private float defRot;

    //[Tooltip("Whether or not the enemy should turn to their landing position before jumping")]
    //[SerializeField] private bool turnIntoJump;

    //[SerializeField] private float rotationSpeed;
    //private void Awake()
    //{
    //    agent = GetComponent<NavMeshAgent>();
    //    defSpeed = agent.speed;
    //    defRot = agent.angularSpeed;
    //}

    //private void Update()
    //{
    //    agent.speed = TimeManager.WorldTimeScale * defSpeed;
    //    agent.angularSpeed = TimeManager.WorldTimeScale * defRot;


    //    StateUpdate();
    //}


    //private void StateUpdate()
    //{
    //    switch(state)
    //    {
    //        case MoveState.Moving:
    //            {
    //                Chase();
    //                break;
    //            }
    //            case MoveState.Offlink:
    //            {
    //                break;
    //            }
    //    }
    //}



    /*

    private void JumpToLedge()
    {
        state = MoveState.Offlink;

        OffMeshLinkData data = agent.currentOffMeshLinkData;

        // Check whether to use jump or fall curve
        if (data.endPos.y >= data.startPos.y)
        {
            StartCoroutine(CurvedJump(jumpCurve, jumpDuration));
        }
        else
        {
            StartCoroutine(CurvedJump(fallCurve, fallDuration));
        }
        
    }

    /// <summary>
    /// Jump between two points given a curve and duration
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator CurvedJump(AnimationCurve curve, float duration)
    {
        // Get starting data
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        // If enabled, wait until this entity has turned to their land position before jumping
        if (turnIntoJump)
        {
            agent.updateRotation = false;
            Vector3 lookDirection = (endPos - transform.position).normalized;
            yield return StartCoroutine(RotateTo(lookDirection));
        }

        // This is set to one because of how animation curves work
        // TODO - detect keypoints in leap and tie into animators
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += TimeManager.WorldDeltaTime / duration;

            yield return null;
        }

        // Reset to normal
        agent.updateRotation = true;
        state = MoveState.Moving;
        agent.CompleteOffMeshLink();
        agent.ResetPath();
        agent.SetDestination(target.position);
    }

    /// <summary>
    /// Lerp rotate this entity to aim in the direction desired
    /// </summary>
    /// <param name="direction">Direction to look towards. Automatically flattened</param>
    /// <returns></returns>
    private IEnumerator RotateTo(Vector3 direction)
    {
        agent.updateRotation = false;

        // Get target rotation
        Quaternion rot = Quaternion.LookRotation(direction);
        rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

        while (transform.rotation.eulerAngles.y != rot.eulerAngles.y)
        {
            // rotate towards the target, limited by rotation
            float deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rot.eulerAngles.y);
            float clampedAngle = Mathf.Clamp(deltaAngle, -rotationSpeed, rotationSpeed);

            // Check if its finished. used because of some of the rotation scaling wont match when done
            if (clampedAngle == 0)
            {
                break;
            }

            // Adjust angle for timestop, apply
            float nextYAng = clampedAngle * TimeManager.WorldTimeScale;
            transform.rotation = Quaternion.Euler(0, (transform.rotation.eulerAngles.y + nextYAng) % 360, 0);

            // Keep this on fixed update to prevent framerate differences
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        transform.rotation = rot;
        yield return null;
    }

    */

    #endregion
}
