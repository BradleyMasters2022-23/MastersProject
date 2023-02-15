/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 15th, 2023
 * Last Edited - February 15th, 2023 by Ben Schuster
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
    [Header("Core Data")]

    [Tooltip("Whether or not this field is currently active.")]
    [SerializeField, ReadOnly] private bool active;
    [Tooltip("Any fields that conflict with the encounters. Set this to any triggers meant to be used from the opposite direction.")]
    [SerializeField] private SpawnTriggerField[] conflictingTriggers;
    [Tooltip("All spawnpoints usable with this field. Can be shared with others.")]
    [SerializeField] private SpawnPoint[] spawnPoints;
    /// <summary>
    /// All colliders used for this trigger
    /// </summary>
    private Collider[] triggerCol;

    [Header("Combat Data")]

    [Tooltip("All waves of relative difficulties to use for this encounter.")]
    [SerializeField] private RelativeDifficulty[] relativeDifficulty;
    [Tooltip("Modifier to the encounter size, intended based on size of the room the encouter is in. Multiplied to the batch list.")]
    [SerializeField] private float sizeModifier = 1;

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
        if(relativeDifficulty.Length > 0 && triggerCol.Length > 0)
            active = true;
    }

    /// <summary>
    /// Disable this spawn trigger
    /// </summary>
    public void Deactivate()
    {
        active = false;

        if(triggerCol != null)
        {
            foreach(Collider col in triggerCol)
            {
                col.enabled = false;
            }
        }
    }

    /// <summary>
    /// Send the data to the spawn manager to begin the encounter
    /// TODO LATER - work in way to 'block player path'
    /// </summary>
    public void StartEncounter()
    {
        //Debug.Log("Sending spawner data for...");
        
        //Debug.Log($"Spawnpoint: {spawnPoints.Length}");

        //Debug.Log($"Requesting waves of...");
        foreach (RelativeDifficulty diffs in relativeDifficulty)
        {
            //Debug.Log($"{diffs} of size modifier of {sizeModifier}");
        }

        LinearSpawnManager.instance.RequestBatch(relativeDifficulty, spawnPoints, sizeModifier);
    }


    /// <summary>
    /// On activation, begin the encounter
    /// </summary>
    /// <param name="other">Other object triggering this</param>
    private void OnTriggerEnter(Collider other)
    {
        if(active && other.CompareTag("Player"))
        {
            // Deactivate this field and any conflicting fields
            Deactivate();
            foreach(SpawnTriggerField otherTrigger in conflictingTriggers)
            {
                otherTrigger.Deactivate();
            }

            StartEncounter();
        }
        else if(!active)
        {
            // If not active and triggered, try disabling triggers again
            Deactivate();
        }
    }
}
