using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRespawner : MonoBehaviour
{
    [SerializeField] ChannelVoid respawnTriggerChannel;

    [SerializeField] private GameObject barrelPrefab;
    private GameObject spawnedRef;

    private void Start()
    {
        TryRespawn();
    }

    private void OnEnable()
    {
        respawnTriggerChannel.OnEventRaised += TryRespawn;
    }

    private void OnDisable()
    {
        respawnTriggerChannel.OnEventRaised -= TryRespawn;
    }

    private void TryRespawn()
    {
        if (spawnedRef == null)
        {
            spawnedRef = Instantiate(barrelPrefab, transform);
        }
    }
}
