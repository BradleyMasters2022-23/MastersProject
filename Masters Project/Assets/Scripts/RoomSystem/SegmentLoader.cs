using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SegmentLoader : MonoBehaviour
{
    /// <summary>
    /// All possibly syncpoints in this room
    /// </summary>
    private List<Transform> syncPoints;

    /// <summary>
    /// The overall prefab itself
    /// </summary>
    private GameObject mapSection;

    /// <summary>
    /// The transform used to enter this map segment
    /// </summary>
    private Transform startPoint;

    /// <summary>
    /// Reference to the navmesh of this segment
    /// </summary>
    private NavMeshSurface navMesh;


    /// <summary>
    /// Initialize internal variables
    /// </summary>
    private void Awake()
    {
        mapSection = gameObject;

        syncPoints = new List<Transform>();

        navMesh = GetComponentInChildren<NavMeshSurface>();

        // Get all syncpoints attached to this segment. Used like this to ensure other segments arent grabbed.
        GameObject[] potentialPoints = GameObject.FindGameObjectsWithTag("SyncPoint");
        foreach(GameObject points in potentialPoints)
        {
            if(IsChild(points))
            {
                syncPoints.Add(points.transform);
            }
        }

        // select start point, remove from pool
        startPoint = syncPoints[Random.Range(0, syncPoints.Count)];
        syncPoints.Remove(startPoint);
    }

    /// <summary>
    /// Check if the given gameobject is a child of this object
    /// </summary>
    /// <param name="target">Game object to check</param>
    /// <returns>Whether the target is a child of this object</returns>
    private bool IsChild(GameObject target)
    {
        Transform pointer = target.transform.parent;

        // Try checking
        while(pointer != null && pointer != transform)
        {
            pointer = pointer.parent;
        }

        return pointer == transform;
    }

    /// <summary>
    /// Choose a start point, adjust map accordingly
    /// </summary>
    public void PrepareStartPoint(Transform syncPoint)
    {
        StartCoroutine(LoadSegment(syncPoint));
    }

    private IEnumerator LoadSegment(Transform syncPoint)
    {
        // Wait for syncpoints to be initialized
        while(startPoint is null)
        {
            yield return null;
        }

        // Rotate each other sync point to point in exit direction, link to map
        foreach (Transform t in syncPoints)
        {
            t.SetParent(mapSection.transform, true);

            t.transform.Rotate(Vector3.up, 180);
        }

        // set parent to help sync
        startPoint.parent = null;
        mapSection.transform.SetParent(startPoint, true);

        // Sync
        PrepareComponent(syncPoint);

        yield return null;
    }


    /// <summary>
    /// Move the loaded component to the sync point
    /// </summary>
    /// <param name="syncPoint">Point to sync with</param>
    private void PrepareComponent(Transform syncPoint)
    {
        startPoint.position = syncPoint.position;
        startPoint.rotation = syncPoint.rotation;
    }

    /// <summary>
    /// Choose an exit from the pool
    /// </summary>
    /// <returns></returns>
    public Transform NextExit()
    {
        // select exit point, remove from pool
        Transform exit = syncPoints[Random.Range(0, syncPoints.Count)];
        
        return exit;
    }
}
