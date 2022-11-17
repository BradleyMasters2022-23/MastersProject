/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 3st, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Concrete loading implementation for hallway segments.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayLoader : SegmentLoader
{
    [SerializeField] private FragmentSpawner fragSpawner;
    [SerializeField] private Transform fragSpawnPoint;
    protected override IEnumerator UniquePoolInitialization()
    {
        // Put upgrade initialization stuff here
        fragSpawner.SetSpawnPoint(fragSpawnPoint);
        fragSpawner.SpawnFragment();

        // Leave this stuff alone and as the last item
        initialized = true;
        yield return null;
    }

    protected override void UniqueActivate()
    {
        // Unlock the hallway door right after activation
        doorManager.UnlockExit();

        return;
    }

    protected override void UniqueDeactivate()
    {
        return;
    }
}
