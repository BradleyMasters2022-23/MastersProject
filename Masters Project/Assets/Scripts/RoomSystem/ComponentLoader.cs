using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentLoader : MonoBehaviour
{
    public List<Transform> syncPoints;

    public GameObject mapSection;

    public Transform startPoint;


    public void PrepareStartPoint()
    {
        // select start point, remove from pool
        startPoint = syncPoints[Random.Range(0, syncPoints.Count)];
        syncPoints.Remove(startPoint);

        // Rotate each other sync point to point in exit direction, link to map
        foreach (Transform t in syncPoints)
        {
            t.SetParent(mapSection.transform, true);

            t.transform.Rotate(Vector3.up, 180);
        }

        // set parent to help sync
        mapSection.transform.SetParent(startPoint, true);
    }

    /// <summary>
    /// Move the loaded component to the sync point
    /// </summary>
    /// <param name="syncPoint"></param>
    public void PrepareComponent(Transform syncPoint)
    {
        startPoint.position = syncPoint.position;
        startPoint.rotation = syncPoint.rotation;
    }

    public Transform NextExit()
    {
        // select exit point, remove from pool
        Transform exit = syncPoints[Random.Range(0, syncPoints.Count)];
        
        return exit;
    }
}
