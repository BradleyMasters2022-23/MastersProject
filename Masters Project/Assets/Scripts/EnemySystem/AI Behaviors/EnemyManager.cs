/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 7, 2022
 * Last Edited - December 7, 2022 by Ben Schuster
 * Description - Manages all the AI behaviors. Can be inherited to create more specialized enemies
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Masters.AI;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.UIElements;

[System.Serializable]
public struct MovementStates
{
    public string name;
    public float moveSpeed;
    public float rotationSpeed;
}
public class EnemyManager : MonoBehaviour
{
    #region Core Variables

    [Header("=== Core Attributes ===")]
    [Tooltip("What are the movement states for this enemy?")]
    [SerializeField] private MovementStates[] characterMovementStates;
    
    [Tooltip("What is the current move state")]
    public MovementStates currentMoveStates;

    [Tooltip("When spawned, how long does the AI wait before activating")]
    [SerializeField] private float activateDelay = 0;

    [Tooltip("When spawned, what sound does this enemy make")]
    [SerializeField] private AudioClip spawnSound;

    private BaseEnemyMovement currentMoveBehavior;

    private ScaledTimer activateTracker;
    
    private NavMeshAgent agent;
    private PlayerController player;
    private AudioSource audioSource;


    [SerializeField] private LookAtTarget lookBehavior;

    #endregion

    #region Chase Behavior Variables

    [Header("=== Chasing ===")]

    [Tooltip("Chase behavior and data, if applicable")]
    [SerializeField] protected ChaseTarget chaseBehavior;
    [Tooltip("How far to the target before trying to give chase")]
    [SerializeField] protected float startChastDist;
    [Tooltip("How close to the target should chasing stop")]
    [SerializeField] protected float stopChaseDist;
    [Tooltip("Whether the chase behavior should stop on line of sight")]
    [SerializeField] protected bool stopChaseOnLineOfSight;
    [Tooltip("How long without getting line of sight should this enemy try chasing")]
    [SerializeField] protected float noLineOfSightDelay;

    [Tooltip("Cooldown for entering other behaviors after chasing")]
    [SerializeField] protected float postChaseCD;

    private ScaledTimer lineOfSightTracker;
    private ScaledTimer stateChangeCDTracker;

    #endregion

    #region Strafe and Reposition Variables

    [Header("=== Strafing and Repositioning ===")]

    [Tooltip("Strafe behavior and data, if applicable")]
    [SerializeField] protected RepositionBehavior strafeBehavior;

    [HideIf("@this.strafeBehavior == null")]
    [Tooltip("The movement profile should be used during this behavior")]
    [SerializeField] private int moveData;

    [HideIf("@this.strafeBehavior == null")]
    // [InfoBox("If strafing attack allowed, make sure to check 'look at target'")]
    [Tooltip("If it can attack mid strafe, what attack should it use?")]
    [SerializeField] protected AttackTarget strafingAttack;

    [Tooltip("Cooldown for entering other behaviors after chasing")]
    [SerializeField] protected float postStrafeCD;

    #endregion

    #region Attack behavior Variables

    [Header("=== Attack Behaviors ===")]

    [SerializeField] private AttackTarget mainAttack;

    [SerializeField] private LayerMask visionLayer;

    [SerializeField] protected float postAttackCD;

    #endregion

