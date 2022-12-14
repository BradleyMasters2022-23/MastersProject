/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Manage individual spawnpoints
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("How long should the spawn take")]
    [SerializeField] private float spawnDelay;
    [Tooltip("What should prevent this spawnpoint from being usable")]
    [SerializeField] private LayerMask overlapMask;
    [Tooltip("Override - How long should this spawnpoint wait until canceling enemy spawn")]
    [SerializeField] private float spawnOverrideDelay;
    [Tooltip("The lower and upper bound for distance between player when spawning")]
    [SerializeField] private Vector2 distanceRange;

    /// <summary>
    /// Linked spawn manager
    /// </summary>
    private SpawnManager spawnManager;

    /// <summary>
    /// What particle plays when spawning
    /// </summary>
    private ParticleSystem spawnParticles;
    /// <summary>
    /// Enemy for this spawn point to spawn
    /// </summary>
    private GameObject enemyStorage;
    /// <summary>
    /// Whether or not the spawn point is overlapped by a living enemy
    /// </summary>
    private bool overlapped;
    /// <summary>
    /// Whether or not this spawnpoint should ignore its rules and start spawning. Failsafe
    /// </summary>
    private bool overrideSpawn;
    /// <summary>
    /// Whether or not this spawn point is currently spawning an enemy
    /// </summary>
    private bool spawning;

    /// <summary>
    /// Time tracker for spawn delay
    /// </summary>
    private ScaledTimer spawnDelayTimer;
    /// <summary>
    /// Time tracker for override delay
    /// </summary>
    private ScaledTimer spawnOverrideTimer;

    /// <summary>
    /// Current spawn coroutine
    /// </summary>
    private Coroutine spawnRoutine;
    /// <summary>
    /// Transform of the player object
    /// </summary>
    private Transform player;
    /// <summary>
    /// Light visual for indication
    /// </summary>
    private Light spawnLight;
    /// <summary>
    /// Audio source player
    /// </summary>
    private AudioSource s;
    [Tooltip("Sound that plays while spawning enemy")]
    [SerializeField] private AudioClip spawnSound;

    [Tooltip("What enemies are allowed to spawn on this spawnpoint. Drag enemy prefabs here.")]
    [SerializeField] private GameObject[] enemyWhitelist;

    private GameControls controls;
    private InputAction endCheat;


    /// <summary>
    /// Initialize settings
    /// </summary>
    private void Awake()
    {
        // Get internal references
        spawnLight = GetComponentInChildren<Light>();
        spawnParticles = GetComponentInChildren<ParticleSystem>();
        s = gameObject.AddComponent<AudioSource>();
        s.spatialBlend = 1;

        // Initialize timers
        spawnDelayTimer = new ScaledTimer(spawnDelay);
        spawnOverrideTimer = new ScaledTimer(spawnOverrideDelay);


        s.playOnAwake = false;
        // Prepare sound
        if (spawnSound != null)
            s.clip = spawnSound;

        s.volume = 0.5f;
        
    }

    /// <summary>
    /// Get outside prefab references
    /// </summary>
    private void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spawnManager = SpawnManager.instance;

        controls = GameManager.controls;
        endCheat = controls.PlayerGameplay.ClearEncounter;
        endCheat.performed += CheatClear;
        endCheat.Enable();
    }

    /// <summary>
    /// Check whether the spawnpoint is loaded with an enemy
    /// </summary>
    /// <returns>Whether the spawnpoint is loaded with an enemy</returns>
    public bool IsLoaded()
    {
        return enemyStorage != null;
    }

    /// <summary>
    /// Load an enemy into this spawn point
    /// </summary>
    /// <param name="enemy">Enemy to load into spawnpoint</param>
    /// <returns>Whether or not an enemy could be loaded into spawnpoint</returns>
    public void LoadSpawn(GameObject enemy)
    {
        if (enemyStorage == null)
        {
            ResetSpawnPoint();
            enemyStorage = enemy;
        }
        else
        {
            Debug.LogError("Spawnpoint told to load enemy when already filled!");
        }
    }

    /// <summary>
    /// Check whether this spawnpoint is open and available
    /// </summary>
    /// <param name="proposedEnemy">Enemy being offered to spawnpoint</param>
    /// <returns>Whether the spawnpoint took the enemy</returns>
    public bool Open(GameObject proposedEnemy)
    {
        if (enemyStorage != null)
            return false;

        bool _allowed = false;
        if (enemyWhitelist.Length > 0)
        {
            //EnemyManager proposed = proposedEnemy.GetComponent<EnemyManager>();
            foreach (GameObject type in enemyWhitelist)
            {
                if (type == proposedEnemy)
                    _allowed = true;
            }

            return (_allowed && enemyStorage == null);
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Resets the spawnpoint's timer
    /// </summary>
    private void ResetSpawnPoint()
    {
        spawnDelayTimer.ResetTimer();
        spawnOverrideTimer.ResetTimer();
    }

    /// <summary>
    /// Check the distance to the player
    /// </summary>
    /// <returns>Whether the player is within range</returns>
    private bool CheckDist()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        return (dist >= distanceRange.x && dist <= distanceRange.y);
    }

    private void Update()
    {
        if (!spawning && IsLoaded() && spawnOverrideTimer.TimerDone())
        {
            spawnManager.ReturnEnemy(enemyStorage);
            enemyStorage = null;
        }

        overlapped = Physics.CheckCapsule(transform.position, transform.position + Vector3.up * 1, 0.5f, overlapMask);

        // TODO - change to normal camera grab once 'perspective cheats' are gone

        if ((!overlapped
            && Camera.main.GetComponent<CameraShoot>().InCamVision(transform.position)
            && CheckDist())
            || overrideSpawn)
        {
            if (!spawning && enemyStorage != null)
            {
                spawning = true;
                spawnRoutine = StartCoroutine(SpawnEnemy());
            }
        }
    }

    /// <summary>
    /// Spawn an enemy. Handle spawning and visuals here
    /// </summary>
    private IEnumerator SpawnEnemy()
    {
        // Prepare indicators
        if (spawnSound != null)
            s.Play();
        spawnLight.enabled = true;
        if(spawnParticles != null)
            spawnParticles.Play();

        // Reset the timer and wait for the delay
        spawnDelayTimer.ResetTimer();
        while (!spawnDelayTimer.TimerDone())
            yield return null;

        // Spawn enemy
        Instantiate(enemyStorage, transform.position, Quaternion.identity);

        // Reset the spawnpoint, disable indicators
        enemyStorage = null;
        spawnRoutine = null;
        spawnLight.enabled = false;
        if(spawnParticles != null)
            spawnParticles.Stop();
        if (spawnSound != null)
            s.Stop();
        spawning = false;
        spawnManager.SpawnedEnemy();
    }

    /// <summary>
    /// Disable routine if still active to prevent crash
    /// </summary>
    private void OnDisable()
    {
        endCheat.Disable();
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private void CheatClear(InputAction.CallbackContext c)
    {
        //Debug.Log("Clear spawnpoints called");

        if(spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        enemyStorage = null;
        spawnRoutine = null;
        spawnLight.enabled = false;
        if (spawnParticles != null)
            spawnParticles.Stop();
        if (spawnSound != null)
            s.Stop();
        spawning = false;
        spawnManager.SpawnedEnemy();
    }
}
