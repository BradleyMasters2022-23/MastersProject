/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Core manager for the spawner
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [Tooltip("How many seconds to wait before starting the encounter?")]
    [SerializeField] private float startDelay;

    /// <summary>
    /// The encounter that was chosen
    /// </summary>
    private EncounterSO chosenEncounter;
    /// <summary>
    /// Current index of waves in the encounter
    /// </summary>
    private int waveIndex;

    /// <summary>
    /// The queue of enemies waiting to be spawned
    /// </summary>
    private Queue<GameObject> spawnQueue;

    [Tooltip("Maximum amount of enemies allowed to be spawned at once")]
    [SerializeField, Range(1, 50)] private int maxEnemies = 10;

    /// <summary>
    /// Current number of enemies spawned
    /// </summary>
    private int enemyCount = 0;
    /// <summary>
    /// Current number of enemies waiting in spawners not yet spawned
    /// </summary>
    private int waitingEnemies = 0;

    [Tooltip("Delay between spawning each individual enemy. Randomly chosen between this range.")]
    [SerializeField] private Vector2 spawnDelay = new Vector2(0, 1);

    /// <summary>
    /// Whether or not the spawner is currently spawning
    /// </summary>
    private bool spawning;

    /// <summary>
    /// Timer for spawning
    /// </summary>
    private ScaledTimer spawnDelayTimer;
    /// <summary>
    /// Timer for spawning
    /// </summary>
    private ScaledTimer startDelayTimer;
    /// <summary>
    /// Track the current spawn routine
    /// </summary>
    private Coroutine spawnRoutine;
    /// <summary>
    /// Track the current backup checking routine
    /// </summary>
    private Coroutine backupCheckRoutine;

    /// <summary>
    /// audio source for spawner
    /// </summary>
    private AudioSource s;

    [Tooltip("Sound played when the encounter ends.")]
    [SerializeField] AudioClip sound;

    /// <summary>
    /// References to every spawnpoint in the scene
    /// </summary>
    private SpawnPoint[] spawnPoints;

    /// <summary>
    /// Whether or not this spawner is complete
    /// </summary>
    private bool finished;


    private GameControls controls;
    private InputAction endCheat;

    /// <summary>
    /// Initialize singleton, internal trackers
    /// </summary>
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
        
        finished = true;
        
        s = gameObject.AddComponent<AudioSource>();
        spawnQueue = new Queue<GameObject>();

        spawnDelayTimer = new ScaledTimer(spawnDelay.x);
        startDelayTimer = new ScaledTimer(startDelay);
    }

    private void Start()
    {
        controls = GameManager.controls;
        endCheat = controls.PlayerGameplay.ClearEncounter;
        endCheat.performed += CheatReset;
        endCheat.Enable();
    }

    /// <summary>
    /// Pass in data needed to begin an encounter
    /// </summary>
    /// <param name="encounterData">the encounter data with enemies to spawn</param>
    /// <param name="spawnData">the spawnpoints to be used</param>
    public void PrepareEncounter(EncounterSO encounterData, SpawnPoint[] spawnData)
    {
        chosenEncounter = encounterData;
        spawnPoints = spawnData;
    }

    /// <summary>
    /// Clear the manager of it previous data
    /// </summary>
    private void ClearManager()
    {
        chosenEncounter = null;
        spawnPoints = null;
    }

    /// <summary>
    /// Start the encounter, if possible
    /// </summary>
    public void BeginEncounter()
    {

        if (chosenEncounter == null || spawnPoints.Length <= 0)
        {
            Debug.Log("No encounter or not enough spawnpoints! Unlocking room!");
            CompleteEncounter();
            return;
        }

        waveIndex = 0;
        enemyCount = 0;
        waitingEnemies = 0;
        finished = false;

        startDelayTimer.ResetTimer();
        spawnRoutine = StartCoroutine(SpawnNextWave());
    }


    /// <summary>
    /// Spawn the next wave
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnNextWave()
    {
        PrepareWaveQueue();

        // Wait for the initial delay to begin spawning
        while (!startDelayTimer.TimerDone())
            yield return null;

        // set to spawning state [TODO - proper state system]
        spawning = true;

        // Continually decide spawnpoints to use
        SpawnPoint _spawnPoint;
        while (spawnQueue.Count > 0 || waitingEnemies > 0 || !SpawnpointsEmpty())
        {

            // Only spawn if not past max
            if ((enemyCount + waitingEnemies) < maxEnemies)
            {
                // Spawn a new enemy with a randomized delay between the range, not choosing same point twice back to back
                do
                {
                    _spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                    yield return null;

                } while (spawnQueue.Count != 0 && !_spawnPoint.Open(spawnQueue.Peek()));

                // Spawn enemy after double checking count
                if (spawnQueue.Count != 0)
                    SpawnEnemy(spawnQueue.Dequeue(), _spawnPoint);
            }

            // Delay spawning between enemies between range
            spawnDelayTimer.ResetTimer(Random.Range(spawnDelay.x, spawnDelay.y));
            while (!spawnDelayTimer.TimerDone())
            {
                yield return null;
            }

            yield return null;
        }
        // End spawning, begin backup check routine
        spawning = false;
        backupCheckRoutine = StartCoroutine(CheckCount());
    }

    /// <summary>
    /// Prepare the wave queue by populating the spawn queue
    /// </summary>
    private void PrepareWaveQueue()
    {
        // load in numbers needed
        List<int> enemyCounts = new List<int>();
        int total = 0;
        foreach (EnemyGroup enemyGroup in chosenEncounter.waves[waveIndex].enemyGroups)
        {
            enemyCounts.Add(enemyGroup.amount);
            total += enemyGroup.amount;
        }

        // Load the specified number of each at random
        for (int i = 0; i < total; i++)
        {
            // Choose a random type, not allowing ones whos quantity has already been filled
            int typeIndex;
            do
            {
                typeIndex = Random.Range(0, enemyCounts.Count);
            } while (enemyCounts[typeIndex] <= 0);

            // Decrement that type's count
            enemyCounts[typeIndex]--;

            // Add to spawn queue
            GameObject enemyToSpawn = chosenEncounter.waves[waveIndex].enemyGroups[typeIndex].enemy;
            spawnQueue.Enqueue(enemyToSpawn);
        }
    }


    /// <summary>
    /// Send an enemy into the spawnpoint
    /// </summary>
    /// <param name="enemyPrefab">enemy to spawn</param>
    /// <param name="spawnPoint">spawnpoint to use</param>
    private void SpawnEnemy(GameObject enemyPrefab, SpawnPoint spawnPoint)
    {
        spawnPoint.LoadSpawn(enemyPrefab);
        waitingEnemies++;
    }


    /// <summary>
    /// Determine whether to start the next wave or complete the room
    /// </summary>
    private void WaveFinished()
    {
        if (finished)
            return;

        Debug.Log("Wave Finished");
        spawnRoutine = null;
        waveIndex++;
        startDelayTimer.ResetTimer();

        if (waveIndex >= chosenEncounter.waves.Length)
        {
            CompleteEncounter();
            spawnQueue.Clear();
        }
        else
        {
            spawnRoutine = StartCoroutine(SpawnNextWave());
        }
    }

    /// <summary>
    /// Complete the encounter
    /// </summary>
    private void CompleteEncounter()
    {
        finished = true;
        Debug.Log("Encounter Finished");

        // Play completion sound, if possible
        if (sound != null)
            s.PlayOneShot(sound);

        // Tell room manager to end encounter
        MapLoader.instance.EndRoomEncounter();

        ClearManager();
    }

    /// <summary>
    /// Disable spawn routine to prevent crashing
    /// </summary>
    private void OnDisable()
    {
        endCheat.Disable();

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }


    /// <summary>
    /// Tell spawner that an enemy is killed, check for wave finished
    /// </summary>
    public void DestroyEnemy()
    {
        enemyCount--;
        CheckWaveFinished();
    }

    /// <summary>
    /// Check if a wave is finished
    /// </summary>
    private void CheckWaveFinished()
    {
        // If still spawning enemies, wave is not finished
        if (spawning || waitingEnemies > 0)
            return;

        // If enemy count is 0, cancel backup and complete wave
        if (enemyCount <= 0)
        {
            if (backupCheckRoutine != null)
            {
                StopCoroutine(backupCheckRoutine);
                backupCheckRoutine = null;
            }
            WaveFinished();
        }
    }

    private IEnumerator CheckCount()
    {
        // Consistantly check if a wave is finished every few seconds
        WaitForSeconds delay = new WaitForSeconds(5f);
        while (!finished)
        {
            CheckWaveFinished();
            yield return delay;
        }
    }

    /// <summary>
    /// Tell spawner that a waiting enemy has been spawned, update counters
    /// </summary>
    public void SpawnedEnemy()
    {
        enemyCount++;
        waitingEnemies--;
    }

    /// <summary>
    /// Return an enemy to the spawn queue
    /// </summary>
    /// <param name="enemy">enemy prefab to return</param>
    public void ReturnEnemy(GameObject enemy)
    {
        spawnQueue.Enqueue(enemy);
        waitingEnemies--;
    }

    /// <summary>
    /// Check if the spawnpoints are empty
    /// </summary>
    /// <returns>Whether spawnpoints are completely empty</returns>
    private bool SpawnpointsEmpty()
    {
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.IsLoaded())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Add an enemy to the counter. Used by enemies that start spawned in a scene
    /// </summary>
    public void AddEnemy()
    {
        enemyCount++;
    }

    private void CheatReset(InputAction.CallbackContext c)
    {
        Debug.Log("Trying to call spawn manager cheat...");

        if (finished)
            return;

        Debug.Log("Spawn manager cheat can work!");

        if (spawnRoutine!= null)
            StopCoroutine(spawnRoutine);

        spawnQueue.Clear();
        CompleteEncounter();
    }

}
