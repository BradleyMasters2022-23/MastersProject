/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages the behavior of a pickup orb
 * ================================================================================================
 */
using UnityEngine;

public abstract class PickupOrb : TimeAffectedEntity, IPoolable, TimeObserver
{
    private enum OrbState
    {
        Spawning, // spawning (upwards velocity)
        Chasing, // chasing the player
        Idle // on the ground
    }

    [SerializeField] private OrbState currState = OrbState.Spawning;

    /// <summary>
    /// this objects rigidbody
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// Reference to player transform
    /// </summary>
    protected Transform player;

    [Header("=====Spawning=====")]

    [Tooltip("Range of velocity items can drop with")]
    [SerializeField] protected Vector2 dropVelocityRange;
    [Tooltip("Range of velocity items can drop with")]
    [SerializeField] protected Vector2 dropXAngleRange;
    [Tooltip("How high does this float in the air")]
    [SerializeField] protected Vector2 floatHeightRange;
    [SerializeField] protected LayerMask groundMask;

    private float floatHeight;

    [Header("=====Pickup=====")]

    [Tooltip("Range before chasing the target")]
    [SerializeField] protected float chaseRadius;
    [Tooltip("Speed it goes to the player at")]
    [SerializeField] protected float chaseSpeed;

    [Tooltip("The collider for the actual core collider")]
    [SerializeField] protected Collider realCollider;
    [Tooltip("Sound when orb is collected")]
    [SerializeField] protected AudioClipSO OrbCollect;

    [Tooltip("Whether the orbs can be picked up during slowed time")]
    [SerializeField] protected bool pickupWhileSlowing;

    private AudioSource source;

    [Header("=====Despawn=====")]

    [Tooltip("How long until the despawn sequence starts")]
    [SerializeField] protected float startDespawnTime;
    [Tooltip("How long it takes to despawn, once the sequence starts")]
    [SerializeField] protected float despawnTime;
    [Tooltip("Whether the despawn cooldowns are affected by timestop")]
    [SerializeField] protected bool despawnTimeAffectedByTimestop;
    [Tooltip("Speed modifier for despawn indicator. Gets faster the closer to the despawn.")]
    [SerializeField] protected float despawnAnimationScaling;

    private LocalTimer startDespawnTracker;
    private LocalTimer despawnTracker;
    private LocalTimer spawnTracker;
    private float minimumSpawnTime = 0.5f;
    private Animator anim;
    private TrailRenderer trail;

    protected void Spawn()
    {
        // Randomly generate velocity and rotation angles
        float vel = Random.Range(dropVelocityRange.x, dropVelocityRange.y);
        float angY = Random.Range(0, 360);
        float angX = -Random.Range(dropXAngleRange.x, dropXAngleRange.y);

        floatHeight = Random.Range(floatHeightRange.x, floatHeightRange.y);

        transform.position += Vector3.up * 0.5f;
        transform.rotation = Quaternion.Euler(angX, angY, 0);
        rb.velocity = transform.forward * vel;
    }

    #region State

    protected virtual void LateUpdate()
    {
        // dont do anything while slowed
        if (Affected && Slowed) return;

        StateUpdate();
    }

