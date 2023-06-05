/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - April 5th, 2022 by Ben Schuster
 * Description - Concrete projectiles that flies straight
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Projectile : RangeAttack, TimeObserver, IPoolable
{
    #region Variables

    /// <summary>
    /// Rigidbody of this object
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// Target velocity for this object
    /// </summary>
    protected Vector3 targetVelocity;

    /// <summary>
    /// Distance covered over time
    /// </summary>
    private float distanceCovered;

    private TrailRenderer trail;

    [Header("Distance Scaling")]

    [Tooltip("The gameobject that actually holds the visual model of the bullet")]
    [SerializeField] private Transform bulletVisual;
    [HideIf("@this.bulletVisual == null")]
    [Tooltip("How does the scale of the bullet change over distance")]
    [SerializeField] private AnimationCurve scaleOverDistance;
    [HideIf("@this.bulletVisual == null")]
    [Tooltip("The VFX that plays when this bullet despawns due to distance limit")]
    [SerializeField] private GameObject fadeVFX;
    /// <summary>
    /// Original scale of this object
    /// </summary>
    private float originalVisualScale;

    private SphereCollider col;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        if (bulletVisual != null)
            originalVisualScale = bulletVisual.localScale.x;

        trail = GetComponentInChildren<TrailRenderer>();
        col = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Fly through the air
        Fly();

        // Check if it should despawn
        if(CheckLife())
        {
            if (fadeVFX != null)
            {
                Instantiate(fadeVFX, transform.position, transform.rotation);
            }

            //TimeManager.instance.UnSubscribe(this);
            End();
        }
    }

    protected override void UniqueActivate()
    {
        if (bulletVisual != null)
            bulletVisual.localScale = new Vector3(originalVisualScale, originalVisualScale, originalVisualScale);

        //TimeManager.instance.Subscribe(this);

        targetVelocity = transform.forward * speed;
    }

    protected override void Fly()
    {
        if (!active)
            return;

        if(rb.isKinematic)
            rb.isKinematic = false;

        float dist = targetVelocity.magnitude * DeltaTime;

        // adjust distance to account for edge case
        if (distanceCovered + dist >= range)
        {
            dist = range - distanceCovered;
        }

        // Check for hitting a wall or target
        RaycastHit target;
        if(Physics.SphereCast(transform.position, col.radius, transform.forward, out target, dist, slowFieldLayers))
        {
            Debug.DrawLine(transform.position, target.point, Color.red, 5f);
            transform.position = target.point;
            rb.isKinematic = true;
            
        }
        else if (Physics.SphereCast(transform.position, col.radius, transform.forward, out target, dist, hitLayers))
        {
            Hit(target.point, target.normal);
            ApplyDamage(target.collider.transform, target.point);
        }
        else if (Physics.Raycast(transform.position, transform.forward, out target, dist, worldLayers))
        {
            Hit(target.point, target.normal);
            End();
        }


        // Update visuals for flying
        ScaleOverDistance();

        rb.velocity = targetVelocity * Timescale;
    }

    protected override bool CheckLife()
    {
        distanceCovered += targetVelocity.magnitude * DeltaTime;

        return distanceCovered >= range;
    }

    public override void Inturrupt()
    {
        //TimeManager.instance.UnSubscribe(this);
        End();
    }

    /// <summary>
    /// Scale projectile over the distance if possible
    /// </summary>
    private void ScaleOverDistance()
    {
        if (scaleOverDistance != null && bulletVisual != null && Timescale > TimeManager.TimeStopThreshold)
        {
            float newScale = scaleOverDistance.Evaluate(distanceCovered / range) * originalVisualScale;

            bulletVisual.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    public void OnStop()
    {
        if(scaleOverDistance != null && bulletVisual != null)
        {
            bulletVisual.localScale = Vector3.one * originalVisualScale;
        }
    }

    public void OnResume()
    {
        return;
    }

    /// <summary>
    /// Initialize the bullet when its first spawned
    /// </summary>
    public virtual void PoolInit()
    {
        // initialization handled by awake
        return;
    }

    /// <summary>
    /// What happens when this projectile is pulled from the pool
    /// </summary>
    public virtual void PoolPull()
    {
        // Automatically taken care of by the activate func
        if(trail != null)
            trail.Clear();
    }

    public virtual void PoolPush()
    {
        active = false;

        distanceCovered = 0;
        hitTargets.Clear();

        if(bulletVisual!= null)
            bulletVisual.localScale = Vector3.one * originalVisualScale;
    }
}
