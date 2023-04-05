using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Laser : RangeAttack
{
    [SerializeField] private float duration;
    private ScaledTimer durationTracker;
    [SerializeField] private VisualEffect originVFX;
    [SerializeField] private VisualEffect endVFX;
    [SerializeField] private LineRenderer beamLineRenderer;
    [SerializeField] private Color startBeamColor;
    [SerializeField] private Color endBeamColor;

    [SerializeField] private float laserRadius;

    

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

    public override void Inturrupt()
    {
        return;
    }

    protected override bool CheckLife()
    {
        return durationTracker.TimerDone();
    }

    protected override void Fly()
    {
        // Update origin point and target point
        beamLineRenderer.SetPosition(0, transform.position);
        beamLineRenderer.SetPosition(1, transform.position + transform.forward * range);

        RaycastHit hitInfo;
        // Check if player was hit 
        if (Physics.SphereCast(transform.position, laserRadius, transform.forward, out hitInfo, range, hitLayers))
        {
            Hit(hitInfo.point, hitInfo.normal);
            ApplyDamage(hitInfo.transform, hitInfo.point);
        }

        hitTargets.Clear();
    }

    protected override void UniqueActivate()
    {
        if (owner != null)
            transform.SetParent(owner.transform, true);

        beamLineRenderer.startColor= startBeamColor;
        beamLineRenderer.endColor= endBeamColor;
        beamLineRenderer.startWidth = laserRadius*2;
        beamLineRenderer.endWidth = laserRadius * 2;


        durationTracker = new ScaledTimer(duration, affectedByTimestop);
    }

}