    private void StateUpdate()
    {
        switch(currState)
        {
            case OrbState.Spawning:
                {
                    // If the minimum spawn time is done, go to idle
                    if (spawnTracker.TimerDone())
                        ChangeState(OrbState.Idle);

                    break;
                }
            case OrbState.Chasing:
                {
                    // if requirements not met, stop chasing
                    //if(!CheckChaseRequirements())
                    //{
                    //    ChangeState(OrbState.Idle);
                    //    return;
                    //}

                    // Chase the player
                    Vector3 dir = player.position - transform.position;
                    rb.transform.position += dir.normalized * chaseSpeed * DeltaTime;

                    break;
                }
            case OrbState.Idle:
                {
                    // if requirements met, start chasing
                    if (CheckChaseRequirements())
                    {
                        ChangeState(OrbState.Chasing);
                        return;
                    }

                    // if hit the float distance, freeze
                    if (Physics.Raycast(transform.position, Vector3.down, floatHeight, groundMask))
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                    else
                        rb.constraints = RigidbodyConstraints.None;

                    // check if it should be dewspawning
                    CheckDespawnStatus();

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void ChangeState(OrbState newState)
    {
        switch (newState)
        {
            case OrbState.Spawning:
                {
                    // When spawning, set trail to nothing
                    trail.Clear();

                    break;
                }
            case OrbState.Chasing:
                {
                    // switch to kinematic for chasing reasons
                    rb.isKinematic = true;

                    // Set animator to chasing
                    anim.SetBool("isChasing", true);

                    break;
                }
            case OrbState.Idle:
                {
                    // switch to normal
                    rb.isKinematic = false;

                    break;
                }
            default:
                {
                    break;
                }
        }
        currState = newState;
    }

    #endregion

    #region Main Funcs

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currState == OrbState.Chasing)
        {
            OnPickup();
            GeneralPickup();
        }
    }

    protected bool InRange()
    {
        return Vector3.Distance(transform.position, player.position) <= chaseRadius;
    }

    /// <summary>
    /// Any effects that happen when this orb is actually picked up
    /// </summary>
    protected void GeneralPickup()
    {
        OrbCollect.PlayClip(transform);

        // try returning to pool
        if (!ProjectilePooler.instance.ReturnProjectile(gameObject))
            Destroy(gameObject);
    }

    /// <summary>
    /// Start the despawning routine. Apply some visual effect to this
    /// </summary>
    protected void StartDespawning()
    {
        despawnTracker = GetTimer(despawnTime);
        anim.SetBool("isDespawning", true);
    }

    /// <summary>
    /// Check whether its time to despawn the orb
    /// </summary>
    protected void CheckDespawnStatus()
    {
        // check if delay effects should happen
        if (startDespawnTracker.TimerDone() && despawnTracker == null)
        {
            StartDespawning();
        }
        else if (startDespawnTracker.TimerDone())
        {
            anim.speed = despawnTracker.TimerProgress() * despawnAnimationScaling * Timescale;

            if (despawnTracker.TimerDone())
                DespawnOrb();
        }
    }

    /// <summary>
    /// Despawn the orb and do any necessary effects
    /// </summary>
    protected virtual void DespawnOrb()
    {
        //Destroy(gameObject);
        if(!ProjectilePooler.instance.ReturnProjectile(gameObject))
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Abstract

    /// <summary>
    /// What happens when this item is picked up
    /// </summary>
    protected abstract void OnPickup();

    /// <summary>
    /// Check any chase requirements before chasing
    /// </summary>
    /// <returns>Whether the chase requirements have been met</returns>
    protected abstract bool CheckChaseRequirements();

    #endregion

    #region Time Observer

    private Vector3 freezeVel;
    private bool freezeTrigger;
    public void OnStop()
    {
        // store velocity
        freezeVel = rb.velocity;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        // store collider data
        freezeTrigger = realCollider.isTrigger;
        realCollider.enabled = false;
    }

    public void OnResume()
    {
        // apply stored velocity
        rb.velocity = freezeVel;
        rb.constraints = RigidbodyConstraints.None;
        // apply stored collider data 
        realCollider.isTrigger = freezeTrigger;
        realCollider.enabled = true;
    }

    #endregion

    #region Pooling

    public void PoolInit()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        trail = GetComponentInChildren<TrailRenderer>(true);

        startDespawnTracker = GetTimer(startDespawnTime);
        spawnTracker = GetTimer(minimumSpawnTime);
    }

    public void PoolPull()
    {
        if(player == null)
            player = PlayerTarget.p.Center;

        trail.Clear();
        startDespawnTracker.ResetTimer();
        currState = OrbState.Spawning;
        rb.isKinematic = false;

        spawnTracker.ResetTimer();
        Spawn();
        //Debug.Break();
    }

    public virtual void PoolPush()
    {
        trail.Clear();
    }

    #endregion
}
