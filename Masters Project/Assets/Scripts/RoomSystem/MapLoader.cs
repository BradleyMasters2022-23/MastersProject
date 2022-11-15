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
    public Transform startingSpawnPoint;

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

    #region Big Map Variables

    [Header("=== BIG MAP VARS ===")]

    [SerializeField, ReadOnly] private List<MapSegmentSO> mapOrder;
    [SerializeField, ReadOnly] private List<SegmentLoader> loadedMap;
    public int roomCount;
    
    [SerializeField] private List<MapSegmentSO> availableRooms;
    [SerializeField] private List<MapSegmentSO> availableHalls;

    public int roomIndex;

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
        mapOrder = new List<MapSegmentSO>();
        StartCoroutine(PrepareMapSegments());
    }

    private IEnumerator PrepareMapSegments()
    {
        // === Choose what rooms to use === //

        while(roomCount < maxFloorLength)
        {
            mapOrder.Add(ChooseNextSegment());
        }
        Debug.Log("Segments prepared");

        // === Spawn in each room, initialize whats required ===/

        loadedMap = new List<SegmentLoader>();
        foreach(MapSegmentSO segment in mapOrder)
        {
            // Create map segment, give its info to it, add to list
            GameObject t = Instantiate(segment.segmentPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
            t.GetComponent<SegmentLoader>().segmentInfo = segment;
            loadedMap.Add(t.GetComponent<SegmentLoader>());

            //t.SetActive(false);

            // Initialize each of its own components
            MapInitialize[] test = t.GetComponents<MapInitialize>();
            for(int i = 0; i < test.Length; i++)
            {
                StartCoroutine(test[i].InitializeComponent());
            }
        }
        Debug.Log("Each component initialized");

        // === Sync up each room === //

        // Make sure last item is initialized before starting
        MapInitialize lastDoorRef = loadedMap[loadedMap.Count - 1].GetComponent<MapInitialize>();
        while (!lastDoorRef.initialized)
        {
            yield return null;
        }
        Debug.Log("Starting syncing!!!");

        // Pair first item to the start point
        loadedMap[0].Sync(startingSpawnPoint);
        // Pair all other consecutive rooms
        for(int i = 0; i < loadedMap.Count-1; i++)
        {
            loadedMap[i + 1].Sync(loadedMap[i].GetExit());
        }
        Debug.Log("Sync finished!");

        // === Set later rooms inactive === //
        // Activate first room and hallway, deactivate rest
        for(int i = 0; i < 2; i++)
        {
            loadedMap[i].ActivateSegment();
        }
        for(int i = 2; i < loadedMap.Count; i++)
        {
            loadedMap[i].DeactivateSegment();
        }
        Debug.Log("Initial inactive set!");

        roomIndex = -1;

        //yield return new WaitForSeconds(5f);

        //StartCoroutine(ContinueLoad());

        yield return null;
    }

    #region Selection Functions

    /// <summary>
    /// Get the next segment based on the last segment in the list
    /// </summary>
    /// <returns>Next segment to add onto map order</returns>
    private MapSegmentSO ChooseNextSegment()
    {
        if(mapOrder.Count == 0
            || mapOrder[mapOrder.Count-1].segmentType == MapSegmentSO.MapSegmentType.Hallway)
        {
            roomCount++;
            return SelectRoom();
        }
        else
        {
            return SelectHallway();
        }
    }

    /// <summary>
    /// Select a room, auto manages room container
    /// </summary>
    /// <returns>Selected room prefab</returns>
    private MapSegmentSO SelectRoom()
    {
        MapSegmentSO selectedObject;

        int c = 0;

        do
        {
            // If out of rooms, repopulate the pool
            if (availableRooms.Count <= 0)
            {
                availableRooms = new List<MapSegmentSO>(allRooms);
            }

            // select a new room
            selectedObject = availableRooms[Random.Range(0, availableRooms.Count)];

            // Verify it is a room, otherwise remove from list and try again
            if (selectedObject.segmentType != MapSegmentSO.MapSegmentType.Room)
            {
                Debug.LogError($"[Map Loader] A non-room named {selectedObject.name} " +
                    $"was placed into room! Please remove from room list!");

                // Remove from pool permenately 
                allRooms.Remove(selectedObject);
            }

            // If out of difficulty or recently used, remove from pool and send to used 
            if (!selectedObject.WithinDifficulty(currentFloorLength))
            {;
                availableRooms.Remove(selectedObject);
                continue;
            }

            // Make sure the last room is not the same
            if(mapOrder.Count >= 2 && selectedObject == mapOrder[mapOrder.Count - 2])
            {
                availableRooms.Remove(selectedObject);
                selectedObject = null;
                continue;
            }

            // Backup failsafe check
            c++;
            if (c >= 100000)
            {
                Debug.LogError($"[Map Loader] Select room 2 is stuck in a loop! Breaking!");
                Debug.DebugBreak();
                return null;
            }

        } while (selectedObject == null || selectedObject.segmentType != MapSegmentSO.MapSegmentType.Room) ;

        // Remove from current pool
        availableRooms.Remove(selectedObject);

        // Return new room
        return selectedObject;
    }

    /// <summary>
    /// Select a hallway, auto manages hallway container
    /// </summary>
    /// <returns>Selected hallway prefab</returns>
    private MapSegmentSO SelectHallway()
    {
        // Select a random hallway, ensure its valid
        MapSegmentSO selectedObject;

        int c = 0;

        do
        {
            // If out of rooms, repopulate the pool
            if (availableHalls.Count <= 0)
            {
                availableHalls = new List<MapSegmentSO>(allHallways);
            }

            // select a new room
            selectedObject = availableHalls[Random.Range(0, availableHalls.Count)];

            // Verify it is a hallway, otherwise remove from list and try again
            if (selectedObject.segmentType != MapSegmentSO.MapSegmentType.Hallway)
            {
                Debug.LogError($"[Map Loader] A non-hallway named {selectedObject.name} " +
                    $"was placed into hallway! Please remove from hallway list!");


                allHallways.Remove(selectedObject);
            }

            // Backup failsafe check
            c++;
            if (c >= 100000)
            {
                Debug.LogError($"[Map Loader] Select room 2 is stuck in a loop! Breaking!");
                Debug.DebugBreak();
                return null;
            }

        } while (selectedObject.segmentType != MapSegmentSO.MapSegmentType.Hallway);

        // Remove from pool to ensure it doesnt appear again
        availableHalls.Remove(selectedObject);

        // Return new hallway
        return selectedObject;
    }

    #endregion

    /// <summary>
    /// Deactivate passed sections and prepare the new ones
    /// </summary>
    public void UpdateLoadedSegments()
    {
        // Increment room index. Do by 2 because its accounting for room and hallway
        roomIndex += 2;

        // Deactivate the last 2 sections, if possible
        loadedMap[roomIndex - 1].DeactivateSegment();
        if (roomIndex - 2 >= 0)
            loadedMap[roomIndex - 2].DeactivateSegment();
        else
            startingSpot.SetActive(false);

        // Activate the next two sections, if possible
        if (roomIndex+1 < loadedMap.Count)
            loadedMap[roomIndex + 1].ActivateSegment();
        if (roomIndex + 2 < loadedMap.Count)
            loadedMap[roomIndex + 2].ActivateSegment();
    }

    public void StartRoomEncounter()
    {
        Debug.Log($"Pew Bang! Encounter started for room at index {roomIndex + 1}!");
    }

    #region Utility

    private IEnumerator ContinueLoad()
    {
        while (roomIndex < loadedMap.Count)
        {
            UpdateLoadedSegments();


            yield return new WaitForSecondsRealtime(3f);

            yield return null;
        }
        yield return null;
    }

    private void OnDestroy()
    {
        instance = null;
        enterDoor = null;
        exitDoor = null;
    }

    #endregion
}
