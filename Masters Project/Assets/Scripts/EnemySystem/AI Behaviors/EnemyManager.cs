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

    #endregion

    #region Chase Behavior Variables

    [Header("=== Chasing ===")]

    [Tooltip("Chase behavior and data, if applicable")]
    [SerializeField] protected ChaseTarget chaseBehavior;
    [Tooltip("How far to the target before trying to give chase")]
    [SerializeField] protected float startChastDist;
    [Tooltip("How close to the target should chasing stop")]
    [SerializeField] protected float stopChaseDist;

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
    [SerializeField] protected bool strafingAttack;

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

        player = FindObjectOfType<PlayerController>();
        
        audioSource = GetComponent<AudioSource>();
        

        // disable behaviors
        if(chaseBehavior != null)
        {
            chaseBehavior.enabled = false;
        }
        if(strafeBehavior != null)
        {
            strafeBehavior.enabled = false;
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

            // if player is too far, start chasing
            if(chaseBehavior != null && distToPlayer >= startChastDist)
            {
                chaseBehavior.enabled = true;
                chaseBehavior.SetChase(player.transform, stopChaseDist);

                while(!chaseBehavior.ReachedTargetDistance())
                {
                    yield return null;
                    yield return tick;
                }

            }
            else
            {
                
            }


            yield return null;
            yield return tick;
        }
    }

    private IEnumerator ActivateDelay()
    {
        activateTracker.ResetTimer();
        while (!activateTracker.TimerDone())
            yield return null;
    }

}
