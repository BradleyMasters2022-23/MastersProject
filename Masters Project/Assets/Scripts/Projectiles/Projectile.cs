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

public class Projectile : RangeAttack
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

    #endregion

    protected override void Awake()
    {
        base.Awake();

        if (bulletVisual != null)
            originalVisualScale = bulletVisual.localScale.x;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        // Fly through the air
        Fly();

        // Check if it should despawn
        if(CheckLife())
        {
            if (fadeVFX != null)
            {
                Instantiate(fadeVFX, transform.position, transform.rotation);
            }

            End();
        }
    }

    protected override void UniqueActivate()
    {
        if (bulletVisual != null)
            bulletVisual.localScale = new Vector3(originalVisualScale, originalVisualScale, originalVisualScale);

        targetVelocity = transform.forward * speed;
    }

    protected override void Fly()
    {
        if (!active)
            return;

        float dist = targetVelocity.magnitude * TimeManager.WorldDeltaTime;

        // adjust distance to account for edge case
        if (distanceCovered + dist >= range)
        {
            dist = range - distanceCovered;
        }

        // Check for hitting a wall or target
        RaycastHit target;
        if (Physics.SphereCast(transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, dist, hitLayers))
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

        // Update velocity with world timescale
        if (affectedByTimestop)
            rb.velocity = targetVelocity * TimeManager.WorldTimeScale;
        else
            rb.velocity = targetVelocity;
    }

    protected override bool CheckLife()
    {
        distanceCovered += targetVelocity.magnitude * TimeManager.WorldDeltaTime;

        return distanceCovered >= range;
    }

    public override void Inturrupt()
    {
        End();
    }

    /// <summary>
    /// Scale projectile over the distance if possible
    /// </summary>
    private void ScaleOverDistance()
    {
        if (scaleOverDistance != null && bulletVisual != null)
        {
            float newScale = scaleOverDistance.Evaluate(distanceCovered / range) * originalVisualScale;

            bulletVisual.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
