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
    [SerializeField] private GameObject fragSpawner;
    [SerializeField] private Transform fragSpawnPoint;

    [SerializeField] private GameObject upgradeContainer;

    protected override IEnumerator UniquePoolInitialization()
    {
        // Put upgrade initialization stuff here
        fragSpawner.GetComponent<FragmentSpawner>().SetSpawnPoint(fragSpawnPoint);
        fragSpawner.GetComponent<FragmentSpawner>().SpawnFragment();

        // Leave this stuff alone and as the last item
        initialized = true;
        yield return null;
    }

    protected override void UniqueActivate()
    {

        return;
    }

    protected override void UniqueDeactivate()
    {
        return;
    }

    /// <summary>
    /// When the upgrade is selected, unlock the exit
    /// </summary>
    public void UpgradeSelected()
    {
        doorManager.UnlockExit();
    }
}
