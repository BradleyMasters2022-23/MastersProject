/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 1st, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Core map loader manager. Manages initializing map pool and choosing what segments 
 * to load and in what order.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Events;

public class MapLoader : MonoBehaviour
{
    public enum States
    {
        Preparing,
        Room, 
        Hallway
    }

    #region Initialization Variables 

    /// <summary>
    /// Current state of the loader
    /// </summary>
    private States currState;

    [Tooltip("Hallways that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allHallways;
    [Tooltip("Rooms that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allRooms;

    [Tooltip("How many rooms to complete before loading ")]
    [SerializeField] private int maxFloorLength;
    
    /// <summary>
    /// Current number of floors
    /// </summary>
    private int currentFloorLength;

    [Tooltip("Starting room for the player"), SceneObjectsOnly, Required]
    [SerializeField] private GameObject startingSpot;

    [Tooltip("Final boss room"), AssetsOnly]
    [SerializeField] private GameObject finalRoom;

    [Tooltip("Final hallway room"), AssetsOnly]
    [SerializeField] private GameObject finalSpot;

    [Tooltip("The next point to use as an exit")]
    public Transform nextSpawnPoint;

    private GameObject lastRoomLoaded;

    /// <summary>
    /// Reference to this loader
    /// </summary>
    public static MapLoader instance;

    /// <summary>
    /// Navmesh for overall map
    /// </summary>
    private NavMeshSurface navMesh;

    #endregion

    #region Object Pooler Variables

    /// <summary>
    /// Track all pooled rooms that have not been used yet
    /// </summary>
    private List<GameObject> standbyRooms;
    /// <summary>
    /// Track all pooled hallways that have not been used yet
    /// </summary>
    private List<GameObject> standbyHallways;

    /// <summary>
    /// Track all pooled rooms that have recently been used
    /// </summary>
    private List<GameObject> usedRooms;
    /// <summary>
    /// Track all pooled hallways that have recently been used
    /// </summary>
    private List<GameObject> usedHallways;

    /// <summary>
    /// The current room being used
    /// </summary>
    private GameObject currentRoom;
    /// <summary>
    /// The current hallway being used
    /// </summary>
    private GameObject currentHallway;

    /// <summary>
    /// Collection of rooms being loaded in.
    /// </summary>
    private Queue<GameObject> loadedRooms;




    #endregion

    #region Events

    /// <summary>
    /// Event system for when a player enters a door
    /// </summary>
    [HideInInspector] public UnityEvent enterDoor;
    /// <summary>
    /// Event system for when a player exits a door
    /// </summary>
    [HideInInspector] public UnityEvent exitDoor;

    #endregion

    /// <summary>
    /// Initialize pools
    /// </summary>
    private void Awake()
    {
        // Prepare instance
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        // Create pool lists 
        standbyRooms = new List<GameObject>();
        usedRooms = new List<GameObject>();

        standbyHallways = new List<GameObject>();
        usedHallways = new List<GameObject>();

        // Populate pools
        for(int i = 0; i < allRooms.Count; i++)
        {
            GameObject room = Instantiate(allRooms[i].segmentPrefab);
            room.GetComponent<SegmentInterface>().SetSO(allRooms[i]);
            standbyRooms.Add(room);
        }

        // Populate hallways, 3 per
        for(int i = 0; i < allHallways.Count; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject hallway = Instantiate(allHallways[i].segmentPrefab);
                hallway.GetComponent<SegmentInterface>().SetSO(allHallways[i]);
                standbyHallways.Add(hallway);
            }
        }

        // Prepare Events
        enterDoor = new UnityEvent();
        exitDoor = new UnityEvent();

        enterDoor.AddListener(LoadNextSegment);
        exitDoor.AddListener(UnloadSegment);

        // Load the entrance into the loaded queue
        loadedRooms = new Queue<GameObject>();
        loadedRooms.Enqueue(startingSpot);

        // Prepare navmesh system
        navMesh = GetComponent<NavMeshSurface>();

        // Loading finished, start in a hallway segment
        currState = States.Hallway;
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());

        
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSecondsRealtime(1f);

        LoadNextSegment();

        yield return null;
    }

    #region Selection Functions

    /// <summary>
    /// Select a room, auto manages room container
    /// </summary>
    /// <returns>Selected room prefab</returns>
    private GameObject SelectRoom()
    {
        GameObject selectedObject;
        SegmentInterface selectedLoader;

        do
        {
            // If out of rooms, repopulate the pool
            if (standbyRooms.Count <= 0)
            {
                standbyRooms = new List<GameObject>(usedRooms);
                usedRooms.Clear();
            }

            // select a new room
            selectedObject = standbyRooms[Random.Range(0, standbyRooms.Count)];
            selectedLoader = selectedObject.GetComponent<SegmentInterface>();

            // Verify it is a hallway, otherwise remove from list and try again
            if (selectedLoader.segmentInfo.segmentType != MapSegmentSO.MapSegmentType.Room)
            {
                Debug.LogError($"[Map Loader] A non-room named {selectedObject.name} " +
                    $"was placed into room! Please remove from room list!");

                // Remove from pool permenately 
                standbyRooms.Remove(selectedObject);
            }
            
            // If out of difficulty or recently used, remove from pool and send to used 
            if(!selectedLoader.segmentInfo.WithinDifficulty(currentFloorLength)
                || (lastRoomLoaded != null && selectedObject == lastRoomLoaded))
            {
                standbyRooms.Remove(selectedObject);
                usedRooms.Add(selectedObject);
            }


        } while (selectedLoader.segmentInfo.segmentType != MapSegmentSO.MapSegmentType.Room);

        // Remove from current pool
        currentRoom = selectedObject;
        lastRoomLoaded = selectedObject;
        standbyRooms.Remove(selectedObject);

        // Return new room
        return selectedObject;
    }

    /// <summary>
    /// Select a hallway, auto manages hallway container
    /// </summary>
    /// <returns>Selected hallway prefab</returns>
    private GameObject SelectHallway()
    {
        // Select a random hallway, ensure its valid
        GameObject selectedObject;
        SegmentInterface selectedLoader;

        do
        {
            // If out of rooms, repopulate the pool
            if (standbyHallways.Count <= 0)
            {
                standbyHallways = new List<GameObject>(usedHallways);
                usedHallways.Clear();
            }

            // select a new room
            selectedObject = standbyHallways[Random.Range(0, standbyHallways.Count)];
            selectedLoader = selectedObject.GetComponent<SegmentInterface>();

            // Verify it is a hallway, otherwise remove from list and try again
            if (selectedLoader.segmentInfo.segmentType != MapSegmentSO.MapSegmentType.Hallway)
            {
                Debug.LogError($"[Map Loader] A non-hallway named {selectedObject.name} " +
                    $"was placed into hallway! Please remove from hallway list!");


                standbyHallways.Remove(selectedObject);
            }

        } while (selectedLoader.segmentInfo.segmentType != MapSegmentSO.MapSegmentType.Hallway);

        // Remove from pool to ensure it doesnt appear again
        currentHallway = selectedObject;
        standbyHallways.Remove(selectedObject);

        // Return new hallway
        return selectedObject;
    }

    #endregion

    #region Pool Management

    /// <summary>
    /// Decide what segment to load, load it
    /// </summary>
    public void LoadNextSegment()
    {
        Debug.Log("next segment called");

        GameObject nextSection;

        switch(currState)
        {
            case States.Room:
                {
                    nextSection = SelectHallway();
                    currState = States.Hallway;

                    break;
                }
            case States.Hallway:
                {
                    nextSection = SelectRoom();
                    currentFloorLength++;

                    currState = States.Room;

                    break;
                }
            default:
                {
                    Debug.LogError("[MapLoader] Trying to load segment before initialization finished!");
                    return;
                }
        }

        Debug.Log("Loading... " + nextSection.name);

        // Add loaded segment to queue
        loadedRooms.Enqueue(nextSection);

        // Prepare spawnpoint
        SegmentInterface sectionManager = nextSection.GetComponent<SegmentInterface>();
        sectionManager.RetrieveFromPool(nextSpawnPoint);

        navMesh.BuildNavMesh();

        nextSpawnPoint = sectionManager.GetNextExit();
    }

    /// <summary>
    /// Unload the oldest item in the load queue
    /// </summary>
    private void UnloadSegment()
    {
        Debug.Log("unload segment called");

        SegmentInterface t;

        if(loadedRooms.Peek().TryGetComponent<SegmentInterface>(out t))
        {
            loadedRooms.Dequeue();
            t.ResetToPool();


            if(t.segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Hallway)
            {
                usedHallways.Add(t.root);

            }
            else if(t.segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Room)
            {
                usedRooms.Add(t.root);
            }
        }
        else
        {
            Destroy(loadedRooms.Dequeue());
        }
    }

    #endregion


    //private IEnumerator ContinueLoad()
    //{
    //    while(currentFloorLength <= maxFloorLength)
    //    {
    //        LoadNextSegment();

    //        if (currentFloorLength >= 1)
    //            UnloadSegment();

    //        yield return new WaitForSecondsRealtime(2f);

    //        yield return null;
    //    }
    //}

    private void OnDestroy()
    {
        instance = null;
        enterDoor = null;
        exitDoor = null;
    }
}
