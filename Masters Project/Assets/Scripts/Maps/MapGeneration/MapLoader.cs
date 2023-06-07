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
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum LoadState
{
    Loading,
    Done
}

public class MapLoader : MonoBehaviour
{
    /// <summary>
    /// Reference to this loader
    /// </summary>
    public static MapLoader instance;

    private LoadState loadState;

    public LoadState LoadState
    {
        get { return loadState; }
    }

    #region Initialization Variables 

    //[Tooltip("Hallways that can be used"), AssetsOnly]
    //[SerializeField] private List<MapSegmentSO> allHallways;
    [Tooltip("Rooms that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allRooms;

    [Tooltip("How many rooms to complete before loading ")]
    [SerializeField] private int maxFloorLength;

    //[Tooltip("Root of the starting room"), SceneObjectsOnly, Required]
    //[SerializeField] private GameObject startingRoom;

    [Tooltip("Final boss room"), AssetsOnly]
    [SerializeField] private MapSegmentSO finalRoom;

    private List<MapSegmentSO> availableRooms;
    //private List<MapSegmentSO> availableHalls;

    [Tooltip("The current order of maps to be played. Randomized at runtime")]
    [SerializeField, ReadOnly, HideInEditorMode] private List<MapSegmentSO> mapOrder;

    [Tooltip("Reference to the loading screen to use")]
    [SerializeField] GameObject loadingScreen;

    /// <summary>
    /// Navmesh for overall map
    /// </summary>
    private NavMeshSurface navMesh;

    /// <summary>
    /// Actions to be executed when an encounter is complete
    /// </summary>
    private UnityEvent onEncounterComplete;

    #endregion

    #region Big Map Variables

    /*
    /// <summary>
    /// All segments loaded into the scene, in order
    /// </summary>
    private List<SegmentLoader> loadedMap;

    /// <summary>
    /// Current index for looking through rooms
    /// </summary>
    [SerializeField] private int roomIndex;

    [SerializeField] private bool testShowAll;
    */

    #endregion

    #region Portal Map Variables 

    [Header("Portal Map Generation")]

    [Tooltip("Current depth of the portal system")]
    [SerializeField, ReadOnly] int portalDepth = -1;

    [Tooltip("Reference to the loader the starting room uses")]
    [SerializeField] RoomInitializer startingRoomInitializer;
    
    [SerializeField] MapSegmentSO preBossNeutralRoom;

    /// <summary>
    /// Current destination to load to
    /// </summary>
    private string dest = "";

    /// <summary>
    /// Refernce to all portals in the current scene
    /// </summary>
    private PortalTrigger[] portals;

    /// <summary>
    /// The current room manager
    /// </summary>
    private RoomInitializer currentRoom;

    #endregion

    #region Secret Room Variables

    [Tooltip("Range of room indexes that can be used for a secret interaction")]
    [SerializeField] Vector2Int randomSecretRoomRange;
    [Tooltip("The secret room index that was chosen")]
    [SerializeField, ReadOnly] int chosenSecretRoomIndex;
    [Tooltip("Pool of secret rooms to utilize")]
    [SerializeField] MapSegmentSO[] secretRoomPool;
    [Tooltip("The random secret room that was chosen")]
    [SerializeField, ReadOnly] MapSegmentSO chosenSecretRoom;

    #endregion

    #region Initialization Funcs

    /// <summary>
    /// Initialize pools
    /// </summary>
    private void Awake()
    {
        // Prepare instance
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }
        mapOrder = new List<MapSegmentSO>();
        // loadedMap = new List<SegmentLoader>();

        availableRooms = new List<MapSegmentSO>();
        // availableHalls = new List<MapSegmentSO>();

        navMesh = GetComponent<NavMeshSurface>();
        StartCoroutine(ArrangeMap());
    }

    /// <summary>
    /// Prepare the order of the run
    /// </summary>
    /// <returns></returns>
    private IEnumerator ArrangeMap()
    {
        loadState = LoadState.Loading;

        // Enable loading screen, wait to disable controls
        loadingScreen.SetActive(true);
        while (GameManager.controls == null)
            yield return null;
        GameControls controls = GameManager.controls;
        if (controls != null)
        {
            controls.Disable();
        }


        // === Choose what rooms to use === //
        int c = 0;
        int backup = 0;
        while (c < maxFloorLength)
        {
            // Add new room, iterate next step
            mapOrder.Add(SelectRoomNew());
            c++;

            backup++;
            if (backup > 1000)
            {
                Debug.LogError("MapLoader loop to build the room list is in an infinite loop!");
                Debug.Break();
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        // Choose which room to 'add' the secret room to
        chosenSecretRoomIndex = Random.Range(randomSecretRoomRange.x, randomSecretRoomRange.y);

        // Load in the final room, if there is one
        if (finalRoom != null)
        {
            mapOrder.Add(finalRoom);
        }

        // make sure player is set to not despawn between scenes 
        yield return new WaitUntil(() => PlayerTarget.p != null);
        DontDestroyOnLoad(PlayerTarget.p);

        // Debug.Log("[MAPLOADER] Map arrangement prepared, order can be viewed in inspector");
        startingRoomInitializer.Init();

        loadState = LoadState.Done;
        loadingScreen.SetActive(false);

        yield return new WaitForSecondsRealtime(0.5f);
        // Wait half a second before reenabling controls
        if (controls != null)
        {
            controls.Enable();
            controls.UI.Disable();
            controls.PlayerGameplay.Enable();
        }

        // tell stat manager that a run has begun
        GlobalStatsManager.data.runsAttempted++;
        if (CallManager.instance != null)
            CallManager.instance.IncrementRuns();

        yield return null;
    }

    /// <summary>
    /// Select a new room, auto managing containers. 
    /// Only difference is this doesn't account for hallways
    /// </summary>
    /// <returns>A new room to use</returns>
    private MapSegmentSO SelectRoomNew()
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
            if (option.WithinDifficulty(mapOrder.Count))
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
        if (allUsableRooms.Count <= 0)
        {
            Debug.LogError("[MapLoader] LOADING ERROR! Cannot find any rooms to build with!");
            Debug.Break();
            return null;
        }

        // Get the next room segment trying to ignore dupes
        MapSegmentSO selectedObject;
        // remove the previous room (if there is one) from the selection pool
        if (allUsableRooms.Count > 1 && mapOrder.Count > 0 && allUsableRooms.Contains(mapOrder[mapOrder.Count - 1]))
        {
            allUsableRooms.Remove(mapOrder[mapOrder.Count - 1]);
        }

        selectedObject = allUsableRooms[Random.Range(0, allUsableRooms.Count)];

        // Remove from current pool
        availableRooms.Remove(selectedObject);

        // Return new room
        return selectedObject;
    }

    #endregion

    #region Portal Transition Funcs

    /// <summary>
    /// Get current portal depth
    /// </summary>
    /// <returns>current portal depth, adjusted for top-level reading</returns>
    public int PortalDepth()
    {
        return portalDepth + 1;
    }

    /// <summary>
    /// Loadd to the next level
    /// </summary>
    public void NextMainPortal()
    {
        StartCoroutine(LoadRoomRoutine());
    }

    /// <summary>
    /// Load to a next room
    /// </summary>
    private IEnumerator LoadRoomRoutine()
    {
        loadingScreen.SetActive(true);

        onEncounterComplete?.RemoveAllListeners();

        // iterate to next step
        portalDepth++;

        // If out of rooms, then return to hub
        if (portalDepth >= mapOrder.Count)
        {
            Debug.Log("Depth matches end, returning to hub");
            ReturnToHub();
            yield break;
        }
        // Otherwise, get the next map and load to that scene. Wait for load to finish
        else
        {
            dest = mapOrder[portalDepth].sceneName;
            yield return GameManager.instance.LoadToScene(dest);
        }

        // Once loading finishes, teleport player to a spawnpoint
        MovePlayerToSpawn();

        // Initialize the room
        currentRoom = FindObjectOfType<RoomInitializer>();
        if (currentRoom == null)
        {
            Debug.LogError("[MAPLOADER] While loading to new map, no room initializer was found!");
        }
        else
        {
            currentRoom.Init();
        }

        // If at secret room depth, load that too
        if (portalDepth == chosenSecretRoomIndex)
        {
            currentRoom.ChooseRandomSecretProp();
            yield return StartCoroutine(LoadSecretRoom());

            //FindObjectOfType<SecretPortalInstance>(true).Init();
        }

        navMesh.BuildNavMesh();

        loadingScreen.SetActive(false);
    }

    public void ActivatePortal()
    {
        PortalTrigger p = currentRoom.GetExitPortal();

        if (p == null)
        {
            Debug.LogError("[MAPLOADER] Tried to select an exit portal, but none found!");
            Debug.Break();
            return;
        }

        p.SummonPortal();
    }

    /// <summary>
    /// Choose a spawnpoint to teleport the player to
    /// </summary>
    public void MovePlayerToSpawn()
    {
        Transform p = PlayerTarget.p.transform;

        GameObject[] positions = GameObject.FindGameObjectsWithTag("ExitPoint");

        Vector3 dest;
        Quaternion destRot;

        if (positions.Length <= 0)
        {
            Debug.LogError("[MAPLOADER] Tried to move player to spawn, but there are no spawn points set!");
            return;
        }
        else if (positions.Length == 1)
        {
            dest = positions[0].transform.position;
            destRot = positions[0].transform.rotation;
        }
        else
        {
            int i = Random.Range(0, positions.Length);
            dest = positions[i].transform.position;
            destRot = positions[i].transform.rotation;
        }

        p.position = dest;
        p.rotation = destRot;
    }

    #endregion

    #region Core Flow Funcs

    public void EndRoomEncounter()
    {
        Debug.Log("End room encounter called");

        //Debug.Log($"Phew! You won it all good jorb {roomIndex+1}!");
        //loadedMap[roomIndex+1].GetComponent<DoorManager>().UnlockExit();

        //if (mapOrder[roomIndex+1].segmentType == MapSegmentSO.MapSegmentType.Room
        //    && LinearSpawnManager.instance != null)
        //{
        //    WarningText.instance.Play("ANOMALY SUBSIDED, ROOM UNLOCKED", false);
        //    LinearSpawnManager.instance.IncrementDifficulty();
        //}

        WarningText.instance.Play("ANOMALY SUBSIDED, ROOM UNLOCKED", false);
        LinearSpawnManager.instance.IncrementDifficulty();

        onEncounterComplete?.Invoke();

        ActivatePortal();
    }

    /// <summary>
    /// Return to the hub world and reset run data 
    /// </summary>
    public void ReturnToHub()
    {
        GameManager.instance.GoToHub();
        ClearRunData();
    }

    /// <summary>
    /// Reset the current run data
    /// </summary>
    public void ClearRunData()
    {
        // Destroy crystal manager
        if (CrystalManager.instance != null)
            CrystalManager.instance.DestroyCM();

        // Move player out of 'dont destroy' scene so it clears properly on unload
        if (PlayerTarget.p != null)
            SceneManager.MoveGameObjectToScene(PlayerTarget.p.gameObject, SceneManager.GetActiveScene());

        instance = null;
        Destroy(gameObject);
    }

    /// <summary>
    /// Register an action to be performed on encounter complete
    /// </summary>
    /// <param name="a">New action to subscribe</param>
    public void RegisterOnEncounterComplete(UnityAction a)
    {
        if (onEncounterComplete == null)
            onEncounterComplete = new UnityEvent();

        onEncounterComplete.AddListener(a);
    }

    #endregion

    #region Old Large Map

    /*
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
            mapOrder.Add(SelectHallway());
            mapOrder.Add(finalRoom);
        }

        //Debug.Log("[MapLoader] Order Prepared");

        // === Spawn in each room, initialize whats required ===/

        foreach(MapSegmentSO segment in mapOrder)
        {
            // Create map segment, give its info to it, add to list
            GameObject t = Instantiate(segment.segmentPrefab, Vector3.left * 1000, Quaternion.Euler(Vector3.zero));
            yield return new WaitForEndOfFrame();
            t.GetComponent<SegmentLoader>().segmentInfo = segment;
            loadedMap.Add(t.GetComponent<SegmentLoader>());

            //t.SetActive(false);

            // Initialize each of its own components
            MapInitialize[] test = t.GetComponents<MapInitialize>();
            for(int i = 0; i < test.Length; i++)
            {
                StartCoroutine(test[i].InitializeComponent());
                yield return new WaitForEndOfFrame();
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


        //Debug.Log("[MapLoader] Each component initialized");

        // === Sync up each room === //

        // Pair first item to the start point
        Door d = startingRoom.GetComponentInChildren<Door>();
        loadedMap[0].Sync(d);
        // Pair all other consecutive rooms
        for(int i = 0; i < loadedMap.Count-1; i++)
        {
            loadedMap[i + 1].Sync(loadedMap[i].GetExit());
        }
        
        //Debug.Log("[MapLoader] Sync finished!");


        // === Set later rooms inactive === //

        roomIndex = -1;

        // Activate first room and hallway, deactivate rest
        for (int i = 0; i < 2; i++)
        {
            // double check range
            if(i < loadedMap.Count)
                yield return StartCoroutine(loadedMap[i].ActivateSegment());
        }
        if (!testShowAll)
        {
            for (int i = 2; i < loadedMap.Count; i++)
            {
                yield return StartCoroutine(loadedMap[i].DeactivateSegment(true));
            }
        }
        
        //Debug.Log("[MapLoader] Initial inactive set!");

        PrepareNavmesh();

        //currState = States.Start;
        loadState = LoadState.Done;

        loadingScreen.SetActive(false);

        yield return new WaitForSecondsRealtime(0.5f);
        // Wait half a second before reenabling controls
        if (controls != null)
        {
            controls.Enable();
            controls.UI.Disable();
            controls.PlayerGameplay.Enable();
        }

        // tell stat manager that a run has begun
        GlobalStatsManager.data.runsAttempted++;
        if (CallManager.instance != null)
            CallManager.instance.IncrementRuns();

        yield return null;
    }

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

    public int RoomDepth()
    {
        return (roomIndex+1)/2;
    }
    */

    #endregion

    #region Secret Room Funcs

    /// <summary>
    /// Get a random secret room to load
    /// </summary>
    /// <returns>Selected random room</returns>
    private MapSegmentSO GetRandomRoom()
    {
        if (secretRoomPool.Length == 0)
            return null;

        return secretRoomPool[ Random.Range(0, secretRoomPool.Length) ];
    }

    /// <summary>
    /// Load in the secret room additively
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSecretRoom()
    {
        // Try to get a random secret room
        chosenSecretRoom = GetRandomRoom();
        if(chosenSecretRoom == null)
        {
            Debug.Log("Tried to load a secret room, but no room found!");
            yield break;
        }

        // Load the secret room addatively so we can quickly teleport the player around
        AsyncOperation op = SceneManager.LoadSceneAsync(chosenSecretRoom.sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(()=> op.isDone);
    }

    #endregion

    private void OnDestroy()
    {
        instance = null;
    }
}
