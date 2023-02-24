using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrapSpawnPoint : MonoBehaviour
{
    [Tooltip("What type of traps can go on this spawn point")]
    [SerializeField] private TrapPlacementType placementType;

    // Blacklist of traps 
    [Tooltip("Set which traps can NEVER be placed here")]
    [SerializeField] private Trap blacklist;

    private bool usable;

    [SerializeField] private GameObject spawnedTraps;

    private void Awake()
    {
        usable = true;
    }

    /*
    /// <summary>
    /// Spawn all passed in traps 
    /// </summary>
    /// <param name="trapPrefab"></param>
    /// <returns></returns>
    public bool SpawnTrap(GameObject trapPrefab)
    {
        // Check if this spot can be used first
        if (!usable 
            || !trapPrefab.GetComponent<Trap>().PlacementType().Contains(placementType))
            return false;


        spawnedTraps = new GameObject[placementPoints.Length];
        for (int i = 0; i < placementPoints.Length; i++)
        {
            spawnedTraps[i] = Instantiate(trapPrefab, placementPoints[i]);
        }

        return true;
    }

    public bool Usable(GameObject trapPrefab)
    {

    }

    */
}
