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

    [Tooltip("Sound when this bullet is shot")]
    [SerializeField] private AudioClip[] bulletShot;
    [Tooltip("Sound when this bullet hits something")]
    [SerializeField] private AudioClip[] bulletHit;
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

        //Debug.DrawLine(lastPos, transform.position, Color.red, 1f);
        //Debug.Log(Vector3.Distance(transform.position, lastPos));

        Vector3 futurePos = transform.position + Vector3.forward * rb.velocity.magnitude * TimeManager.WorldDeltaTime;

        float dist = targetVelocity.magnitude * TimeManager.WorldDeltaTime;

        // Check if it passed target
        RaycastHit target;
        if (Physics.SphereCast(transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, dist, targetLayers))
        {
            hitPoint = target.point;

            hitRotatation = target.normal;

            TriggerTarget(target.collider);
        }
        else
        {
            // Check if it directly hit a wall
            if (Physics.Raycast(transform.position, transform.forward, out target, dist, groundLayer))
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

        // Update velocity with world timescale
        rb.velocity = targetVelocity * TimeManager.WorldTimeScale;

        lastPos = transform.position;
    }

    protected override void Hit()
    {
        // Spawn in whatever its told to, if able
        if(onHitEffect != null)
        {
            GameObject t = Instantiate(onHitEffect, hitPoint, Quaternion.identity);
            t.transform.LookAt(t.transform.position + hitRotatation);
        }

        if (bulletHit.Length > 0)
        {
            AudioSource.PlayClipAtPoint(bulletHit[Random.Range(0, bulletHit.Length)], transform.position, 0.3f);
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

        if (bulletShot.Length > 0)
        {
            source.PlayOneShot(bulletShot[Random.Range(0, bulletShot.Length)], 0.3f);
        }
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetDistanceCovered()
    {
        return distanceCovered;
    }

    public void ChangeDamageTo(int newDamage)
    {
        damage = newDamage;
    }

    public bool GetShotByPlayer()
    {
        return shotByPlayer;
    }

    protected override void Awake()
    {
        lastPos = transform.position;

        source = gameObject.AddComponent<AudioSource>();
    }
}
