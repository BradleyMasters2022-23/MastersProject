/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Manage individual spawnpoints
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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
    [SerializeField] private VisualEffect spawnParticles;

    [SerializeField] private float VFXSpeedScalar;

    /// <summary>
    /// Enemy for this spawn point to spawn
    /// </summary>
    [ShowInInspector, ReadOnly] private EnemySO enemyStorage;
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

    [Tooltip("Sound that plays while spawning enemy")]
    [SerializeField] private AmbientSFXSource spawnSoundManager;

    [Tooltip("What enemies are allowed to spawn on this spawnpoint. Drag enemy prefabs here.")]
    [SerializeField] private List<EnemySO> enemyWhitelist;

    [SerializeField] private List<EnemySO> enemyBlacklist;

    private GameControls controls;
    private InputAction endCheat;

    private GameObject lastSpawnedEnemy;

    [Header("Spawn Effects")]
    [SerializeField] MeshFilter spawnMesh;
    [SerializeField] Spawnable spawnEffectController;

    /// <summary>
    /// Initialize settings
    /// </summary>
    private void Awake()
    {
        // Initialize timers
        spawnDelayTimer = new ScaledTimer(spawnDelay);
        spawnOverrideTimer = new ScaledTimer(spawnOverrideDelay);
    }

    /// <summary>
    /// Get outside prefab references
    /// </summary>
    private void Start()
    {
        player = PlayerTarget.p.transform;
        spawnManager = SpawnManager.instance;

        if(spawnParticles!= null)
        {
            spawnParticles.Stop();
            spawnParticles.playRate *= VFXSpeedScalar;
        }

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
    public void LoadSpawn(EnemySO enemy)
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
    public bool Open(EnemySO proposedEnemy)
    {
        if (enemyStorage != null)
            return false;

        if (enemyWhitelist.Count > 0)
        {
            bool _allowed = false;
            foreach (var type in enemyWhitelist)
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
        spawnDelayTimer?.ResetTimer();
        spawnOverrideTimer?.ResetTimer();
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
        if (!spawning && IsLoaded() && spawnOverrideTimer.TimerDone() && spawnManager!=null)
        {
            spawnManager.ReturnEnemy(enemyStorage);
            enemyStorage = null;
        }

        overlapped = Overlapped();

        // TODO - change to normal camera grab once 'perspective cheats' are gone

        if ((!overlapped
            && Camera.main.GetComponent<CameraShoot>().InCamVision(transform.position)
            && CheckDist())
            || overrideSpawn)
        {
            if (!spawning && enemyStorage != null)
            {
                spawning = true;
                spawnRoutine = StartCoroutine(SpawnEnemy(enemyStorage));
            }
        }
    }

    /// <summary>
    /// Check if the enemy can spawn in a linear aspect.
    /// Currently, only check overlap and blacklist status
    /// </summary>
    /// <param name="enemyData">Type of enemy to spawn</param>
    /// <returns>Whether this enemy can currently be spawned</returns>
    public bool CanSpawnLinear(EnemySO enemyData)
    {
        return (!InBlacklist(enemyData) && !Overlapped() && !spawning);
    }

    /// <summary>
    /// Check if the passed in enemy is in the blacklist
    /// </summary>
    /// <param name="enemy">enemy data to check</param>
    /// <returns>Whether the enemy is in the blacklist</returns>
    private bool InBlacklist(EnemySO enemy)
    {
        return enemyBlacklist.Contains(enemy);
    }
    /// <summary>
    /// Whether this spawnpoint is overlapped by an enemy or player
    /// </summary>
    /// <returns>Whether this spawnpoint is overlapped</returns>
    private bool Overlapped()
    {
        return Physics.CheckCapsule(transform.position, transform.position + Vector3.up * 1, 0.5f, overlapMask);
    }

    /// <summary>
    /// Spawn an enemy now in the linear style
    /// </summary>
    /// <param name="enemyData">Enemy data to spawn</param>
    /// <param name="associatedField">The field to send the spawned enemy to</param>
    public void SpawnNow(EnemySO enemyData, SpawnTriggerField associatedField)
    {
        spawning = true;
        spawnRoutine = StartCoroutine(SpawnEnemy(enemyData, associatedField));
    }

    /// <summary>
    /// Spawn an enemy. Handle spawning and visuals here
    /// </summary>
    private IEnumerator SpawnEnemy(EnemySO enemy, SpawnTriggerField sendTo = null)
    {
        // immediately spawn the enemy, but disable it temporarily
        lastSpawnedEnemy = EnemyPooler.instance.RequestEnemy(enemy.enemyPrefab);
        // just incase it fails, return it
        if(lastSpawnedEnemy == null)
        {
            spawnManager.ReturnEnemy(enemy);
            yield break;
        }
        lastSpawnedEnemy.transform.position = transform.position;
        lastSpawnedEnemy.SetActive(false);

        bool spawnComplete = false;

        // get direction to player, flatten it
        Vector3 toPlayer = (player.position - transform.position).normalized;
        toPlayer.y = 0;

        // get spawn effects, make them look at player
        spawnMesh.mesh = enemy.enemySpawnMesh;
        spawnMesh.transform.LookAt(transform.position + toPlayer);
        spawnMesh.transform.Rotate(Vector3.up, enemy.rotationOverride);
        spawnMesh.gameObject.SetActive(true);

        if (spawnParticles != null)
            spawnParticles.Play();

        // start spawn effects, wait for them to finish spawning
        if (spawnSoundManager != null)
        {
            spawnSoundManager.Play();
        }
        spawnEffectController.StartSpawning(enemy.playbackSpeedModifier, () => spawnComplete = true);
        yield return new WaitUntil(() => spawnComplete);

        // If missing, then enemy is gone so stop. Only happens in unload of the tutorial
        if (lastSpawnedEnemy == null)
            yield break;

        // Enable enemy, make look at player
        lastSpawnedEnemy.SetActive(true);
        lastSpawnedEnemy.transform.LookAt(player.transform.position);

        if (sendTo != null)
            sendTo.EnqueueEnemy(lastSpawnedEnemy);


        // Reset the spawnpoint, disable indicators
        enemyStorage = null;
        spawnRoutine = null;
        if(spawnParticles != null)
            spawnParticles.Stop();

        // start spawn effects, wait for them to finish spawning
        if (spawnSoundManager != null)
        {
            spawnSoundManager.Stop();
        }

        spawning = false;
        
        if(spawnManager != null)
            spawnManager.SpawnedEnemy();
    }

    /// <summary>
    /// Disable routine if still active to prevent crash
    /// </summary>
    private void OnDisable()
    {
        if(endCheat != null)
            endCheat.performed -= CheatClear;
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
        if (spawnParticles != null)
            spawnParticles.Stop();

        // start spawn effects, wait for them to finish spawning
        if (spawnSoundManager != null)
        {
            spawnSoundManager.Stop();
        }

        spawning = false;

        if(spawnManager != null)
            spawnManager.SpawnedEnemy();
    }
}
