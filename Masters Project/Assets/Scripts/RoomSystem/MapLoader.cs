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
using Unity.VisualScripting;

public class MapLoader : MonoBehaviour
{
    public enum States
    {
        Preparing,
        Start,
        Room, 
        Hallway,
        Final
    }

    /// <summary>
    /// Reference to this loader
    /// </summary>
    public static MapLoader instance;

    /// <summary>
    /// Current state of the loader
    /// </summary>
    private States currState;

    /// <summary>
    /// Navmesh for overall map
    /// </summary>
    private NavMeshSurface navMesh;

    #region Initialization Variables 

    [Tooltip("Hallways that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allHallways;
    [Tooltip("Rooms that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allRooms;

    [Tooltip("How many rooms to complete before loading ")]
    [SerializeField] private int maxFloorLength;

    [Tooltip("Root of the starting room"), SceneObjectsOnly, Required]
    [SerializeField] private GameObject startingRoom;

    [Tooltip("Final boss room"), AssetsOnly]
    [SerializeField] private MapSegmentSO finalRoom;

    #endregion

    #region Big Map Variables

    [Header("=== Big Map Variables ===")]

    [Tooltip("The current order of maps to be played. Randomized at runtime")]
    [SerializeField, ReadOnly, HideInEditorMode] private List<MapSegmentSO> mapOrder;
    /// <summary>
    /// All segments loaded into the scene, in order
    /// </summary>
    private List<SegmentLoader> loadedMap;
    
    private List<MapSegmentSO> availableRooms;
    private List<MapSegmentSO> availableHalls;

    /// <summary>
    /// Current index for looking through rooms
    /// </summary>
    private int roomIndex;

    #endregion

    #region Initialization

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
        loadedMap = new List<SegmentLoader>();

        availableRooms = new List<MapSegmentSO>();
        availableHalls = new List<MapSegmentSO>();

        navMesh = GetComponent<NavMeshSurface>();
        StartCoroutine(PrepareMapSegments());
    }

    /// <summary>
    /// Initialize the entire map
    /// </summary>
    /// <returns></returns>
    private IEnumerator PrepareMapSegments()
    {
        // === Choose what rooms to use === //
        int c = 0;
        while(c < maxFloorLength)
        {
            // Add new segment, check if a room was added. If so, add
            mapOrder.Add(ChooseNextSegment());
            if (mapOrder[mapOrder.Count - 1].segmentType == MapSegmentSO.MapSegmentType.Room)
                c++;
        }

        // Load in the final room, if there is one. Add hallway buffer
        if(finalRoom != null)
        {
            mapOrder.Add(SelectHallway());
            mapOrder.Add(finalRoom);
        }

        Debug.Log("[MapLoader] Order Prepared");

        // === Spawn in each room, initialize whats required ===/

        foreach(MapSegmentSO segment in mapOrder)
        {
            // Create map segment, give its info to it, add to list
            GameObject t = Instantiate(segment.segmentPrefab, Vector3.left * 1000, Quaternion.Euler(Vector3.zero));
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

        // Make sure last item is initialized before continuing
        MapInitialize[] lastItemRef = loadedMap[loadedMap.Count - 1].GetComponents<MapInitialize>();
        bool doneInitializing;
        do
        {
            doneInitializing = true;

            foreach (MapInitialize m in lastItemRef)
            {
                if (!m.initialized)
                    doneInitializing = false;
            }

            yield return null;

        } while (!doneInitializing);


        Debug.Log("[MapLoader] Each component initialized");

        // === Sync up each room === //

        // Pair first item to the start point
        loadedMap[0].Sync(startingRoom.GetComponentInChildren<Door>().SyncPoint );
        // Pair all other consecutive rooms
        for(int i = 0; i < loadedMap.Count-1; i++)
        {
            loadedMap[i + 1].Sync(loadedMap[i].GetExit());
        }
        Debug.Log("[MapLoader] Sync finished!");


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
        Debug.Log("[MapLoader] Initial inactive set!");

        roomIndex = -1;

        navMesh.BuildNavMesh();

        currState = States.Start;

        yield return null;
    }

    #endregion

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
            if (!selectedObject.WithinDifficulty(roomIndex/2))
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

    #region Room Incremenation

    /// <summary>
    /// Deactivate passed sections and prepare the new ones
    /// </summary>
    public void UpdateLoadedSegments()
    {
        // Increment room index. Do by 2 because its accounting for room and hallway
        roomIndex += 2;

        if (loadedMap[roomIndex].segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Room)
        {
            currState = States.Room;
        }
        else if(loadedMap[roomIndex].segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Hallway)
        {
            currState = States.Hallway;
        }
        else if (loadedMap[roomIndex].segmentInfo.segmentType == MapSegmentSO.MapSegmentType.FinalRoom)
        {
            currState = States.Final;
        }

        // Deactivate the last 2 sections, if possible
        loadedMap[roomIndex - 1].DeactivateSegment();
        if (roomIndex - 2 >= 0)
            loadedMap[roomIndex - 2].DeactivateSegment();
        else
            startingRoom.SetActive(false);

        // Activate the next two sections, if possible
        if (roomIndex+1 < loadedMap.Count)
            loadedMap[roomIndex + 1].ActivateSegment();
        if (roomIndex + 2 < loadedMap.Count)
            loadedMap[roomIndex + 2].ActivateSegment();

        navMesh.BuildNavMesh();
    }

    public void StartRoomEncounter()
    {
        //Debug.Log($"Pew Bang! Encounter started for room at index {roomIndex +1}!");

        //loadedMap[roomIndex].GetComponent<SpawnManager>().BeginEncounter();
        SpawnManager.instance.BeginEncounter();
    }

    public void EndRoomEncounter()
    {
        //Debug.Log($"Phew! You won it all good jorb {roomIndex+1}!");
        loadedMap[roomIndex+1].GetComponent<DoorManager>().UnlockExit();
    }

    /// <summary>
    /// Return to the hub world and reset upgrades
    /// </summary>
    public void ReturnToHub()
    {
        instance = null;

        

        // TODO - RESET PLAYER UPGRADES
        Debug.Log($"PUM active : {PlayerUpgradeManager.instance != null} | AUM active : {AllUpgradeManager.instance != null}");

        if (PlayerUpgradeManager.instance != null)
            PlayerUpgradeManager.instance.DestroyPUM();
        if (AllUpgradeManager.instance != null)
            AllUpgradeManager.instance.DestroyAUM();

        GameManager.instance.ChangeState(GameManager.States.HUB);

        Destroy(gameObject);
    }


    #endregion

    #region Utility - Debug

    /// <summary>
    /// Continuely load the rooms. Debug only to help test in inspector
    /// </summary>
    /// <returns></returns>
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
    }

    #endregion
}
