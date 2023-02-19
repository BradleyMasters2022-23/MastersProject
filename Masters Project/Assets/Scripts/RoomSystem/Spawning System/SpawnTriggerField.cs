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

    /// <summary>
    /// All colliders used for this trigger
    /// </summary>
    private Collider[] triggerCol;
    #endregion

    #region Difficulty Variables
    [Header("===Difficulty===")]

    [Tooltip("All waves of relative difficulties to use for this encounter.")]
    [SerializeField] private RelativeDifficulty[] relativeDifficulty;
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
    
    private List<GameObject> spawnedEnemies;
    private Coroutine spawnRoutine;
    private Queue<EnemySO> spawnQueue;
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

        Init();
    }

    /// <summary>
    /// Initialize the spawn field. Called by map generator
    /// </summary>
    public void Init()
    {
        // Get collider date, make sure its set to trigger
        foreach (Collider col in triggerCol)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        // dont bother activating if theres no combat or triggers attached to this obj
        if (relativeDifficulty.Length > 0 && triggerCol.Length > 0)
            active = true;
    }

    /// <summary>
    /// Disable this spawn trigger
    /// </summary>
    public void Deactivate()
    {
        active = false;

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
            Deactivate();
            foreach (SpawnTriggerField otherTrigger in conflictingTriggers)
            {
                otherTrigger.Deactivate();
            }
            StartCoroutine(StartEncounter());
        }
        else if (!active)
        {
            // If not active and triggered, try disabling triggers again
            Deactivate();
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
        yield return new WaitForSeconds(activationDelay);

        SetWallStatus(true);

        // Request waves with current difficulty
        waves = LinearSpawnManager.instance.RequestBatch(relativeDifficulty, sizeModifier);

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

                // If spawnpoint can spawn, spawn the enemy
                if(point != null && point.CanSpawnLinear(spawnQueue.Peek()))
                {
                    point.SpawnNow(spawnQueue.Dequeue(), this);
                    yield return new WaitForSeconds(Random.Range(spawningDelayRange.x, spawningDelayRange.y));
                }

                yield return null;
            }

            // Wait a few seconds to let currently spawning enemies spawn and populate tracking queue
            // Use scale timer so freezing time doesn't interfere
            //ScaledTimer scaledDelay = new ScaledTimer(3f);
            //while (!scaledDelay.TimerDone())
            //    yield return null;

            // Check end loop - Repetedly check if all enemies are killed
            while(spawnedEnemies.Count > 0)
            {
                for(int j = spawnedEnemies.Count-1; j >= 0; j--)
                {
                    if (spawnedEnemies[j] == null)
                        spawnedEnemies.RemoveAt(j);
                }

                spawnedEnemies.TrimExcess();
                yield return null;
            }

            
        }

        // End encounter once done with all waves
        EndEncounter();

        yield return null;
    }

    public void EndEncounter()
    {
        Debug.Log("Encounter finished!");
        SetWallStatus(false);
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
