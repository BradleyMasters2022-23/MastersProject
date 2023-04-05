using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Laser : RangeAttack, TimeObserver
{
    [SerializeField] private float duration;
    private ScaledTimer durationTracker;
    [SerializeField] private VisualEffect originVFX;
    [SerializeField] private VisualEffect endVFX;
    [SerializeField] private LineRenderer beamLineRenderer;
    [SerializeField] private Color startBeamColor;
    [SerializeField] private Color endBeamColor;

    [SerializeField] private float laserRadius;
    [SerializeField] private bool solidInTimestop;

    [SerializeField] private CapsuleCollider col;

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
        

        RaycastHit hitInfo;
        float hitRange = range;
        // Check for wall collision first
        if (Physics.SphereCast(transform.position, laserRadius, transform.forward, out hitInfo, range, worldLayers))
        {
            Debug.Log("World hit");
            beamLineRenderer.SetPosition(1, hitInfo.point);
            hitRange = Vector3.Distance(hitInfo.point, transform.position);
        }

        // Update laser effect to hit wall, check if a damage target is between range
        beamLineRenderer.SetPosition(1, transform.position + transform.forward * hitRange);
        if (Physics.SphereCast(transform.position, laserRadius, transform.forward, out hitInfo, hitRange, hitLayers))
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

        TimeManager.instance.Subscribe(this);

        durationTracker = new ScaledTimer(duration, affectedByTimestop);
    }

    private void OnDisable()
    {
        TimeManager.instance.UnSubscribe(this);
    }

    private void ToggleSolid(bool solid)
    {
        if(solid)
        {
            col.radius = laserRadius;

            col.height = Vector3.Distance(beamLineRenderer.GetPosition(0), beamLineRenderer.GetPosition(1));
            col.center = new Vector3(0, 0, (col.height / 2));

            col.enabled = true;
        }
        else
        {
            col.enabled = false;
        }
    }

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
