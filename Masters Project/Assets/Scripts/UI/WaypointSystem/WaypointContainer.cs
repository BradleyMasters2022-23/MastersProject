/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 17th, 2023
 * Last Edited - April 17th, 2023 by Ben Schuster
 * Description - Waypoint container that requests waypoints while active. 
 * Used to help create more custom waypoints easily
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointContainer : MonoBehaviour
{
    [SerializeField] private Transform targetOverride;

    [SerializeField] private WaypointData displayData;

    private Waypoint wp;

    private WaypointManager i;

    private void OnEnable()
    {
        if (WaypointManager.instance == null)
        {
            Debug.Log("No waypoint manager found!");
            Destroy(this);
            return;
        }
        else if(i == null)
        {
            i = WaypointManager.instance;
        }

        // If no override done, use attached transform
        if (targetOverride == null)
            targetOverride = transform;

        wp = i.Request(targetOverride, displayData);
    }

    private void OnDisable()
    {
        if(wp != null)
        {
            i.Return(wp);
            wp = null;
        }
    }
}
