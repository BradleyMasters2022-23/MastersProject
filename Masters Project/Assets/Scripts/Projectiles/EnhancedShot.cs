using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedShot : Projectile
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineDuration;


    private ScaledTimer lineLifeTracker;

    public override void Activate()
    {
        base.Activate();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit target;
        if (Physics.SphereCast(transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, range, targetLayers))
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * target.distance);

        }
        else if (Physics.Raycast(transform.position, transform.forward, out target, range, groundLayer))
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * target.distance);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * range);
        }

        lineLifeTracker = new ScaledTimer(lineDuration);
        lineRenderer.enabled = true;
    }

    private void Update()
    {
        if (TimeManager.WorldTimeScale == 0 && lineRenderer.GetPosition(0) != transform.position)
            lineRenderer.SetPosition(0, transform.position);

        if(lineRenderer.enabled && lineLifeTracker.TimerDone())
        {
            lineRenderer.enabled = false;
        }
    }
}
