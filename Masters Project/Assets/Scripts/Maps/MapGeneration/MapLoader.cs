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

    [Tooltip("Rooms that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allRooms;

    [Tooltip("How many rooms to complete before loading ")]
    [SerializeField] private int maxFloorLength;

    [Tooltip("Final boss room"), AssetsOnly]
    [SerializeField] private MapSegmentSO finalRoom;

    /// <summary>
    /// List of rooms possible to be pulled from
    /// </summary>
    private List<MapSegmentSO> availableRooms;

    [Tooltip("The current order of maps to be played. Randomized at runtime")]
    [SerializeField, ReadOnly, HideInEditorMode] private List<MapSegmentSO> mapOrder;

    [Tooltip("Reference to the loading screen to use")]
    [SerializeField] GameObject loadingScreen;

    [SerializeField] bool enableControlsOnFinish = true;
    [Tooltip("Channel called when any scene change happens. Used to tell poolers to reset.")]
    [SerializeField] ChannelVoid onSceneChange;

    [Tooltip("Hub world portal probe for the final room")]
    [SerializeField] Cubemap hubworldReflectionMap;
    [SerializeField] float hubworldReflectionProbeIntensity;
    /// <summary>
    /// Navmesh for overall map
    /// </summary>
    private NavMeshSurface navMesh;

    /// <summary>
    /// Actions to be executed when an encounter is complete
    /// </summary>
    private UnityEvent onEncounterComplete;

    [SerializeField] private AudioSource source;

    #endregion

    #region Portal Map Variables 

    [Header("Portal Map Generation")]

    [Tooltip("Current depth of the portal system")]
    [SerializeField, ReadOnly] int portalDepth = -1;

    [Tooltip("Reference to the loader the starting room uses")]
    [SerializeField] RoomInitializer startingRoomInitializer;
    
    [SerializeField] AudioClipSO normalPortalUseSFX;

    /// <summary>
    /// Current destination to load to
    /// </summary>
    private string dest = "";

    /// <summary>
    /// The current room manager
    /// </summary>
    private RoomInitializer currentRoom;

    #endregion

    #region Secret Room Variables

    [Tooltip("Range of room indexes that can be used for a secret interaction")]
    [SerializeField] List<int> secretRoomOptions;
    [Tooltip("Number of secret rooms to utilize")]
    [SerializeField] int numberOfSecretRooms;
    [Tooltip("Pool of secret rooms to utilize")]
    [SerializeField] MapSegmentSO[] secretRoomPool;
    [Tooltip("The random secret room that was chosen")]
    [SerializeField, ReadOnly] MapSegmentSO chosenSecretRoom;
    [SerializeField] AudioClipSO secretPortalActivateSFX;
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
        normalPortalUseSFX.PlayClip(source);

        // Enable loading screen, wait to disable controls
        loadingScreen.SetActive(true);
        while (GameManager.controls == null)
            yield return null;
        GameControls controls = GameManager.controls;
        InputManager.ControlScheme scheme = InputManager.CurrControlScheme;
        if (controls != null)
        {
            controls.Disable();
        }

        currentRoom = startingRoomInitializer;

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

        // Randomize which secret room index are allowed to stay.
        while(secretRoomOptions.Count > numberOfSecretRooms)
        {
            int idx = Random.Range(0, secretRoomOptions.Count);
            secretRoomOptions.RemoveAt(idx);
            yield return null;
        }

        // Load in the final room, if there is one
        if (finalRoom != null)
        {
            mapOrder.Add(finalRoom);
        }

        // make sure player is set to not despawn between scenes 
        yield return new WaitUntil(() => PlayerTarget.p != null);
        DontDestroyOnLoad(PlayerTarget.p);

        // Debug.Log("[MAPLOADER] Map arrangement prepared, order can be viewed in inspector");
        startingRoomInitializer.Init(mapOrder[0].portalViewMat, mapOrder[0].probeIntensityLevel);

        loadState = LoadState.Done;
        loadingScreen.SetActive(false);

        yield return new WaitForSecondsRealtime(0.5f);
        // Wait half a second before reenabling controls
        if (controls != null && enableControlsOnFinish)
        {
            controls.PlayerGameplay.Enable();
        }
        InputManager.Instance.SetDirectControlscheme(scheme);


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
        // if not already loading, load
        if(loadState != LoadState.Loading)
            StartCoroutine(LoadRoomRoutine());
    }

    /// <summary>
    /// Load to a next room
    /// </summary>
    private IEnumerator LoadRoomRoutine()
    {
        loadState = LoadState.Loading;
        normalPortalUseSFX.PlayClip(source);

        onSceneChange?.RaiseEvent();

        // disable to prevent any new inputs
        GameManager.controls.Disable();

        loadingScreen.SetActive(true);

        onEncounterComplete?.RemoveAllListeners();

        // Debug.Log("Load room routine called");

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

        // Once loading finishes, teleport player to a spawnpoint. Wait a frame
        MovePlayerToSpawn();
        yield return new WaitForEndOfFrame();

        // Initialize the room
        currentRoom = FindObjectOfType<RoomInitializer>();
        if (currentRoom == null)
        {
            Debug.LogError("[MAPLOADER] While loading to new map, no room initializer was found!");
        }
        else
        {
            Cubemap nextRoomMap;
            float intensity = 1;
            int nextRoom = portalDepth + 1;
            if (nextRoom >= mapOrder.Count)
            {
                nextRoomMap = hubworldReflectionMap;
                intensity = hubworldReflectionProbeIntensity;
            }
            else
            {
                nextRoomMap = mapOrder[nextRoom].portalViewMat;
                intensity = mapOrder[nextRoom].probeIntensityLevel;
            }

            currentRoom.Init(nextRoomMap, intensity);
        }

        // If at secret room depth, load that too
        if (secretRoomOptions.Contains(portalDepth))
        {
            currentRoom.ChooseRandomSecretProp();
            yield return StartCoroutine(LoadSecretRoom());
        }

        navMesh.BuildNavMesh();
        loadingScreen.SetActive(false);
        GameManager.controls.PlayerGameplay.Enable();

        loadState = LoadState.Done;
    }

    public void ActivatePortal()
    {
        PortalTrigger p = currentRoom.GetExitPortal();

        if (p == null)
        {
            Debug.LogError("[MAPLOADER] Tried to select an exit portal, but none found!");
            return;
        }
        else
        {
            p.SummonPortal();
        }
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
    /// <summary>
    /// Play the audio for secret portal activate
    /// </summary>
    public void PlaySecretPortalSFX()
    {
        secretPortalActivateSFX.PlayClip(source);
    }

    #endregion

    #region Core Flow Funcs

    public void EndRoomEncounter()
    {
        WarningText.instance.Play("ANOMALY SUBSIDED, PORTAL DETECTED", false);
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

    #region Secret Room Funcs

    /// <summary>
    /// Get a random secret room to load
    /// </summary>
    /// <returns>Selected random room</returns>
    private MapSegmentSO GetRandomSecretRoom()
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
        chosenSecretRoom = GetRandomSecretRoom();
        if(chosenSecretRoom == null)
        {
            Debug.Log("Tried to load a secret room, but no room found!");
            yield break;
        }

        // Load the secret room addatively so we can quickly teleport the player around
        AsyncOperation op = SceneManager.LoadSceneAsync(chosenSecretRoom.sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(()=> op.isDone);
    }

    /// <summary>
    /// Get the data for the current secret room. Can return nul if none.
    /// </summary>
    /// <returns></returns>
    public MapSegmentSO GetCurrentSecretRoom()
    {
        return chosenSecretRoom;
    }

    /// <summary>
    /// Get map segment data based on current room
    /// </summary>
    /// <param name="indexOffset">Index offset from the current room.
    /// EX. setting to 1 will return the next room if possible</param>
    /// <returns>Relevant map room data</returns>
    public MapSegmentSO GetRoomData(int indexOffset = 0)
    {
        int newIdx = Mathf.Clamp(portalDepth+indexOffset, 0, mapOrder.Count);
        return mapOrder[newIdx];
    }

    #endregion

    private void OnDestroy()
    {
        instance = null;
    }
}
