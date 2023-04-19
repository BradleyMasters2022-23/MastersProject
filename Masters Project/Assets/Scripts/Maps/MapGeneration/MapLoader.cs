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
using UnityEngine.InputSystem.XR;
using UnityEditor.ShaderGraph;

public enum LoadState
{
    Loading,
    Done
}

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
    //private States currState;

    private LoadState loadState;

    public LoadState LoadState
    {
        get { return loadState; }
    }

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
    [SerializeField] private int roomIndex;

    [SerializeField] private bool testShowAll;

    #endregion

    #region Loading Stuff

    [SerializeField] GameObject loadingScreen;

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
        // Enable loading screen, wait to disable controls

        loadingScreen.SetActive(true);

        while (GameManager.controls == null)
            yield return null;
        GameControls controls = GameManager.controls;
        if (controls != null)
        {
            controls.Disable();
        }
            

        loadState = LoadState.Loading;

        // === Choose what rooms to use === //
        int c = 0;
        int backup = 0;
        while(c < maxFloorLength)
        {
            // Add new segment, check if a room was added. If so, add
            mapOrder.Add(ChooseNextSegment());
            if (mapOrder[mapOrder.Count - 1].segmentType == MapSegmentSO.MapSegmentType.Room)
                c++;

            backup++;
            if(backup > 1000)
            {
                Debug.LogError("MapLoader loop to build the room list is in an infinite loop!");
                Debug.Break();
                break;
            }
        }

        // Load in the final room, if there is one. Add hallway buffer
        if(finalRoom != null)
        {
            //mapOrder.Add(SelectHallway());
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

        // Make sure last item is finished initialized before continuing
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
        Door d = startingRoom.GetComponentInChildren<Door>();
        loadedMap[0].Sync(d);
        // Pair all other consecutive rooms
        for(int i = 0; i < loadedMap.Count-1; i++)
        {
            loadedMap[i + 1].Sync(loadedMap[i].GetExit());
        }
        Debug.Log("[MapLoader] Sync finished!");


        // === Set later rooms inactive === //

        roomIndex = -1;

        // Activate first room and hallway, deactivate rest
        for (int i = 0; i < 2; i++)
        {
            yield return StartCoroutine(loadedMap[i].ActivateSegment());
        }
        if (!testShowAll)
        {
            for (int i = 2; i < loadedMap.Count; i++)
            {
                yield return StartCoroutine(loadedMap[i].DeactivateSegment(true));
            }
        }
        
        Debug.Log("[MapLoader] Initial inactive set!");

        PrepareNavmesh();

        //currState = States.Start;
        loadState = LoadState.Done;

        loadingScreen.SetActive(false);

        yield return new WaitForSecondsRealtime(0.5f);
        // Wait half a second before reenabling controls
        if (controls != null)
            controls.Enable();

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
        // If out of rooms, repopulate the pool
        if (availableRooms.Count <= 0)
        {
            availableRooms = new List<MapSegmentSO>(allRooms);
        }

        // populate all potential rooms allowed to be used here
        List<MapSegmentSO> allUsableRooms = new List<MapSegmentSO>();
        foreach (MapSegmentSO option in availableRooms)
        {
            if(option.WithinDifficulty(mapOrder.Count / 2))
            {
                allUsableRooms.Add(option);
            }
        }
        // if no usable rooms available after first attempt, allow all rooms ignoring difficulty
        if (allUsableRooms.Count <= 0)
        {
            foreach (MapSegmentSO option in availableRooms)
            {
                allUsableRooms.Add(option);
            }
        }
        // if still empty, just stop the play session because something broke or no rooms passed in
        if(allUsableRooms.Count <= 0)
        {
            Debug.LogError("[MapLoader] LOADING ERROR! Cannot find any rooms to build with!");
            Debug.Break();
            return null;
        }

        MapSegmentSO selectedObject;

        // loop through trying to select a new room that ideally isnt the last room
        int c = 0;
        do
        {
            // select a new room
            selectedObject = allUsableRooms[Random.Range(0, allUsableRooms.Count)];

            // Make sure the last room is not the same
            // c variable can prevent this incase something breaks
            if(allUsableRooms.Count > 1 && 
                (mapOrder.Count >= 2 && selectedObject == mapOrder[mapOrder.Count - 2])
                && c < 100)
            {
                allUsableRooms.Remove(selectedObject);
                selectedObject = null;
                continue;
            }

            // Backup failsafe check
            c++;
            if (c >= 1000)
            {
                Debug.LogError($"[Map Loader] Select room 2 is stuck in a loop! Breaking!");
                Debug.DebugBreak();
                return null;
            }

        } while (selectedObject == null);

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
        // If out of rooms, repopulate the pool
        if (availableHalls.Count <= 0)
        {
            availableHalls = new List<MapSegmentSO>(allHallways);
        }

        List<MapSegmentSO> allUsableHallway = new List<MapSegmentSO>(availableHalls);
        if(allUsableHallway.Count <= 0)
        {
            Debug.LogError("[MapLoader] LOADING ERROR! Cannot find any hallways to build with!");
            Debug.Break();
            return null;
        }


        // Select a random hallway, ensure its valid
        MapSegmentSO selectedObject;

        int c = 0;
        do
        {
            // select a new hallway
            selectedObject = allUsableHallway[Random.Range(0, allUsableHallway.Count)];

            // Try to prevent dupes if there more than one option available
            if (allUsableHallway.Count > 1 &&
                (mapOrder.Count >= 2 && selectedObject == mapOrder[mapOrder.Count - 2])
                && c < 100)
            {
                allUsableHallway.Remove(selectedObject);
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

        } while (selectedObject == null);

        // Remove from pool to ensure it doesnt appear again
        availableHalls.Remove(selectedObject);

        // Return new hallway
        return selectedObject;
    }

    #endregion

    #region Room Incremenation

    public void UpdateLoadedSegments()
    {
        loadState = LoadState.Loading;

        StartCoroutine(LoadSegments());
    }

    /// <summary>
    /// Deactivate passed sections and prepare the new ones
    /// </summary>
    public IEnumerator LoadSegments()
    {
        // Increment room index. Do by 2 because its accounting for room and hallway
        roomIndex += 2;

        // Deactivate the last 2 sections, if possible
        yield return StartCoroutine(loadedMap[roomIndex - 1].DeactivateSegment(false));

        if (roomIndex - 2 >= 0)
        {
            yield return StartCoroutine(loadedMap[roomIndex - 2].DeactivateSegment(false));
        }
        else
            startingRoom.SetActive(false);

        // Activate the next two sections, if possible
        if (roomIndex+1 < loadedMap.Count)
        {
            yield return StartCoroutine((loadedMap[roomIndex + 1].ActivateSegment()));
        }
        if (roomIndex + 2 < loadedMap.Count)
        {
            yield return StartCoroutine(loadedMap[roomIndex + 2].ActivateSegment());
        }

        PrepareNavmesh();

        loadState = LoadState.Done;
        yield return null;
    }

    private void PrepareNavmesh()
    {
        navMesh.BuildNavMesh();
    }

    public void StartRoomEncounter()
    {
        loadedMap[roomIndex+1].StartSegment();
    }

    public void EndRoomEncounter()
    {
        

        //Debug.Log($"Phew! You won it all good jorb {roomIndex+1}!");
        loadedMap[roomIndex+1].GetComponent<DoorManager>().UnlockExit();

        if (mapOrder[roomIndex+1].segmentType == MapSegmentSO.MapSegmentType.Room
            && LinearSpawnManager.instance != null)
        {
            WarningText.instance.Play("ANOMALY SUBSIDED, ROOM UNLOCKED", false);
            LinearSpawnManager.instance.IncrementDifficulty();
        }
            

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


    public int RoomDepth()
    {
        return (roomIndex+1)/2;
    }

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
