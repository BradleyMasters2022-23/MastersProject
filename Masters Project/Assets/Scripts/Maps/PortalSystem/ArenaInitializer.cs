using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaInitializer : RoomInitializer
{
    [Header("Arena Loading")]
    /// <summary>
    /// Chosen wave to use this encounter
    /// </summary>
    [SerializeField] private EncounterDifficulty[] encounterData;

    /// <summary>
    /// All spawnpoints to use
    /// </summary>
    [ShowInInspector, ReadOnly] private SpawnPoint[] allSpawnpoints;

    public override void Init()
    {
        allSpawnpoints = FindObjectsOfType<SpawnPoint>(false);

        base.Init();

        if(encounterData == null)
        {
            MapLoader.instance.EndRoomEncounter();
        }
        else
        {
            SpawnManager.instance.PrepareEncounter(encounterData, allSpawnpoints);
            SpawnManager.instance.BeginEncounter();
        }
        
    }
}
