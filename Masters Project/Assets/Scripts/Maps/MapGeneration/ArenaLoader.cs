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
    private EncounterSO chosenEncounter;

    /// <summary>
    /// All spawnpoints to use
    /// </summary>
    private SpawnPoint[] allSpawnpoints;



    protected override IEnumerator UniquePoolInitialization()
    {
        // Get reference to all spawnpoints
        allSpawnpoints = GetComponentsInChildren<SpawnPoint>(true);


        //Debug.Log("Arena segment finished initialization");
        initialized = true;
        yield return null;
    }

    protected override void UniqueActivate()
    {
        // Get a random encounter from the segment info
        chosenEncounter = segmentInfo.potentialEncounters[Random.Range(0, segmentInfo.potentialEncounters.Length)];
        Debug.Log("Loading in encounter named: " + chosenEncounter.name);
        

        // Send necessary data to spawner [spawnpoints, encounters]
        SpawnManager.instance.PrepareEncounter(chosenEncounter, allSpawnpoints);
    }

    protected override void UniqueDeactivate()
    {
        chosenEncounter = null;
    }

}
