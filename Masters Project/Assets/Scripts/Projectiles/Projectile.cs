/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Concrete projectile that flies straight
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Projectile : RangeAttack
{
    private bool active;

    /// <summary>
    /// Rigidbody of this object
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Target velocity for this object
    /// </summary>
    private Vector3 targetVelocity;

    /// <summary>
    /// The position of this object last frame
    /// </summary>
    private Vector3 lastPos;

    [Tooltip("What spawns when this his something")]
    [SerializeField] public GameObject onHitEffect;

    [Tooltip("What layers should this projectile hit with a larger radius")]
    [SerializeField] private LayerMask targetLayers;

    [Tooltip("What layers should this impact directly with")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Sound when enemy shoots")]
    [SerializeField] private AudioClip[] enemyShoot;
    [Tooltip("Sound when enemy takes a hit")]
    [SerializeField] private AudioClip[] enemyHit;
    private AudioSource source;

    /// <summary>
    /// Distance covered to calculate when to despawn
    /// </summary>
    private float distanceCovered;

    /// <summary>
    /// point where the contact was made
    /// </summary>
    private Vector3 hitPoint;

    /// <summary>
    /// rotation where the contact was made
    /// </summary>
    private Vector3 hitRotatation;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active)
            return;

        // Update velocity with world timescale
        rb.velocity = targetVelocity * TimeManager.WorldTimeScale;

        Vector3 futurePos = transform.position + Vector3.forward * rb.velocity.magnitude * TimeManager.WorldDeltaTime;

        // Check if it passed target
        RaycastHit target;
        if (Physics.SphereCast(transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, (Vector3.Distance(transform.position, lastPos)), targetLayers))
        {
            hitPoint = target.point;

            hitRotatation = target.normal;

            TriggerTarget(target.collider);
        }
        else
        {
            // Check if it directly hit a wall
            if (Physics.Raycast(transform.position, transform.forward, out target, (Vector3.Distance(transform.position, lastPos)), groundLayer))
            {
                hitPoint = target.point;
                hitRotatation = target.normal;
                TriggerTarget(target.collider);
            }
            else
            {
                hitPoint = transform.position;
                hitRotatation = -transform.forward;
            }
        }
        


        // Check if projectile reached its max range
        distanceCovered += targetVelocity.magnitude * TimeManager.WorldDeltaTime;
        if(distanceCovered >= range)
        {
            Hit();
        }

        lastPos = transform.position;
    }

    protected override void Hit()
    {
        // Spawn in whatever its told to, if able
        if(onHitEffect != null)
        {
            GameObject t = Instantiate(onHitEffect, hitPoint, Quaternion.identity);
            t.transform.LookAt(hitRotatation);
        }

        if (enemyHit.Length > 0)
        {
            source.PlayOneShot(enemyHit[Random.Range(0, enemyHit.Length)], 0.3f);
        }

        // End this projectile attack
        End();
    }

    /// <summary>
    /// Activate this projectile and allow it to fire
    /// </summary>
    public override void Activate()
    {
        rb = GetComponent<Rigidbody>();
        targetVelocity = transform.forward * speed;
        active = true;

        if (enemyShoot.Length > 0)
        {
            source.PlayOneShot(enemyShoot[Random.Range(0, enemyShoot.Length)], 0.3f);
        }
    }

    protected override void Awake()
    {
        lastPos = transform.position;

        source = gameObject.AddComponent<AudioSource>();
    }
}
