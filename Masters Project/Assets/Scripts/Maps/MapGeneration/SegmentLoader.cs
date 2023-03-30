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

public abstract class SegmentLoader : MonoBehaviour, SegmentInterface, MapInitialize
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
    protected DoorManager doorManager;

    /// <summary>
    /// Whether this segment has finished initializing
    /// </summary>
    protected bool initialized;
    public bool Initialized { get { return initialized; } }
    bool MapInitialize.initialized => initialized;

    protected IRandomizer[] randomizedObjs;

    public IEnumerator InitializeComponent()
    {
        // Get references
        root = transform;
        syncPoints = new List<Transform>();
        doorManager = GetComponent<DoorManager>();
        randomizedObjs = GetComponentsInChildren<IRandomizer>(false);

        // Initialize sync buffer automatically
        syncBuffer = new GameObject("SyncBuffer").transform;
        syncBuffer.parent = root;
        syncBuffer.localPosition = Vector3.zero;


        // Get sync points when its initialized
        while (!doorManager.Initialized)
            yield return null;
        syncPoints = doorManager.GetSyncpoints();

        if (segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Hallway)
            doorManager.SetHallway();

        // Do any unique initialization
        StartCoroutine(UniquePoolInitialization());

        yield return StartCoroutine(LoadRoom(false));

        yield return null;
    }


    #region Segment Interface Implementaiton

    /// <summary>
    /// Choose a start point, adjust map accordingly
    /// </summary>
    public void Sync(Transform syncPoint)
    {
        // select start point, remove from pool
        startPoint = doorManager.GetEntrance().SyncPoint;
        syncPoints.Remove(startPoint);

        // Rotate each other sync point to point in exit direction, link to map
        foreach (Transform t in syncPoints)
        {
            t.transform.Rotate(Vector3.up, 180);
        }

        syncBuffer.SetPositionAndRotation(startPoint.position, startPoint.rotation);

        // set parent to help sync
        Transform syncBufferParent = syncBuffer.parent;
        syncBuffer.parent = null;
        root.transform.SetParent(syncBuffer, true);

        // Sync
        syncBuffer.position = syncPoint.position;
        syncBuffer.rotation = syncPoint.rotation;

        // Revert parents post sync
        root.transform.parent = null;
        syncBuffer.transform.parent = syncBufferParent;

        //doorManager.UnlockExit();
    }

    /// <summary>
    /// Activate this component of the object. Do anything needed here
    /// </summary>
    public IEnumerator ActivateSegment()
    {
        yield return StartCoroutine(LoadRoom(true));

        // Tell randomized objects to initiate randomization
        foreach (IRandomizer obj in randomizedObjs)
        {
            obj.Randomize();
        }


        UniqueActivate();
    }

    private IEnumerator LoadRoom(bool enabled)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(enabled);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }


    /// <summary>
    /// Reset this segment to the pool, hiding it and preparing it for the next use
    /// </summary>
    public IEnumerator DeactivateSegment(bool instant)
    {
        if(instant)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            yield return StartCoroutine(LoadRoom(false));
        }

        UniqueDeactivate();
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
    public Transform GetExit()
    {
        return doorManager.GetExit().SyncPoint;
    }

    #endregion

    #region Abstract Stuff

    /// <summary>
    /// Perform any unique pool initialization needed
    /// </summary>
    protected abstract IEnumerator UniquePoolInitialization();

    /// <summary>
    /// Perform any unique sync when pulling from pull
    /// </summary>
    protected abstract void UniqueActivate();

    /// <summary>
    /// Perform any unique cleanup when returning to pull
    /// </summary>
    protected abstract void UniqueDeactivate();

    public abstract void StartSegment();

    #endregion
}
