using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererIndicator : IIndicator
{
    [SerializeField] protected LineRenderer[] lines;
    [SerializeField] protected float maxRange;
    [SerializeField] protected LayerMask hitMask;

    protected bool active = false;

    public override void Activate()
    {
        active= true;
        foreach(var l in lines)
        {
            l.enabled = true;
        }

        if(lines.Length > 0)
            StartCoroutine(Laser());
    }

    public override void Deactivate()
    {
        StopAllCoroutines();

        active= false;
        foreach (var l in lines)
        {
            l.enabled = false;
        }
    }


    // Continuously update laser renderers
    protected IEnumerator Laser()
    {
        while(active)
        {
            // Get max range
            float dist = maxRange;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, dist, hitMask))
                dist = hit.distance;

            // Update positions
            foreach (var l in lines)
            {
                l.SetPosition(0, l.transform.position);
                l.SetPosition(1, l.transform.position + transform.forward * dist);
            }

            yield return null;
        }
    }
}
