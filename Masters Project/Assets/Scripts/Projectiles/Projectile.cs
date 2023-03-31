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
    protected Rigidbody rb;

    /// <summary>
    /// Target velocity for this object
    /// </summary>
    protected Vector3 targetVelocity;


    [Tooltip("What spawns when this his something")]
    [SerializeField] public GameObject onHitEffect;

    [Tooltip("What layers should this projectile hit with a larger radius")]
    [SerializeField] protected LayerMask targetLayers;

    [Tooltip("What layers should this impact directly with")]
    [SerializeField] protected LayerMask groundLayer;

    [Tooltip("Sound when this bullet is shot")]
    [SerializeField] private AudioClipSO bulletShot;
    [Tooltip("Sound when this bullet hits something")]
    [SerializeField] private AudioClipSO bulletHit;

    private AudioSource source;

    [Header("Distance Stuff")]
    [Tooltip("The gameobject that actually holds the visual model of the bullet")]
    [SerializeField] private Transform bulletVisual;
    [Tooltip("How does the scale of the bullet change over distance")]
    [SerializeField] private AnimationCurve scaleOverDistance;
    
    [Tooltip("The VFX that plays when this bullet despawns due to distance limit")]
    [SerializeField] private GameObject fadeVFX;


    [SerializeField] protected int maxHits = 1;

    [SerializeField] protected bool affectedByTimestop;

    private float originalVisualScale;

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

    protected override void Awake()
    {
        base.Awake();

        if (bulletVisual != null)
            originalVisualScale = bulletVisual.localScale.x;

        rb = GetComponent<Rigidbody>();
        source = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Fly();
        CheckLife();
    }

    protected virtual void Fly()
    {
        if (!active)
            return;

        //Debug.DrawLine(lastPos, transform.position, Color.red, 1f);
        //Debug.Log(Vector3.Distance(transform.position, lastPos));

        float dist = targetVelocity.magnitude * TimeManager.WorldDeltaTime;

        // adjust distance to account for end case
        if (distanceCovered + dist >= range)
        {
            dist = range - distanceCovered;
        }

        // Check for hitting a wall or target
        RaycastHit target;
        if (Physics.SphereCast(transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, dist, targetLayers))
        {
            hitRotatation = target.normal;

            Hit(target.point);
            RegisterAttackHit(target.collider.transform);
        }
        else if(Physics.Raycast(transform.position, transform.forward, out target, dist, groundLayer))
        {
            hitRotatation = target.normal;

            Hit(target.point);
            End();
        }
        else
        {
            hitPoint = transform.position;
            hitRotatation = -transform.forward;
        }


        // Update visuals for flying
        if (scaleOverDistance != null && bulletVisual != null)
        {
            float newScale = scaleOverDistance.Evaluate(distanceCovered / range) * originalVisualScale;

            bulletVisual.localScale = new Vector3(newScale, newScale, newScale);
        }

        // Update velocity with world timescale
        if (affectedByTimestop)
            rb.velocity = targetVelocity * TimeManager.WorldTimeScale;
        else
            rb.velocity = targetVelocity;
    }

    protected virtual void CheckLife()
    {
        distanceCovered += targetVelocity.magnitude * TimeManager.WorldDeltaTime;

        if (distanceCovered >= range)
        {
            if (fadeVFX != null)
            {
                Instantiate(fadeVFX, transform.position, transform.rotation);
            }

            End();
        }
    }

    protected override void Hit(Vector3 impactPoint)
    {
        // Spawn in whatever its told to, if able
        if(onHitEffect != null)
        {
            GameObject t = Instantiate(onHitEffect, impactPoint, Quaternion.identity);
            t.transform.LookAt(t.transform.position + hitRotatation);
        }

        bulletHit.PlayClip(transform);
    }

    /// <summary>
    /// Register an attack hit, perform any checks based on dealing damage
    /// </summary>
    /// <param name="target"></param>
    protected void RegisterAttackHit(Transform target)
    {
        bool dealtDamage = DealDamage(target);

        // if damage was dealt and max targets reached, end projectile
        if (dealtDamage && hitTargets.Count >= maxHits)
            End();
    }

    /// <summary>
    /// Activate this projectile and allow it to fire
    /// </summary>
    public override void Activate()
    {
        if (bulletVisual != null)
            bulletVisual.localScale = new Vector3(originalVisualScale, originalVisualScale, originalVisualScale);

        // clear hit list on activate
        if(hitTargets != null)
            hitTargets.Clear();
        else
            hitTargets = new List<Transform>();

        targetVelocity = transform.forward * speed;
        active = true;

        bulletShot.PlayClip(transform);
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
}
