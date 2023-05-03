using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericWaypointContainer : MonoBehaviour
{
    [SerializeField] private Transform targetOverride;

    [SerializeField] private WaypointData displayData;

    protected Waypoint wp;

    private void OnEnable()
    {
        // If no override done, use attached transform
        if (targetOverride == null)
            targetOverride = transform;

        wp = GetWaypoint(targetOverride, displayData);
    }

    private void OnDisable()
    {
        if (wp != null)
        {
            ReturnWaypoint();
            wp = null;
        }
    }
    public WaypointData GetData()
    {
        return displayData;
    }

    protected abstract Waypoint GetWaypoint(Transform targetOverride, WaypointData data);
    protected abstract void ReturnWaypoint();

    
}
