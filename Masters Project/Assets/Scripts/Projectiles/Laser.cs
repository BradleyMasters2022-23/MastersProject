/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 6, 2022
 * Last Edited - April 6, 2022 by Ben Schuster
 * Description - Implementation for laser projectiles
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Laser : RangeAttack, TimeObserver
{
    [Header("===== Laser Info =====")]

    [Tooltip("Whether or not to attach to its owner on fire")]
    [SerializeField] private bool attachToOwner;
    [Tooltip("Radius of the laser")]
    [SerializeField] private float laserRadius;
    [Tooltip("Whether or not the laser is solid in timestop")]
    [SerializeField] private bool solidInTimestop;
    [Tooltip("Collider of the laser")]
    [SerializeField] private CapsuleCollider col;
    /// <summary>
    /// Tracker for the timed duration of the laser
    /// </summary>
    private ScaledTimer durationTracker;

    [Header("Laser Effects")]
    [Tooltip("Object playing the laser origin VFX, where the beam starts")]
    [SerializeField] private GameObject originVFX;
    [SerializeField] private float originVFXOffset;
    [Tooltip("Object playing the laser wall impact VFX, where the beam ends if it hits the world")]
    [SerializeField] private GameObject endVFX;
    [Tooltip("Reference to the line renderer this laser uses")]
    [SerializeField] private LineRenderer beamLineRenderer;

    [SerializeField] private float hitVFXCooldown;

    [SerializeField] private Color startBeamColor;
    [SerializeField] private Color endBeamColor;

    private ScaledTimer hitVFXCooldownTracker;

    /// <summary>
    /// Activate everything regarding the laser
    /// </summary>
    protected override void UniqueActivate()
    {
        if (owner != null && attachToOwner)
            transform.SetParent(owner.transform, true);

        beamLineRenderer.startColor = startBeamColor;
        beamLineRenderer.endColor = endBeamColor;
        beamLineRenderer.startWidth = laserRadius;
        beamLineRenderer.endWidth = laserRadius;

        if (solidInTimestop && col != null)
            TimeManager.instance.Subscribe(this);

        if (originVFX != null)
        {
            originVFX.SetActive(true);
            originVFX.transform.localPosition = Vector3.forward * originVFXOffset;
        }

        durationTracker = new ScaledTimer(range, affectedByTimestop);
        hitVFXCooldownTracker = new ScaledTimer(hitVFXCooldown);
    }

    private void OnDisable()
    {
        if(solidInTimestop && col!=null)
            TimeManager.instance.UnSubscribe(this);
    }

    private void FixedUpdate()
    {
        if (!active)
            return;

        if (CheckLife())
        {
            End();
            return;
        }


        Fly();
    }

    /// <summary>
    /// Perform flight behavior for laser, which is hitscan
    /// </summary>
    protected override void Fly()
    {
        // Update origin point and target point
        beamLineRenderer.SetPosition(0, transform.position);

        RaycastHit hitInfo;
        float hitRange = 999;
        // Check for wall collision first
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 999f, worldLayers))
        {
            beamLineRenderer.SetPosition(1, hitInfo.point);
            hitRange = Vector3.Distance(hitInfo.point, transform.position);

            // update the world hit VFX position and if its enabled or not
            if (endVFX != null)
            {
                if (!endVFX.activeInHierarchy)
                    endVFX.SetActive(true);

                endVFX.transform.position = hitInfo.point;
                endVFX.transform.LookAt(hitInfo.point + hitInfo.normal);
            }
        }
        // if no world hit, turn off VFX
        else if (endVFX != null && endVFX.activeInHierarchy)
        {
            endVFX.SetActive(false);
        }

        // Update laser effect to hit wall, check if a damage target is between range
        beamLineRenderer.SetPosition(1, transform.position + transform.forward * (hitRange + laserRadius));

        // get all targets between this and the end of the laser
        RaycastHit[] targetsHit =
            Physics.SphereCastAll(transform.position, laserRadius, transform.forward, hitRange + laserRadius, hitLayers);

        // deal damage and effects on all targets hit by sphere cast
        foreach (RaycastHit target in targetsHit)
        {
            // Check for hit cooldown to prevent overwhelming VFX spam
            if (hitVFXCooldownTracker.TimerDone())
                Hit(target.point, target.normal);

            ApplyDamage(target.collider.transform, target.point);
        }

        // If any target was hit and the cooldown is done, then reset the cooldown
        if (targetsHit.Length > 0 && hitVFXCooldownTracker.TimerDone())
            hitVFXCooldownTracker.ResetTimer();

        // Clear hit targets as the laser can hit infinite times
        // maybe rework later with dictionary and hit count or use damage field
        hitTargets.Clear();
    }


    protected override bool CheckLife()
    {
        return durationTracker.TimerDone();
    }

    public override void Inturrupt()
    {
        End();
    }

    protected override void End()
    {
        if (spawnProjectileOnEnd && onEndPrefab != null && (endVFX != null && endVFX.activeInHierarchy))
        {
            Instantiate(onEndPrefab, endVFX.transform.position, endVFX.transform.rotation);
        }
            

        Destroy(gameObject);
    }

    /// <summary>
    /// Align the collider with the current laser
    /// </summary>
    private void AlignCollider()
    {
        if (col == null) return;

        col.radius = laserRadius;

        col.height = Vector3.Distance(beamLineRenderer.GetPosition(0), beamLineRenderer.GetPosition(1));
        col.center = new Vector3(0, 0, (col.height / 2));
    }

    /// <summary>
    /// Toggle the solidness of the laser
    /// </summary>
    private void ToggleSolid(bool solid)
    {
        AlignCollider();

        if (col != null)
        {
            col.isTrigger = !solid;
            col.enabled = solid;
        }
    }

    // Dont use trigger stay, above spherecast is more performative for some reason
    //private void OnTriggerStay(Collider other)
    //{
    //    Vector3 target = other.ClosestPoint(transform.position);

    //    if (hitVFXCooldownTracker.TimerDone())
    //        Hit(target, (target - other.transform.position).normalized);

    //    ApplyDamage(other.transform, target);
    //}


    public void OnStop()
    {
        if(solidInTimestop)
            ToggleSolid(true);
    }

    public void OnResume()
    {
        if (solidInTimestop)
            ToggleSolid(false);
    }
}
