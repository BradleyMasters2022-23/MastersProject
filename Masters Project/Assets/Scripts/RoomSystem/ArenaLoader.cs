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
    private EncounterSO chosenEncounter;

    private SpawnPoint[] allSpawnpoints;



    protected override void UniquePoolInitialization()
    {
        // Get reference to all spawnpoints
        allSpawnpoints = GetComponentsInChildren<SpawnPoint>(true);
    }

    protected override void UniquePoolPull()
    {
        // Get a random encounter from the segment info
        chosenEncounter = segmentInfo.potentialEncounters[Random.Range(0, segmentInfo.potentialEncounters.Length)];

        // Send necessary data to spawner [spawnpoints, encounters]

    }

    protected override void UniquePoolReturn()
    {
        chosenEncounter = null;
    }

}
