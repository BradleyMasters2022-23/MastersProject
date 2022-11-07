using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

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

    /// <summary>
    /// Current collection of hallways that can be pulled from
    /// </summary>
    private List<MapSegmentSO> remainingHallways;
    /// <summary>
    /// Current collection of rooms that can be pulled from
    /// </summary>
    private List<MapSegmentSO> remainingRooms;


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

    private List<GameObject> pooledRooms;
    private List<GameObject> pooledHallways;

    public static MapLoader instance;



    #endregion




    private void Awake()
    {
        // Create backup for each container 
        remainingHallways = new List<MapSegmentSO>(allHallways);
        remainingRooms = new List<MapSegmentSO>(allRooms);

        // Load the entrance into the loaded queue
        loadedRooms = new Queue<GameObject>();
        loadedRooms.Enqueue(startingSpot);
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
        // If out of room, repopulate the pool
        if (remainingRooms.Count <= 0)
        {
            remainingRooms = new List<MapSegmentSO>(allRooms);
        }

        MapSegmentSO selected;

        do
        {
            // select a new room
            selected = remainingRooms[Random.Range(0, remainingRooms.Count)];

            // Verify it is a hallway, otherwise remove from list and try again
            if (selected.segmentType != MapSegmentSO.MapSegmentType.Room)
            {
                Debug.LogError($"[Map Loader] A non-room named {selected.name} " +
                    $"was placed into room! Please remove from room list!");

                allRooms.Remove(selected);
                remainingRooms.Remove(selected);
            }
            
            if(!selected.WithinDifficulty(currentFloorLength))
            {
                remainingRooms.Remove(selected);
            }

            // Repopulate incase its out
            if (remainingRooms.Count <= 0)
            {
                remainingRooms = new List<MapSegmentSO>(allRooms);
            }

        } while (selected.segmentType != MapSegmentSO.MapSegmentType.Room);

        // Remove from pool to ensure it doesnt appear again
        remainingRooms.Remove(selected);

        // Return new room
        return selected.segmentPrefab;
    }

    /// <summary>
    /// Select a hallway, auto manages hallway container
    /// </summary>
    /// <returns>Selected hallway prefab</returns>
    private GameObject SelectHallway()
    {
        // If out of hallways, repopulate the pool
        if (remainingHallways.Count <= 0)
        {
            remainingHallways = new List<MapSegmentSO>(allHallways);
        }

        // Select a random hallway, ensure its valid
        MapSegmentSO selected;
        do
        {
            // select a new room
            selected = remainingHallways[Random.Range(0, remainingHallways.Count)];

            // Verify it is a hallway, otherwise remove from list and try again
            if (selected.segmentType != MapSegmentSO.MapSegmentType.Hallway)
            {
                Debug.LogError($"[Map Loader] A non-hallway named {selected.name} " +
                    $"was placed into hallway! Please remove from hallway list!");

                allHallways.Remove(selected);
                remainingHallways.Remove(selected);

                // Repopulate incase its out
                if (remainingHallways.Count <= 0)
                {
                    remainingHallways = new List<MapSegmentSO>(allHallways);
                }
            }

        } while (selected.segmentType != MapSegmentSO.MapSegmentType.Hallway);

        // Remove from pool to ensure it doesnt appear again
        remainingHallways.Remove(selected);

        // Return new hallway
        return selected.segmentPrefab;
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

        loadRoom = !loadRoom;

        GameObject section = Instantiate(nextSection, new Vector3(500, 500, 500), Quaternion.identity);
        loadedRooms.Enqueue(section);

        SegmentLoader sectionManager = section.GetComponent<SegmentLoader>();


        sectionManager.PrepareStartPoint(nextSpawnPoint);

        nextSpawnPoint = sectionManager.NextExit();
    }

    /// <summary>
    /// Unload the oldest item in the load queue
    /// </summary>
    private void UnloadSegment()
    {
        Destroy(loadedRooms.Dequeue());
    }

    private IEnumerator ContinueLoad()
    {
        while(currentFloorLength <= maxFloorLength)
        {
            LoadNextComponent();

            //if (currentFloorLength >= 2)
            //    UnloadSegment();

            yield return new WaitForSecondsRealtime(1f);

            yield return null;
        }
    }
}
