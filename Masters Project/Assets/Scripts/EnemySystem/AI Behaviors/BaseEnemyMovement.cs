/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 6, 2022
 * Last Edited - December 6, 2022 by Ben Schuster
 * Description - Base enemy movement behavior script that contains common functions needed
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public abstract class BaseEnemyMovement : MonoBehaviour
{
    public enum MoveState
    {
        Standby,
        Moving,
        Offlink
    }

    // [SerializeField] private MovementStates[] characterMovementStates;

    [SerializeField] public MoveState state;

    protected Transform target;

    /// <summary>
    /// weee im in the base script unity
    /// </summary>
    protected Vector3 lastTargetPos;

    protected NavMeshAgent agent;

    [SerializeField] protected AnimationCurve jumpCurve;
    // [SerializeField] protected float jumpDuration;

    [SerializeField] protected AnimationCurve fallCurve;
    // [SerializeField] protected float fallDuration;

    //protected float defSpeed;
    //protected float defRot;

    [Tooltip("Whether or not the enemy should face the jump direction before jumping")]
    [SerializeField] private bool faceJump;
    [Tooltip("Whether or not the enemy actually rotates to the landing position before jumping")]
    [HideIf("@this.faceJump == false")]
    [SerializeField] private bool rotateIntoJump;

    protected float rotationSpeed;

    [SerializeField] protected LayerMask collisionLayers;

    protected EnemyManager manager;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = GetComponent<EnemyManager>();

        if(manager != null)
        {
            //defSpeed = manager.currentMoveStates.moveSpeed;
            //rotationSpeed = manager.currentMoveStates.rotationSpeed;
        }

        //if(characterMovementStates.Length > 0)
        //{
        //    Debug.Log($"Loading move profile of {characterMovementStates[0].name}");
        //    agent.speed = characterMovementStates[0].moveSpeed;
        //    rotationSpeed = characterMovementStates[0].rotationSpeed;
        //    agent.angularSpeed = rotationSpeed;
        //}
        

        // defSpeed = agent.speed;
        // defRot = agent.angularSpeed;
        agent.updateRotation = false;
    }

    protected virtual void Update()
    {
        // dont do anything while on standby
        if (state == MoveState.Standby)
        {
            {
                return;
            }
        }

        

        StateUpdate();
    }


    protected virtual void StateUpdate()
    {
        agent.speed = TimeManager.WorldTimeScale * manager.currentMoveStates.moveSpeed;
        agent.angularSpeed = TimeManager.WorldTimeScale * manager.currentMoveStates.rotationSpeed;
        rotationSpeed = manager.currentMoveStates.rotationSpeed;

        switch (state)
        {
            case MoveState.Moving:
                {
                    BehaviorFunction();
                    break;
                }
            case MoveState.Offlink:
                {
                    break;
                }
        }
    }

    //public abstract void StartBehavior(Transform t);

    /// <summary>
    /// Perform the unique movement function of this behavior
    /// </summary>
    protected abstract void BehaviorFunction();

    /// <summary>
    /// Go to the target position
    /// </summary>
    /// <param name="target"></param>
    protected virtual void GoToTarget(Vector3 target)
    {
        if (target == null)
        {
            Debug.Log("No target detected, resetting path");
            agent.ResetPath();
            return;
        }

        if (target != lastTargetPos)
        {
            lastTargetPos = target;
            agent.SetDestination(target);
        }
        
    }

    /// <summary>
    /// Go to the assigned target's current position
    /// </summary>
    /// <param name="target"></param>
    protected virtual void GoToTarget(Transform target)
    {
        GoToTarget(target.position);
    }

    /// <summary>
    /// Check if an offlink has been detected and should be used
    /// </summary>
    protected virtual void CheckOfflinkConnection()
    {
        if (agent.isOnOffMeshLink && state == MoveState.Moving)
        {
            JumpToLedge();
        }
    }

    /// <summary>
    /// Core manager that manages offmesh links
    /// </summary>
    protected void JumpToLedge()
    {
        state = MoveState.Offlink;

        // Get necessary data
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        float duration = agent.currentOffMeshLinkData.offMeshLink.gameObject.GetComponent<LedgeData>().traverseTime;

        // Check whether to use jump or fall curve
        if (data.endPos.y >= data.startPos.y)
        {
            StartCoroutine(CurvedJump(jumpCurve, duration));
        }
        else
        {
            StartCoroutine(CurvedJump(fallCurve, duration));
        }
    }

    /// <summary>
    /// Jump between two points given a curve and duration
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator CurvedJump(AnimationCurve curve, float duration)
    {
        // Get starting data
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        // If enabled, wait until this entity has turned to their land position before jumping
        if (faceJump && rotateIntoJump)
        {
            agent.updateRotation = false;
            Vector3 lookDirection = (endPos - transform.position).normalized;
            yield return StartCoroutine(RotateTo(lookDirection));
        }
        // Instantly look towards the jump instead
        else if (faceJump && !rotateIntoJump)
        {
            agent.updateRotation = false;
            Vector3 lookDirection = (endPos - transform.position).normalized;

            Quaternion rot = Quaternion.LookRotation(lookDirection);
            rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);
            transform.rotation = rot;
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
        //agent.updateRotation = true;
        agent.CompleteOffMeshLink();
        state = MoveState.Moving;
    }

    /// <summary>
    /// Lerp rotate this entity to aim in the direction desired
    /// </summary>
    /// <param name="direction">Direction to look towards. Automatically flattened</param>
    /// <returns></returns>
    protected IEnumerator RotateTo(Vector3 direction)
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

    protected void RotateToInUpdate(Transform target)
    {
        RotateToInUpdate((target.position - transform.position).normalized);
    }

    protected void RotateToInUpdate(Vector3 direction)
    {
        // temp, adjust this to be better later
        if (state != MoveState.Moving || direction == Vector3.zero)
            return;

        agent.updateRotation = false;

        // Get target rotation
        Quaternion rot = Quaternion.LookRotation(direction);
        rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

        // rotate towards the target, limited by rotation
        float deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rot.eulerAngles.y);
        float clampedAngle = Mathf.Clamp(deltaAngle, -rotationSpeed, rotationSpeed);

        // Check if its finished. used because of some of the rotation scaling wont match when done
        if (clampedAngle == 0)
        {
            return;
        }

        // Adjust angle for timestop, apply
        float nextYAng = clampedAngle * TimeManager.WorldTimeScale;
        transform.rotation = Quaternion.Euler(0, (transform.rotation.eulerAngles.y + nextYAng) % 360, 0);
    }

    protected bool CheckForCollision(Vector3 dir)
    {
        // Debug.DrawRay(transform.position, dir, Color.red, 1f);

        RaycastHit hit;

        if(Physics.Raycast(transform.position+Vector3.up, dir, out hit, dir.magnitude, collisionLayers))
        {
            // Debug.Log($"Hit detected at {hit.collider.name}!");
            return true;
        }
        else
        {
            return false;
        }
    }
}
