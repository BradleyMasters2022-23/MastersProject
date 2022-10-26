/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Base class for all ranged enemies
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRange : EnemyBase
{
    /// <summary>
    /// Current state of the enemy
    /// </summary>
    private EnemyState state;
    /// <summary>
    /// The navmesh component for the enemy
    /// </summary>
    private NavMeshAgent agent;
    /// <summary>
    /// Current time, referenced by time manager
    /// </summary>
    private float currTime = 1;

    [Header("======Move Information======")]

    [Tooltip("Movement speed of the enemy")]
    [SerializeField] float walkSpeed;
    [Tooltip("Rotation speed of the enemy")]
    [SerializeField][Range(0f, 1f)] private float rotationSpeed;
    [Tooltip("How much does this enemy overshoot its movement, such as rounding corners")]
    [SerializeField] private float overshootMult;

    [Tooltip("Range the enemy tries to stay in")]
    [SerializeField] private float idealRange;
    [Tooltip("Range the enemy can attack from")]
    [SerializeField] private float attackRange;
    [Tooltip("Distance tolerance before attempting to reposition")]
    [SerializeField] private float moveThreshold;

    [Tooltip("The movement modifier(%) when fleeing. Multiplies with attack move modifiers.")]
    [SerializeField][Range(0, 2)] private float fleeMoveModifier = 1;

    /// <summary>
    /// Current distance between this enemy and player
    /// </summary>
    private float currDist;

    [Header("======Attack Information======")]
    [Header("---Core Info---")]
    [Tooltip("Projectile that this enemy shoots")]
    [SerializeField] private GameObject shotPrefab;
    [Tooltip("Where the bullet(s) spawn from")]
    [SerializeField] private Transform[] shootPoints;
    [Tooltip("How many shots the attack fire")]
    [SerializeField][Range(0, 50)] private int numberOfShots;
    [Tooltip("What is the radius of the cone this enemy can see in")]
    [SerializeField] private int lookRadius;

    [Header("---Attack Stage Duration---")]
    [Tooltip("How long this enemy waits while aiming")]
    [SerializeField][Range(0, 10)] private float aimDuration;
    [Tooltip("How long does each individual shot take")]
    [SerializeField][Range(0, 10)] private float shootDuration;
    [Tooltip("How long this enemy waits while reloading")]
    [SerializeField][Range(0, 10)] private float reloadDuration;

    [Header("---Attack Stage Movement---")]
    [Tooltip("The movement modifier(%) when aiming")]
    [SerializeField][Range(0, 2)] private float aimMoveModifier = 1;
    [Tooltip("The movement modifier(%) when shooting")]
    [SerializeField][Range(0, 2)] private float shootMoveModifier = 1;
    [Tooltip("The movement modifier(%) when reload")]
    [SerializeField][Range(0, 2)] private float reloadMoveModifier = 1;

    [Header("---Attack Stage Rotation---")]
    [Tooltip("The movement modifier(%) when aiming")]
    [SerializeField][Range(0, 2)] private float aimRotModifier = 1;
    [Tooltip("The movement modifier(%) when shooting")]
    [SerializeField][Range(0, 2)] private float shootRotModifier = 1;
    [Tooltip("The movement modifier(%) when reload")]
    [SerializeField][Range(0, 2)] private float reloadRotModifier = 1;

    [Header("---Attack Stage Special---")]
    [Tooltip("Whether or not this enemy fires at the same point when shooting")]
    [SerializeField] private bool lockAiming;
    [Tooltip("Whether or not this enemy tries to flee while still attacking")]
    [SerializeField] private bool fleeWhileAttacking;
    [Tooltip("Strength of shot-leading from this enemy")]
    [SerializeField] private Vector2 leadStrength;

    /// <summary>
    /// The current attack routine being triggered.
    /// </summary>
    private Coroutine attackRoutine;

    // Start is called before the first frame update
    void Start()
    {
        currTime = TimeManager.WorldTimeScale; ;
        agent = GetComponent<NavMeshAgent>();

        agent.speed = walkSpeed;
        agent.updateRotation = false;
        state = EnemyState.Moving;
    }

    private void FixedUpdate()
    {
        CheckState();
        // Get current time state
        currTime = TimeManager.WorldTimeScale;

        currDist = Vector3.Distance(playerCenter.position, transform.position);
        agent.speed = walkSpeed * currTime;

        // Move towards player when out of range and out of threshold
        if (Mathf.Abs(currDist - idealRange) > moveThreshold && currDist > idealRange)
        {
            agent.speed = walkSpeed * currTime;
            agent.SetDestination(playerCenter.position);
        }
        // flee from player when too close
        else if (fleeWhileAttacking && Mathf.Abs(currDist - idealRange) > moveThreshold && currDist < idealRange)
        {
            Vector3 awayDir = ((transform.position + Vector3.up) - playerCenter.position).normalized * walkSpeed * fleeMoveModifier;
            agent.speed = walkSpeed * currTime;
            agent.SetDestination(transform.position + awayDir);
        }


        switch (state)
        {
            case EnemyState.Moving:
                {
                    // Move towards player position

                    //agent.speed = walkSpeed * currTime;
                    //agent.SetDestination(p.transform.position);

                    break;
                }
            case EnemyState.Attacking:
                {

                    // Attack if vision and not already attacking
                    if (attackRoutine == null && InVision())
                    {
                        attackRoutine = StartCoroutine(Attack());
                    }

                    break;
                }
        }

        // Get direction of player, rotate towards them
        Vector3 direction = playerCenter.position - (transform.position + Vector3.up);
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float nextAng = Mathf.LerpAngle(transform.rotation.eulerAngles.y, angle, rotationSpeed * currTime);
        transform.rotation = Quaternion.Euler(0, nextAng, 0);
    }

    /// <summary>
    /// Check state, see if it should swap
    /// </summary>
    /// <returns>New state</returns>
    private void CheckState()
    {
        switch (state)
        {
            case EnemyState.Moving:
                {
                    // Switch to attacking if line of sight and within ideal range
                    if (LineOfSight(playerCenter.gameObject) && currDist <= idealRange)
                    {
                        state = EnemyState.Attacking;

                        // Overshoot a tiny bit, namely corners
                        Vector3 temp = agent.velocity.normalized;
                        temp *= overshootMult;

                        agent.SetDestination((transform.position + Vector3.up) + temp);
                    }

                    break;
                }
            case EnemyState.Attacking:
                {
                    // Resume moving if outside of attack range and not already attacking
                    if (!LineOfSight(playerCenter.gameObject) && attackRoutine == null)
                    {
                        state = EnemyState.Moving;
                    }

                    break;
                }
        }
    }

    /// <summary>
    /// Check if this object has line of sight on target via raycast
    /// </summary>
    /// <param name="target">target to check</param>
    /// <returns>Line of sight</returns>
    private bool LineOfSight(GameObject target)
    {
        Vector3 direction = target.transform.position - (transform.position + Vector3.up);

        // Set mask to ignore raycasts and enemy layer
        int lm = LayerMask.NameToLayer("Enemy");
        lm = (1 << lm);
        lm |= (1 << LayerMask.NameToLayer("Ignore Raycast"));

        // Try to get player
        RaycastHit hit;
        if (Physics.Raycast((transform.position + Vector3.up), direction, out hit, attackRange, ~lm))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
        }
        return false;
    }


    private bool InVision()
    {
        Vector3 temp = (playerCenter.position - (transform.position + Vector3.up)).normalized;
        float angle = Vector3.SignedAngle(temp, transform.forward, Vector3.up);
        return (Mathf.Abs(angle) <= lookRadius);
    }

    private void Shoot(Vector3 targetPos)
    {
        // Spawn projectile for each barrel
        List<GameObject> spawnedProjectiles = new List<GameObject>();
        foreach(Transform barrel in shootPoints)
        {
            GameObject o = Instantiate(shotPrefab, barrel.position, shotPrefab.transform.rotation);
            spawnedProjectiles.Add(o);
        }
        RangeAttack temp = spawnedProjectiles[0].GetComponent<RangeAttack>();

        // Calculate lead aim
        float travelTime = (targetPos - transform.position).magnitude / temp.Speed;
        float strength = Random.Range(leadStrength.x, leadStrength.y);
        Vector3 leadPos = playerCenter.position + (player.GetComponent<Rigidbody>().velocity * travelTime * strength);

        // Aim shot at target position, activate
        foreach (GameObject shot in spawnedProjectiles)
        {
            shot.transform.LookAt(leadPos);
            shot.GetComponent<RangeAttack>().Activate();
        }
    }


    private IEnumerator Attack()
    {
        // Temp timer variables used for each substate
        float attackTimer = 0;
        float _originalMovement = walkSpeed;
        float _originalRotation = rotationSpeed;

        // Aim Substate
        walkSpeed = (_originalMovement * aimMoveModifier);
        rotationSpeed = _originalRotation * aimRotModifier;
        while (attackTimer < aimDuration)
        {
            attackTimer += TimeManager.WorldDeltaTime;
            yield return null;
        }

        // Get player position to target
        Vector3 targetPos = playerCenter.position;

        // Shoot Substate
        walkSpeed = (_originalMovement * shootMoveModifier);
        rotationSpeed = _originalRotation * shootRotModifier;
        for (int i = 0; i < numberOfShots; i++)
        {
            Shoot(targetPos);
            attackTimer = 0;
            while (attackTimer < shootDuration)
            {
                attackTimer += TimeManager.WorldDeltaTime;
                yield return null;
            }

            // If not locked, update the target position
            if (!lockAiming)
                targetPos = playerCenter.position;
        }

        // Recover/Reload Substate
        attackTimer = 0;
        walkSpeed = (_originalMovement * reloadMoveModifier);
        rotationSpeed = _originalRotation * reloadRotModifier;
        while (attackTimer < reloadDuration)
        {
            attackTimer += TimeManager.WorldDeltaTime;
            yield return null;
        }

        // Return original values
        rotationSpeed = _originalRotation;
        walkSpeed = _originalMovement;

        // Clear attack routine 
        attackRoutine = null;
        yield return null;
    }

    private void OnDisable()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
    }
}
