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

        // Leave this stuff alone and as the last item
        initialized = true;
        yield return null;
    }

    protected override void UniqueActivate()
    {
        // quick null check
        FragmentSpawner fragSpawnerComp = fragSpawner.GetComponent<FragmentSpawner>();

        if(fragSpawnerComp!= null )
        {
            fragSpawnerComp.SetSpawnPoint(fragSpawnPoint);
            fragSpawnerComp.SpawnFragment(transform);
        }

        doorManager.UnlockExit();
        return;
    }

    protected override void UniqueDeactivate()
    {
        return;
    }

    /// <summary>
    /// When the upgrade is selected, unlock the exit
    /// </summary>
    //public void UpgradeSelected()
    //{
    //    doorManager.UnlockExit();
    //}

    public override void StartSegment()
    {
        // dont do anything, internal triggers activate this
        return;
    }
}
