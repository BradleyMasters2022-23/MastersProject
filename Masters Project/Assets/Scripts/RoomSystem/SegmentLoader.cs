using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SegmentLoader : MonoBehaviour
{
    /// <summary>
    /// All available syncpoints for this object
    /// </summary>
    private List<Transform> syncPoints;

    /// <summary>
    /// All syncpoints found for this object
    /// </summary>
    private List<Transform> allSyncPoints;

    /// <summary>
    /// Reference to the segment's SO data
    /// </summary>
    public MapSegmentSO segmentInfo;

    /// <summary>
    /// The transform used to enter this map segment
    /// </summary>
    private Transform startPoint;

    /// <summary>
    /// Buffer transform that uses syncpoint data to rotate the map
    /// </summary>
    public Transform syncBuffer;

    /// <summary>
    /// The root of this prefab
    /// </summary>
    private Transform root;

    /// <summary>
    /// Initialize internal variables
    /// </summary>
    private void Awake()
    {
        root = transform;

        allSyncPoints = new List<Transform>();


        // Get all syncpoints in scene
        GameObject[] potentialPoints = GameObject.FindGameObjectsWithTag("SyncPoint");
        // Make sure these are syncpoints in this prefab, not from the others
        foreach (GameObject points in potentialPoints)
        {
            if (points.transform.root == root)
            {
                allSyncPoints.Add(points.transform);
            }
        }

        // Load syncpoints into usable syncpoints
        syncPoints = new List<Transform>(allSyncPoints);

        // On load, deactivate self
        ResetToPool();
    }

    /// <summary>
    /// Set this segment's SO reference
    /// </summary>
    /// <param name="so">ScriptableObject containing this segments info</param>
    public void SetSO(MapSegmentSO so)
    {
        segmentInfo = so;
    }

    /// <summary>
    /// Choose a start point, adjust map accordingly
    /// </summary>
    public void RetrieveFromPool(Transform syncPoint)
    {
        gameObject.SetActive(true);

        StartCoroutine(LoadSegment(syncPoint));
    }

    public void ResetToPool()
    {
        // Rotate each other sync point to point in default position direction, link to map
        if(syncPoints != null)
        {
            foreach (Transform t in syncPoints)
            {
                //t.SetParent(root, true);

                t.transform.Rotate(Vector3.up, 180);
            }
        }
        syncPoints = new List<Transform>(allSyncPoints);
        
        // Set root back to root, reset sync buffer
        root.parent = null;
        syncBuffer.SetParent(root, true);

        // Disable, move out of the way
        root.gameObject.SetActive(false);
        //root.position = transform.forward * 100f;

        // clear startpoint buffer
        startPoint = null;
    }


    private IEnumerator LoadSegment(Transform syncPoint)
    {
        // select start point, remove from pool
        startPoint = syncPoints[Random.Range(0, syncPoints.Count)];
        syncPoints.Remove(startPoint);

        // Rotate each other sync point to point in exit direction, link to map
        foreach (Transform t in syncPoints)
        {
            //t.SetParent(root, true);

            t.transform.Rotate(Vector3.up, 180);
        }

        syncBuffer.SetPositionAndRotation(startPoint.position, startPoint.rotation);

        // set parent to help sync
        syncBuffer.parent = null;
        root.transform.SetParent(syncBuffer, true);

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
        syncBuffer.position = syncPoint.position;
        syncBuffer.rotation = syncPoint.rotation;
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
