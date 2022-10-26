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

    /// <summary>
    /// Distance covered to calculate when to despawn
    /// </summary>
    private float distanceCovered;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active)
            return;

        // Update velocity with world timescale
        rb.velocity = targetVelocity * TimeManager.WorldTimeScale;

        // Check if it passed target
        RaycastHit target;
        if(Physics.CapsuleCast(lastPos, transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, (Vector3.Distance(lastPos, transform.position))))
        {
            TriggerTarget(target.collider);
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
            Instantiate(onHitEffect);
            
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
    }

    protected override void Awake()
    {
        lastPos = transform.position;
    }
}
