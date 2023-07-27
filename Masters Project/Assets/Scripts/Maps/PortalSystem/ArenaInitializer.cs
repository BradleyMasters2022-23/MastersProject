using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public override void Init(Cubemap nextCubemap)
    {
        allSpawnpoints = FindObjectsOfType<SpawnPoint>(false);

        base.Init(nextCubemap);

        StartCoroutine(WaitForInput());
    }

    protected IEnumerator WaitForInput()
    {
        // wait for any player input before starting
        InputAction move = InputManager.Controls.PlayerGameplay.Move;
        yield return new WaitUntil(()=> move.ReadValue<Vector2>().magnitude != 0);

        if (encounterData == null)
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
