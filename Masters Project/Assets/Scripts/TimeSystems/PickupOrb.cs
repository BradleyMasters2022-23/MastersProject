/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages the behavior of a pickup orb
 * ================================================================================================
 */
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

public abstract class PickupOrb : MonoBehaviour
{
    /// <summary>
    /// this objects rigidbody
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// Reference to player transform
    /// </summary>
    protected Transform player;

    [Tooltip("Whether this object is affected with timestop")]
    [SerializeField] protected bool affectedByTimestop;

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

    [Tooltip("Range before getting picked up")]
    [SerializeField] protected float pickupRadius;

    [Tooltip("Range before chasing the target")]
    [SerializeField] protected float chaseRadius;

    [Tooltip("Speed it goes to the player at")]
    [SerializeField] protected float chaseSpeed;

    [Tooltip("The collider for the actual core collider")]
    [SerializeField] protected Collider realCollider;

    [Tooltip("Sound when orb is collected")]
    [SerializeField] private AudioClipSO OrbCollect;

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

   

    private ScaledTimer startDespawnTracker;
    private ScaledTimer despawnTracker;
    private Animator anim;
    
    //private bool ready;
    //[SerializeField] private float targetVelocity;
    //[SerializeField] private Vector3 targetDir;
    //private Vector3 lastHit;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>().CenterMass;
        anim = GetComponent<Animator>();

        startDespawnTracker = new ScaledTimer(startDespawnTime, despawnTimeAffectedByTimestop);
        //despawnTracker = new ScaledTimer(despawnTime, despawnTimeAffectedByTimestop);

        // Randomly generate velocity and rotation angles
        float vel = Random.Range(dropVelocityRange.x, dropVelocityRange.y);
        float angY = Random.Range(0, 360);
        float angX = -Random.Range(dropXAngleRange.x, dropXAngleRange.y);

        floatHeight = Random.Range(floatHeightRange.x, floatHeightRange.y);

        transform.position += Vector3.up * 0.5f;
        transform.rotation = Quaternion.Euler(angX, angY, 0);
        rb.velocity = transform.forward * vel;

        //targetDir = rb.velocity.normalized;
        //targetVelocity = vel;

        GetComponent<SphereCollider>().radius = pickupRadius;
        //ready = false;

        //StartCoroutine(SpawningOrb());

        source = gameObject.AddComponent<AudioSource>();
    }

    //protected IEnumerator SpawningOrb()
    //{
    //    WaitForFixedUpdate tick = new WaitForFixedUpdate();
    //    RaycastHit hitInfo;

    //    while (true)
    //    {
    //        Debug.DrawRay(transform.position, Vector3.down, Color.red, 0.1f);
    //        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, Mathf.Infinity, groundMask))
    //        {
    //            // If within range of ground, set to ground mode
    //            if(hitInfo.distance <= floatHeight)
    //            {
    //                rb.isKinematic = true;
    //                rb.velocity = Vector3.zero;

    //                rb.transform.position = hitInfo.point + Vector3.up * floatHeight;

    //                rb.mass /= 2;

    //                break;
    //            }

    //            // if not, modify velocity for time stop
    //            else if (affectedByTimestop)
    //            {
    //                if (TimeManager.WorldTimeScale == 0 && !rb.isKinematic)
    //                    rb.isKinematic = true;
    //                else if (TimeManager.WorldTimeScale != 0 && rb.isKinematic)
    //                    rb.isKinematic = false;
    //            }

    //            lastHit = hitInfo.point;


    //        }
    //        // if no ground hit, then it fell through the world so just destroy it now
    //        else
    //        {
    //            rb.isKinematic = true;
    //            rb.velocity = Vector3.zero;

    //            rb.transform.position = lastHit + Vector3.up * floatHeight;

    //            rb.mass /= 2;

    //            //DespawnOrb();
    //            yield break;
    //        }

    //        yield return null;
    //        yield return tick;
    //    }

    //    ready = true;
    //}
    private void OnCollisionEnter(Collision collision)
    {
        realCollider.enabled = false;
        rb.isKinematic = true;
        Vector3 temp = rb.velocity;
        temp.x = 0;
        temp.y = 0;
        rb.velocity = temp;
        rb.mass /= 2;
    }

    protected void Update()
    {
        CheckDespawnStatus();

        RaycastHit hitInfo;

        // Switch to kinematic after the initial burst after spawning
        if(!rb.isKinematic)
        {
            // Check if it needs to start floating
            if (rb.velocity.y < 0 && Physics.Raycast(transform.position, Vector3.down, out hitInfo, floatHeight, groundMask))
            {
                realCollider.enabled = false;
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.mass /= 2;
            }
        }
        // If it can chase, chase the player
        else if(CheckChaseRequirements())
        {
            // Chase the player
            Vector3 dir = player.position - transform.position;
            anim.SetBool("isChasing", true);

            // Modify chase formula based on timestop preference
            if (affectedByTimestop)
            {
                rb.transform.position += dir.normalized * chaseSpeed * TimeManager.WorldDeltaTime;
            }
            else
            {
                rb.transform.position += dir.normalized * chaseSpeed * Time.deltaTime;
            }
        }
        // Otherwise, try falling again
        else if (rb.isKinematic && !Physics.Raycast(transform.position, Vector3.down, out hitInfo, floatHeight, groundMask))
        {
            rb.isKinematic = false;
            anim.SetBool("isChasing", false);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OnPickup();

            OrbCollect.PlayClip(transform);
        }
    }

    /// <summary>
    /// Check whether its time to despawn the orb
    /// </summary>
    protected void CheckDespawnStatus()
    {
        // Dont worry about despawning stuff if player picked it up
        if (CheckChaseRequirements())
            return;

        // check if delay effects should happen
        if(startDespawnTracker.TimerDone() && despawnTracker == null)
        {
            StartDespawning();
        }
        else if(startDespawnTracker.TimerDone())
        {

            if(despawnTimeAffectedByTimestop)
                anim.speed = despawnTracker.TimerProgress() * despawnAnimationScaling * TimeManager.WorldTimeScale;
            else
                anim.speed = despawnTracker.TimerProgress() * despawnAnimationScaling;

            if (despawnTracker.TimerDone())
                DespawnOrb();
        }
    }

    /// <summary>
    /// Start the despawning routine. Apply some visual effect to this
    /// </summary>
    protected void StartDespawning()
    {
        despawnTracker = new ScaledTimer(despawnTime, despawnTimeAffectedByTimestop);
        anim.SetBool("isDespawning", true);
    }

    /// <summary>
    /// Despawn the orb and do any necessary effects
    /// </summary>
    protected virtual void DespawnOrb()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// What happens when this item is picked up
    /// </summary>
    protected abstract void OnPickup();

    /// <summary>
    /// Check any chase requirements before chasing
    /// </summary>
    /// <returns>Whether the chase requirements have been met</returns>
    protected abstract bool CheckChaseRequirements();
}
