using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnhancedShot : Projectile
{
    [Header("Enhanced Shots")]

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineDuration;


    private LocalTimer lineLifeTracker;

    public override void Activate()
    {
        base.Activate();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, lineRenderer.transform.position);

        RaycastHit target;
        if (Physics.SphereCast(transform.position, GetComponent<SphereCollider>().radius, transform.forward, out target, range, hitLayers))
        {
            lineRenderer.SetPosition(1, target.point);

        }
        else if (Physics.Raycast(transform.position, transform.forward, out target, range, worldLayers))
        {
            lineRenderer.SetPosition(1, target.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * range);
        }

        lineLifeTracker = GetTimer(lineDuration);
        lineRenderer.enabled = true;

        // on activate, register itself to any timeslow fieldds around
        Collider[] temp = Physics.OverlapSphere(transform.position, 0.1f, slowFieldLayers);
        TimeslowField field;
        foreach(var t in temp)
        {
            field = t.GetComponent<TimeslowField>();
            if(field != null)
            {
                field.SubToField(GetComponent<Collider>());
            }
        }
    }

    private void Update()
    {
        if (Timescale == 0 && lineRenderer.GetPosition(0) != transform.position)
            lineRenderer.SetPosition(0, lineRenderer.transform.position);

        if(lineRenderer.enabled && lineLifeTracker.TimerDone())
        {
            lineRenderer.enabled = false;
        }
    }
}
