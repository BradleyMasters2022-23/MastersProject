/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 19th, 2022
 * Last Edited - February 19th, 2022 by Ben Schuster
 * Description - Concrete loading implementation for linear levels.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLoader : SegmentLoader
{
    [Tooltip("Which fields are required to be completed to unlock this room")]
    [SerializeField] private SpawnTriggerField[] combatFields;
    private bool unlocked;

    protected override IEnumerator UniquePoolInitialization()
    {
        // Leave this stuff alone and as the last item
        initialized = true;
        yield return null;
    }

    protected override void UniqueActivate()
    {
        // init all fields
        foreach(var field in combatFields)
        {
            field.Init();
        }

        unlocked = false;
        return;
    }

    protected override void UniqueDeactivate()
    {
        //LinearSpawnManager.instance.IncrementDifficulty();
        return;
    }

    private void Update()
    {
        // Check if the required fields are finished
        if(!unlocked)
        {
            bool finished = true;
            foreach(var field in combatFields)
            {
                if (!field.Finished)
                {
                    finished = false;
                    break;
                }
            }

            if (finished)
            {
                MapLoader.instance.EndRoomEncounter();
                unlocked= true;
            }
                
        }
    }

    public override void StartSegment()
    {
        // init all fields
        foreach (var field in combatFields)
        {
            field.LoadWaves();
        }
        return;
    }
}
