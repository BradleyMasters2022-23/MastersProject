/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Base class for all ranged enemies
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;



public class EnemyRange : EnemyBase
{
    private enum MoveState
    {
        Moving,
        Offlink
    }

    /// <summary>
    /// Current state of the enemy
    /// </summary>
    [SerializeReference] private EnemyState state;

    [SerializeField] private MoveState moveState;

    [SerializeField] private bool hasLineOfSight;

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
    [SerializeField] float moveSpeed;
    [Tooltip("Rotation speed of the enemy")]
    [SerializeField][Range(0f, 5f)] private float rotationSpeed;
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

    private Vector3 lastTargetPos;

    [SerializeField] private AnimationCurve jumpCurve;
    //[SerializeField] private float jumpDuration;

    [SerializeField] private AnimationCurve fallCurve;
    //[SerializeField] private float fallDuration;

    [Tooltip("Whether or not the enemy should face the jump direction before jumping")]
    [SerializeField] private bool faceJump;
    [Tooltip("Whether or not the enemy actually rotates to the landing position before jumping")]
    [HideIf("@this.faceJump == false")]
    [SerializeField] private bool rotateIntoJump;

    //private ScaledTimer jumpCooldown;

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

    [Tooltip("What layers affects this enemy's vision")]
    [SerializeField] private LayerMask visionLayer;

    private float shotRadius;

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
        currTime = TimeManager.WorldTimeScale;
        agent = GetComponent<NavMeshAgent>();

        shotRadius = shotPrefab.GetComponent<SphereCollider>().radius;

        //jumpCooldown = new ScaledTimer(0.8f);

