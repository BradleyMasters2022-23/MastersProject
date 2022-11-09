using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.AI;
public class MapLoader : MonoBehaviour
{
    public enum States
    {
        Preparing,
        Room, 
        Hallway
    }


    [Tooltip("Hallways that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allHallways;
    [Tooltip("Rooms that can be used"), AssetsOnly]
    [SerializeField] private List<MapSegmentSO> allRooms;

    public int currentFloorLength;
    public int maxFloorLength;

    [Tooltip("Starting room for the player"), SceneObjectsOnly, Required]
    [SerializeField] private GameObject startingSpot;

    [Tooltip("Final boss room"), AssetsOnly]
    [SerializeField] private GameObject finalRoom;

    [Tooltip("Final hallway room"), AssetsOnly]
    [SerializeField] private GameObject finalSpot;

    public Transform nextSpawnPoint;

    public bool loadRoom;

    /// <summary>
    /// Collection of rooms being loaded in.
    /// </summary>
    private Queue<GameObject> loadedRooms;


    #region Object Pooler

    /// <summary>
    /// Track all pooled rooms that have not been used yet
    /// </summary>
    [SerializeField] private List<GameObject> standbyRooms;
    /// <summary>
    /// Track all pooled hallways that have not been used yet
    /// </summary>
    [SerializeField] private List<GameObject> standbyHallways;

    /// <summary>
    /// Track all pooled rooms that have recently been used
    /// </summary>
    [SerializeField] private List<GameObject> usedRooms;
    /// <summary>
    /// Track all pooled hallways that have recently been used
    /// </summary>
    [SerializeField] private List<GameObject> usedHallways;

    /// <summary>
    /// The current room being used
    /// </summary>
    private GameObject currentRoom;
    /// <summary>
    /// The current hallway being used
    /// </summary>
    private GameObject currentHallway;


    /// <summary>
    /// Reference to this loader
    /// </summary>
    public static MapLoader instance;

    /// <summary>
    /// Navmesh for overall map
    /// </summary>
    private NavMeshSurface navMesh;

    #endregion




    private void Awake()
    {
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
            room.GetComponent<SegmentLoader>().SetSO(allRooms[i]);
            standbyRooms.Add(room);
        }

        // Populate hallways, 3 per
        for(int i = 0; i < allHallways.Count; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject hallway = Instantiate(allHallways[i].segmentPrefab);
                hallway.GetComponent<SegmentLoader>().SetSO(allHallways[i]);
                standbyHallways.Add(hallway);
            }
        }

        // Load the entrance into the loaded queue
        loadedRooms = new Queue<GameObject>();
        loadedRooms.Enqueue(startingSpot);

        navMesh = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        StartCoroutine(ContinueLoad());

    }

    

    /// <summary>
    /// Select a room, auto manages room container
    /// </summary>
    /// <returns>Selected room prefab</returns>
    private GameObject SelectRoom()
    {
        GameObject selectedObject;
        SegmentLoader selectedLoader;

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
            selectedLoader = selectedObject.GetComponent<SegmentLoader>();

            // Verify it is a hallway, otherwise remove from list and try again
            if (selectedLoader.segmentInfo.segmentType != MapSegmentSO.MapSegmentType.Room)
            {
                Debug.LogError($"[Map Loader] A non-room named {selectedObject.name} " +
                    $"was placed into room! Please remove from room list!");

                // Remove from pool permenately 
                standbyRooms.Remove(selectedObject);
            }
            
            // If out of difficulty, remove from pool and send to used 
            if(!selectedLoader.segmentInfo.WithinDifficulty(currentFloorLength))
            {
                standbyRooms.Remove(selectedObject);
                usedRooms.Add(selectedObject);
            }


        } while (selectedLoader.segmentInfo.segmentType != MapSegmentSO.MapSegmentType.Room);

        // Remove from current pool

        currentRoom = selectedObject;
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
        SegmentLoader selectedLoader;

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
            selectedLoader = selectedObject.GetComponent<SegmentLoader>();

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

    /// <summary>
    /// Load in the next section
    /// </summary>
    public void LoadNextComponent()
    {
        GameObject nextSection;

        if (loadRoom)
        {
            nextSection = SelectRoom();
            currentFloorLength++;
        }
        else
        {
            nextSection = SelectHallway();
        }

        Debug.Log("Loading room " + nextSection.name);

        loadRoom = !loadRoom;

        //GameObject section = Instantiate(nextSection, new Vector3(500, 500, 500), Quaternion.identity);
        loadedRooms.Enqueue(nextSection);

        SegmentLoader sectionManager = nextSection.GetComponent<SegmentLoader>();



        sectionManager.RetrieveFromPool(nextSpawnPoint);

        navMesh.BuildNavMesh();

        nextSpawnPoint = sectionManager.NextExit();
    }

    /// <summary>
    /// Unload the oldest item in the load queue
    /// </summary>
    private void UnloadSegment()
    {
        SegmentLoader t;

        if(loadedRooms.Peek().TryGetComponent<SegmentLoader>(out t))
        {
            loadedRooms.Dequeue();
            t.ResetToPool();


            if(t.segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Hallway)
            {
                usedHallways.Add(t.gameObject);

            }
            else if(t.segmentInfo.segmentType == MapSegmentSO.MapSegmentType.Room)
            {
                usedRooms.Add(t.gameObject);

            }
        }
        else
        {
            Destroy(loadedRooms.Dequeue());
        }


        //loadedRooms.Dequeue().GetComponent<SegmentLoader>().ResetToPool();
    }

    private IEnumerator ContinueLoad()
    {
        while(currentFloorLength <= maxFloorLength)
        {
            LoadNextComponent();

            if (currentFloorLength >= 1)
                UnloadSegment();

            yield return new WaitForSecondsRealtime(2f);

            yield return null;
        }
    }
    
}
