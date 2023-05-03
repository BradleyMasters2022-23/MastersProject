using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[System.Serializable]
public struct WaypointData
{
    public Color displayColor;
    public float maxRange;
    public float minRange;
    public Sprite icon;
}

public abstract class GenericWaypointManager<T> : MonoBehaviour where T : Waypoint
{
    //public static WaypointManager instance;

    [SerializeField] GameObject waypointPrefab;
    [SerializeField, ReadOnly] Queue<T> waypoints;


    [SerializeField] int startingSize;
    [SerializeField] int incrementAmt;

    protected virtual void Awake()
    {
        waypoints = new Queue<T>(startingSize);

        // Load in initial amount as requested
        for (int i = 0; i < startingSize; i++)
        {
            T newWP = Instantiate(waypointPrefab, transform).GetComponent<T>();
            // move them off screen when spawning
            newWP.transform.position = Vector3.left * 1000;
            newWP.Init();
            newWP.gameObject.SetActive(false);
            waypoints.Enqueue(newWP);
        }
    }

    /// <summary>
    /// Request a waypoint
    /// </summary>
    /// <param name="target">Target to display to</param>
    /// <param name="data">Data to load into waypoint</param>
    /// <returns>A waypoint to use</returns>
    public T Request(Transform target, WaypointData data)
    {
        T wp;

        // Get a new waypoint if possible
        if (waypoints.Count <= 0)
            wp = IncreasePool();
        else
            wp = waypoints.Dequeue();

        // Pass in the waypoint data
        wp.AssignTarget(target, data);

        // Set active, return
        wp.gameObject.SetActive(true);
        return wp;
    }

    /// <summary>
    /// Increase the pool of waypoints
    /// </summary>
    /// <returns>One of the new waypoints recently created</returns>
    private T IncreasePool()
    {
        // incremenet and initialize by the requested amount
        for (int i = 0; i < incrementAmt; i++)
        {
            T newWP = Instantiate(waypointPrefab, transform).GetComponent<T>();
            // move them off screen when spawning
            newWP.transform.position = Vector3.left * 1000;
            newWP.Init();
            newWP.gameObject.SetActive(false);
            waypoints.Enqueue(newWP);
        }

        // Return a new one
        return waypoints.Dequeue();
    }

    /// <summary>
    /// Return the waypoint to the pool
    /// </summary>
    /// <param name="wp">waypoint to return</param>
    public void Return(T wp)
    {
        // move them off screen when spawning
        wp.transform.position = Vector3.left * 1000;
        wp.gameObject.SetActive(false);
        wp.ReturnToPool();
        waypoints.Enqueue(wp);
    }
}