    /// <summary>
    /// Initialize variables
    /// </summary>
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }
        else
        {
            // Check if agent is needed. If so, destroy enemy to prevent any bugs.
            if (chaseBehavior != null || strafeBehavior != null)
            {
                Debug.LogError($"Enemy {name} has been given movement behaviors but no navmeshagent! " +
                    $"Destroying enemy to prevent any issues!");
                Destroy(gameObject);
                return;
            }
        }

        // Check if it has proper player movement
        if (agent != null && characterMovementStates.Length > 0)
        {
            currentMoveStates = characterMovementStates[0];
            agent.speed = currentMoveStates.moveSpeed;
            agent.angularSpeed = currentMoveStates.rotationSpeed;
        }
        else if(agent != null && characterMovementStates.Length <= 0)
        {
            Debug.LogError($"Enemy {name} has been given an agent but not movement state!" +
                $"Please add one as it is necessary for movement and behaviors!");
            Destroy(gameObject); 
            return;
        }


        activateTracker = new ScaledTimer(activateDelay);

        lineOfSightTracker = new ScaledTimer(noLineOfSightDelay);

        stateChangeCDTracker = new ScaledTimer(postChaseCD);

        player = FindObjectOfType<PlayerController>();
        
        audioSource = GetComponent<AudioSource>();
        

        // disable move behaviors
        if(chaseBehavior != null)
        {
            chaseBehavior.state = BaseEnemyMovement.MoveState.Standby;
        }
        if(strafeBehavior != null)
        {
            strafeBehavior.state = BaseEnemyMovement.MoveState.Standby;
        }
        if(lookBehavior != null)
        {
            lookBehavior.SetTarget(player.transform);
            lookBehavior.state = BaseEnemyMovement.MoveState.Standby;
        }
    }

    /// <summary>
    /// When spawned, start activate delay
    /// </summary>
    private void Start()
    {
        StartCoroutine(MainAIController());
    }

    /// <summary>
    /// Manage the core logic of the AI. 
    /// Routine to gain some bonus functionality
    /// </summary>
    /// <returns></returns>
    private IEnumerator MainAIController()
    {
        WaitForFixedUpdate tick = new WaitForFixedUpdate();

        yield return StartCoroutine(ActivateDelay());

        // start the thing here
        if (agent != null)
        {
            agent.enabled = true;
        }


        while (true) 
        {
            // calculate distance if needed
            float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

            bool lineOfSight = transform.HasLineOfSight(player.transform, visionLayer);

            // determine line of sight
            if (noLineOfSightDelay > 0 && lineOfSight)
            {
                // Debug.Log(transform.HasLineOfSight(player.transform, visionLayer));
                lineOfSightTracker.ResetTimer();
            }

            bool chaseTrigger = chaseBehavior != null &&
                (noLineOfSightDelay > 0 && lineOfSightTracker.TimerDone())
                || (!stopChaseOnLineOfSight && distToPlayer >= startChastDist);


            if (stateChangeCDTracker.TimerDone())
            {
                // If nothing to do, just stare
                if (lookBehavior.state == BaseEnemyMovement.MoveState.Standby)
                {
                    Debug.Log("Calling rotate behavior!");
                    lookBehavior.state = BaseEnemyMovement.MoveState.Moving;
                    lookBehavior.SetTarget(player.transform);
                }
                continue;
            }
                

            // if player is too far, start chasing
            if (chaseTrigger)
            {
                yield return StartCoroutine(HandleChase());


            }
            else if ((chaseBehavior != null && chaseBehavior.state == BaseEnemyMovement.MoveState.Standby) 
                && (strafeBehavior.CanStrafe() || mainAttack.currentAttackState == AttackState.Ready))
            {

                if(strafeBehavior.CanStrafe() && mainAttack.currentAttackState != AttackState.Ready)
                {
                    yield return StartCoroutine(HandleStrafe());
                }
                else if(!strafeBehavior.CanStrafe() && mainAttack.currentAttackState == AttackState.Ready)
                {
                    yield return StartCoroutine(HandleAttack());
                }
                else
                {
                    // if both are usable, randomly choose one

                    if (Random.Range(0, 2) == 0)
                        yield return StartCoroutine(HandleAttack());
                    else
                        yield return StartCoroutine(HandleStrafe());
                }
            }
            else
            {
                // If nothing else, look at target
                if (lookBehavior.state == BaseEnemyMovement.MoveState.Standby)
                {
                    Debug.Log("Calling rotate behavior!");
                    lookBehavior.state = BaseEnemyMovement.MoveState.Moving;
                    lookBehavior.SetTarget(player.transform);
                }
                    
            }


            yield return null;
            yield return tick;
        }
    }


    private IEnumerator HandleChase()
    {
        WaitForFixedUpdate tick = new WaitForFixedUpdate();

        Debug.Log("Calling chase behavior!");
        lookBehavior.state = BaseEnemyMovement.MoveState.Standby;
        chaseBehavior.state = BaseEnemyMovement.MoveState.Moving;

        chaseBehavior.SetChase(player.transform, stopChaseDist);

        while (true)
        {
            bool lineOfSight = transform.HasLineOfSight(player.transform, visionLayer);
            bool stopTrigger = (chaseBehavior.ReachedTargetDistance()) || (stopChaseOnLineOfSight && lineOfSight);

            // If setting is enabled and has line of sight, stop chase
            if (stopTrigger)
            {
                chaseBehavior.StopChase();
                break;
            }

            yield return null;
            yield return tick;

        }
        Debug.Log("Chase behavior done!");
        stateChangeCDTracker.ResetTimer(postChaseCD);
        chaseBehavior.state = BaseEnemyMovement.MoveState.Standby;
    }

    private IEnumerator HandleAttack()
    {
        WaitForFixedUpdate tick = new WaitForFixedUpdate();

        Debug.Log("Calling attack behavior!");

        lookBehavior.state = BaseEnemyMovement.MoveState.Standby;

        // mainAttack.enabled = true;
        mainAttack.Attack(player.transform);

        while (mainAttack.currentAttackState != AttackState.Cooldown)
        {
            yield return null;
            yield return tick;
        }

        stateChangeCDTracker.ResetTimer(postAttackCD);
        Debug.Log("Attack finished");
    }

    private IEnumerator HandleStrafe()
    {
        WaitForFixedUpdate tick = new WaitForFixedUpdate();

        Debug.Log("Start strafe behavior");

        strafeBehavior.BeginStrafe();

        // stop once strafe is finished and if strafing attack is finished. 
        while(!strafeBehavior.StrafeFinished() && (strafingAttack is null || 
            strafingAttack.currentAttackState == AttackState.Cooldown))
        {
            if(strafingAttack != null 
                && strafingAttack.currentAttackState == AttackState.Ready)
            {
                strafingAttack.Attack(player.transform);
            }

            yield return null;
            yield return tick;
        }

        stateChangeCDTracker.ResetTimer(postStrafeCD);
        Debug.Log("Finished strafe behavior");
    }

    private IEnumerator ActivateDelay()
    {
        activateTracker.ResetTimer();
        while (!activateTracker.TimerDone())
            yield return null;
    }

}
