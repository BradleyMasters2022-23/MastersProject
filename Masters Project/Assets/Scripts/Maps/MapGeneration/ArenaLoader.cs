/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 3st, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Concrete loading implementation for combat area segments.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaLoader : SegmentLoader
{
    /// <summary>
    /// Chosen wave to use this encounter
    /// </summary>
    [SerializeField] private EncounterDifficulty[] encounterData;

    /// <summary>
    /// All spawnpoints to use
    /// </summary>
    private SpawnPoint[] allSpawnpoints;



    protected override IEnumerator UniquePoolInitialization()
    {
        // Get reference to all spawnpoints
        allSpawnpoints = GetComponentsInChildren<SpawnPoint>(true);

        initialized = true;
        yield return null;
    }

    protected override void UniqueActivate()
    {
        // Send necessary data to spawner [spawnpoints, encounters]
        SpawnManager.instance.PrepareEncounter(encounterData, allSpawnpoints);
    }

    protected override void UniqueDeactivate()
    {
        //chosenEncounter = null;
        //LinearSpawnManager.instance.IncrementDifficulty();
    }

    public override void StartSegment()
    {
        SpawnManager.instance.BeginEncounter();
        return;
    }
}