        agent.speed = moveSpeed;
        agent.updateRotation = false;
        state = EnemyState.Moving;
    }

    private void FixedUpdate()
    {
        CheckState();
        // Get current time state
        currTime = TimeManager.WorldTimeScale;

        currDist = Vector3.Distance(playerCenter.position, transform.position);
        agent.speed = moveSpeed * currTime;


        switch (state)
        {
            case EnemyState.Moving:
                {
                    // Move towards player position
                    MoveStateUpdate();

                    break;
                }
            case EnemyState.Attacking:
                {
                    // Attack if vision and not already attacking
                    if (attackRoutine == null && LineOfSight(playerCenter) && InVision(playerCenter))
                    {
                        attackRoutine = StartCoroutine(Attack());
                    }

                    // Move towards player when out of range and out of threshold, or in the moving state at all
                    //if ((Mathf.Abs(currDist - idealRange) > moveThreshold && currDist > idealRange))
                    //{
                    //    agent.speed = moveSpeed * currTime;
                    //    agent.SetDestination(playerCenter.position);
                    //}
                    //// flee from player when too close
                    //else if (fleeWhileAttacking && Mathf.Abs(currDist - idealRange) > moveThreshold && currDist < idealRange)
                    //{
                    //    Vector3 awayDir = ((transform.position + Vector3.up) - playerCenter.position).normalized * moveSpeed * fleeMoveModifier;
                    //    agent.speed = moveSpeed * currTime;
                    //    agent.SetDestination(transform.position + awayDir);
                    //}

                    break;
                }
        }

        if(moveState != MoveState.Offlink || (moveState == MoveState.Offlink && !faceJump) )
        {
            Vector3 direction;

            // Get direction of player, or next jump
            if (agent.nextOffMeshLinkData.valid && (faceJump || !InVision(playerCenter)))
            {
                direction = (agent.nextOffMeshLinkData.startPos - transform.position);
            }
            else
            {
                direction = (playerCenter.position - transform.position);
            }

            // rotate towards them, clamped
            Quaternion rot = Quaternion.LookRotation(direction);
            float nextYAng = Mathf.Clamp(Mathf.DeltaAngle(gameObject.transform.rotation.eulerAngles.y, rot.eulerAngles.y), -rotationSpeed, rotationSpeed) * currTime;
            transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y + nextYAng, 0);
        }
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
                    if (LineOfSight(playerCenter) && currDist <= idealRange
                        && moveState != MoveState.Offlink)
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
                    if (!LineOfSight(playerCenter) && attackRoutine == null)
                    {
                        Debug.Log("Switching to moving");
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
    private bool LineOfSight(Transform target)
    {
        Vector3 direction = target.position - shootPoints[0].position;

        // Try to get player
        RaycastHit hit;

        Debug.DrawRay(shootPoints[0].position, direction, Color.red, 0.5f);

        // Use a cast to make sure it accounts for the shot's radius
        if (Physics.SphereCast(shootPoints[0].position, shotRadius, direction, out hit, attackRange, visionLayer))
        {
            hasLineOfSight = true;
            if (hit.transform.CompareTag("Player"))
                return true;
        }

        hasLineOfSight = false;
        return false;
    }


    private bool InVision(Transform target)
    {
        Vector3 targetPos = new Vector3(target.position.x, centerMass.position.y, target.position.z);
        Vector3 temp = (targetPos - centerMass.position).normalized;
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
        float _originalMovement = moveSpeed;
        float _originalRotation = rotationSpeed;

        // Aim Substate
        moveSpeed = (_originalMovement * aimMoveModifier);
        rotationSpeed = _originalRotation * aimRotModifier;
        while (attackTimer < aimDuration)
        {
            attackTimer += TimeManager.WorldDeltaTime;
            yield return null;
        }

        // Get player position to target
        Vector3 targetPos = playerCenter.position;

        // Shoot Substate
        moveSpeed = (_originalMovement * shootMoveModifier);
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
        moveSpeed = (_originalMovement * reloadMoveModifier);
        rotationSpeed = _originalRotation * reloadRotModifier;
        while (attackTimer < reloadDuration)
        {
            attackTimer += TimeManager.WorldDeltaTime;
            yield return null;
        }

        // Return original values
        rotationSpeed = _originalRotation;
        moveSpeed = _originalMovement;

        // Clear attack routine 
        attackRoutine = null;
        yield return null;
    }

    private void OnDisable()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
    }


    #region Movement

    private void MoveStateUpdate()
    {
        switch (moveState)
        {
            case MoveState.Moving:
                {
                    Chase();

                    break;
                }
            case MoveState.Offlink:
                {
                    break;
                }
        }
    }

    private void Chase()
    {
        if (player == null)
        {
            agent.ResetPath();
            return;
        }

        if (player.transform.position != lastTargetPos)
        {
            lastTargetPos = player.transform.position;
            agent.SetDestination(player.transform.position);
        }

        if (agent.isOnOffMeshLink && moveState != MoveState.Offlink)
        {
            JumpToLedge();
        }
    }

    private void JumpToLedge()
    {
        moveState = MoveState.Offlink;

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
    private IEnumerator CurvedJump(AnimationCurve curve, float duration)
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
        else if(faceJump && !rotateIntoJump)
        {
            agent.updateRotation = false;
            Vector3 lookDirection = (endPos - transform.position).normalized;

            Quaternion rot = Quaternion.LookRotation(lookDirection);
            rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);
            transform.rotation = rot;
        }

        // Check to break before starting jump
        // Check if target is now on its own level and if this should stop

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
        agent.CompleteOffMeshLink();
        //agent.ResetPath();
        //agent.SetDestination(player.transform.position);
        //jumpCooldown.ResetTimer();
        moveState = MoveState.Moving;
    }

    /// <summary>
    /// Lerp rotate this entity to aim in the direction desired
    /// </summary>
    /// <param name="direction">Direction to look towards. Automatically flattened</param>
    /// <returns></returns>
    private IEnumerator RotateTo(Vector3 direction)
    {
        Debug.Log("Rotate towards started");

        agent.updateRotation = false;

        // Get target rotation
        Quaternion rot = Quaternion.LookRotation(direction);
        rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

        while (transform.rotation.eulerAngles.y != rot.eulerAngles.y)
        {
            // Check if target is now on its own level and if this should stop
            //if (PlayerMovedPlatform())
            //    yield break;

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
        Debug.Log("Rotate towards ended");

        transform.rotation = rot;
        yield return null;
    }

    private bool PlayerMovedPlatform()
    {
        agent.SetDestination(player.transform.position);
        if(!agent.nextOffMeshLinkData.valid)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
