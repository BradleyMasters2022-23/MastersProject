/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 1st, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Abstract base class for loading a map segment
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SegmentLoader : MonoBehaviour, SegmentInterface
{
    #region Core Data Variables

    /// <summary>
    /// Reference to the segment's SO data
    /// </summary>
    [HideInInspector] public MapSegmentSO segmentInfo;
    /// <summary>
    /// Getter for interface
    /// </summary>
    MapSegmentSO SegmentInterface.segmentInfo { get => segmentInfo; }

    /// <summary>
    /// All available syncpoints for this object
    /// </summary>
    protected List<Transform> syncPoints;

    /// <summary>
    /// All syncpoints found for this object
    /// </summary>
    protected List<Transform> allSyncPoints;

    #endregion

    /// <summary>
    /// The root of this prefab
    /// </summary>
    protected Transform root;
    /// <summary>
    /// Getter for interface
    /// </summary>
    GameObject SegmentInterface.root => root.gameObject;

    /// <summary>
    /// The transform used to enter this map segment
    /// </summary>
    protected Transform startPoint;

    /// <summary>
    /// Buffer transform that uses syncpoint data to rotate the map
    /// </summary>
    private Transform syncBuffer;

    /// <summary>
    /// Door manager for this segment
    /// </summary>
    private DoorManager doorManager;

    /// <summary>
    /// Initialize internal variables
    /// </summary>
    protected void Awake()
    {
        // Initialize internal variables
        root = transform;
        allSyncPoints = new List<Transform>();
        doorManager = GetComponent<DoorManager>();
        doorManager.InitializeDoorManager();

        // Initialize sync buffer automatically
        syncBuffer = new GameObject("SyncBuffer").transform;
        syncBuffer.parent = root;
        syncBuffer.localPosition = Vector3.zero;

        // Get all syncpoints in scene
        Door[] potentialPoints = GetComponentsInChildren<Door>(true);
        foreach (Door d in potentialPoints)
        {
            allSyncPoints.Add(d.SyncPoint);
        }

        // Load syncpoints into usable syncpoints
        syncPoints = new List<Transform>(allSyncPoints);

        // Do any unique initialization
        UniquePoolInitialization();

        // On load, deactivate self
        ResetToPool();
    }

    #region Pooling Functions

    /// <summary>
    /// Choose a start point, adjust map accordingly
    /// </summary>
    public void RetrieveFromPool(Transform syncPoint)
    {
        SyncSegment(syncPoint);

        UniquePoolPull();
    }

    /// <summary>
    /// Sync this object to the point passed in
    /// </summary>
    /// <param name="syncPoint">The point to sync to</param>
    protected void SyncSegment(Transform syncPoint)
    {
        // select start point, remove from pool
        startPoint = doorManager.SelectEntrance();
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
        syncBuffer.position = syncPoint.position;
        syncBuffer.rotation = syncPoint.rotation;

        // Enable
        gameObject.SetActive(true);

        // DBUG
        doorManager.UnlockAllDoors();
    }

    /// <summary>
    /// Reset this segment to the pool, hiding it and preparing it for the next use
    /// </summary>
    public void ResetToPool()
    {
        // Rotate each other sync point to point in default position direction, link to map
        if (syncPoints != null)
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

        // clear startpoint buffer
        startPoint = null;

        // reset doors
        doorManager.ResetDoors();

        UniquePoolReturn();
    }

    #endregion

    #region Utility Functions

    /// <summary>
    /// Set this segment's SO reference
    /// </summary>
    /// <param name="so">ScriptableObject containing this segments info</param>
    public void SetSO(MapSegmentSO so)
    {
        segmentInfo = so;
    }


    /// <summary>
    /// Choose an exit from the pool
    /// </summary>
    /// <returns>The exit chosen</returns>
    public Transform GetNextExit()
    {
        return doorManager.SelectExit();
    }

    #endregion

    #region Abstract Stuff

    /// <summary>
    /// Perform any unique pool initialization needed
    /// </summary>
    protected abstract void UniquePoolInitialization();

    /// <summary>
    /// Perform any unique sync when pulling from pull
    /// </summary>
    protected abstract void UniquePoolPull();

    /// <summary>
    /// Perform any unique cleanup when returning to pull
    /// </summary>
    protected abstract void UniquePoolReturn();

    #endregion
}
