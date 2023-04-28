using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererIndicator : IIndicator
{
    [SerializeField] private LineRenderer[] lines;
    [SerializeField] private float maxRange;
    [SerializeField] private LayerMask hitMask;

    bool active = false;

    public override void Activate()
    {
        active= true;
        foreach(var l in lines)
        {
            l.enabled = true;
        }
    }

    public override void Deactivate()
    {
        active= false;
        foreach (var l in lines)
        {
            l.enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!active || lines.Length <= 0) return;

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
    }
}
