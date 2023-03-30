/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 15th, 2023
 * Last Edited - February 19th, 2023 by Ben Schuster
 * Description - A trigger field that sends data to the spawn manager on activation
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class SpawnTriggerField : MonoBehaviour
{
    #region Setup Variables
    [Header("===Setup===")]

    [Tooltip("Whether or not this field is currently active.")]
    [SerializeField, ReadOnly] private bool active;
    [Tooltip("Any fields that conflict with the encounters. Set this to any triggers meant to be used from the opposite direction.")]
    [SerializeField] private SpawnTriggerField[] conflictingTriggers;
    [Tooltip("All spawnpoints usable with this field. Can be shared with others.")]
    [SerializeField] private SpawnPoint[] spawnPoints;
    [Tooltip("Any walls that activate while this encounter starts and disabled when finished.")]
    [SerializeField] private GameObject[] blockerWalls;

    [SerializeField] private bool testing;

    /// <summary>
    /// All colliders used for this trigger
    /// </summary>
    private Collider[] triggerCol;
    #endregion

    #region Difficulty Variables
    [Header("===Difficulty===")]

    [Tooltip("All waves of relative difficulties to use for this encounter.")]
    [SerializeField] private EncounterDifficulty[] relativeDifficulty;
    [Tooltip("Modifier to the encounter size, intended based on size of the room the encouter is in. Multiplied to the batch list.")]
    [SerializeField] private float sizeModifier = 1;

    private Dictionary<EnemySO, int>[] waves;
    #endregion

    #region Spawning Variables
    [Header("===Spawning===")]

    [Tooltip("The initial delay between activating encounter and spawning enemies")]
    [SerializeField] private float activationDelay;
    [Tooltip("The delay between spawning each individiaul enemy. Randomly chosen between these bounds")]
    [SerializeField] private Vector2 spawningDelayRange;
    
    /// <summary>
    /// Collection of all spawned enemies
    /// </summary>
    private List<GameObject> spawnedEnemies;
    /// <summary>
    /// The current spawn routine
    /// </summary>
    private Coroutine spawnRoutine;
    /// <summary>
    /// Current queue of enemies being spawned
    /// </summary>
    private Queue<EnemySO> spawnQueue;
    /// <summary>
    /// Whether or not this spawn trigger is still active and spawning
    /// </summary>
    private bool finished;
    public bool Finished
    {
        get { return finished; }
    }
    #endregion

    #region Setup Functions

    /// <summary>
    /// Disable any data before anything begins to prevent potential activation bugs
    /// </summary>
    private void Awake()
    {
        triggerCol = GetComponents<Collider>();
        foreach (Collider col in triggerCol)
        {
            col.enabled = false;
        }
        active = false;

        if(testing)
            Init();
    }

    /// <summary>
    /// Initialize the spawn field. Called by map generator
    /// </summary>
    public void Init()
    {
        finished = false;

        // Get collider date, make sure its set to trigger
        foreach (Collider col in triggerCol)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        // dont bother activating if theres no combat or triggers attached to this obj
        if (relativeDifficulty.Length > 0 && triggerCol.Length > 0)
        {
            active = true;
        }
    }

    private void Start()
    {
        if (testing)
            LoadWaves();
    }

    /// <summary>
    /// Load in the waves in a way to reduce activation later 
    /// </summary>
    public void LoadWaves()
    {
        // Request waves with current difficulty
        waves = LinearSpawnManager.instance.RequestBatch(relativeDifficulty);
    }

    /// <summary>
    /// Disable this spawn trigger
    /// </summary>
    /// <param name="encounterFinished">Whether this field should be considered finished</param>
    public void Deactivate(bool encounterFinished)
    {
        finished = encounterFinished;

        if (triggerCol != null)
        {
            foreach (Collider col in triggerCol)
            {
                col.enabled = false;
            }
        }
    }

    /// <summary>
    /// On activation, begin the encounter
    /// </summary>
    /// <param name="other">Other object triggering this</param>
    private void OnTriggerEnter(Collider other)
    {
        if (active && other.CompareTag("Player"))
        {
            // Deactivate this field and any conflicting fields
            Deactivate(false);
            foreach (SpawnTriggerField otherTrigger in conflictingTriggers)
            {
                otherTrigger.Deactivate(true);
            }
            StartCoroutine(StartEncounter());
        }
        else if (!active)
        {
            // If not active and triggered, try disabling triggers again
            Deactivate(false);
        }
    }

    #endregion

    #region Spawning Functions

    /// <summary>
    /// Send the data to the spawn manager to begin the encounter
    /// TODO LATER - work in way to 'block player path'
    /// </summary>
    public IEnumerator StartEncounter()
    {
        // if no waves, exit
        if(waves == null || waves.Length <= 0)
        {
            finished = true;
            EndEncounter();
            yield break;
        }

        if (BackgroundMusicManager.instance != null)
            BackgroundMusicManager.instance.SetMusic(Music.Combat, 1f);

        finished = false;

        yield return new WaitForSeconds(activationDelay);

        SetWallStatus(true);

        // Handle spawning for each wave
        for(int i = 0; i < waves.Length; i++)
        {
            // Load in all of wave into spawn queue
            spawnQueue = new Queue<EnemySO>();
            foreach (var spawnDetails in waves[i])
            {
                for (int j = 0; j < spawnDetails.Value; j++)
                    spawnQueue.Enqueue(spawnDetails.Key);
            }

            spawnedEnemies = new List<GameObject>();
            // Spawning Loop - Repeatedly try loading in enemies until empty
            while (spawnQueue.Count > 0)
            {
                // choose spawnpoint
                SpawnPoint point = spawnPoints[Random.Range(0, spawnPoints.Length)];

                if (point != null && !point.isActiveAndEnabled)
                    continue;

                // If spawnpoint can spawn, spawn the enemy
                if(point != null && point.CanSpawnLinear(spawnQueue.Peek()))
                {
                    point.SpawnNow(spawnQueue.Dequeue(), this);
                    yield return new WaitForSeconds(Random.Range(spawningDelayRange.x, spawningDelayRange.y));
                }

                yield return null;
            }

            // wait for all enemies to be given a chance to fully spawn
            yield return new WaitForSeconds(3f);

            // Check end loop - Repetedly check if all enemies are killed
            while(spawnedEnemies.Count > 0)
            {
                for(int j = spawnedEnemies.Count-1; j >= 0; j--)
                {
                    if (spawnedEnemies[j] == null || !spawnedEnemies[j].activeInHierarchy)
                        spawnedEnemies.RemoveAt(j);
                }

                spawnedEnemies.TrimExcess();
                yield return null;
            }

            // Debug.Log("Wave Finished, moving to next");
        }

        // End encounter once done with all waves
        EndEncounter();

        yield return null;
    }

    /// <summary>
    /// End the current encounter
    /// </summary>
    public void EndEncounter()
    {
        //Debug.Log("Encounter finished!");
        SetWallStatus(false);
        finished = true;
    }

    public void EnqueueEnemy(GameObject enemyInstance)
    {
        spawnedEnemies.Add(enemyInstance);
    }

    /// <summary>
    /// Enable or disable any walls used to lock a player in
    /// </summary>
    /// <param name="enabled"></param>
    private void SetWallStatus(bool enabled)
    {
        foreach (GameObject wall in blockerWalls)
        { wall.SetActive(enabled); }
    }

    #endregion


    private void OnDisable()
    {
        // Incase its still active, disable the spawn routine
        if(spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        if(spawnedEnemies != null)
        {
            // Incase any are left, kill all spawned enemies
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                Destroy(spawnedEnemies[i]);
            }
        }
    }
}
