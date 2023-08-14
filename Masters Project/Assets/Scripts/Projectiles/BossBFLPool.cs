using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBFLPool : DamagePool
{
    [SerializeField] ChannelVoid bossChannel;

    private void OnEnable()
    {
        bossChannel.OnEventRaised += InstantDestroyPool;
    }
    private void OnDisable()
    {
        bossChannel.OnEventRaised -= InstantDestroyPool;
    }

    /// <summary>
    /// instantly despawn the pool. Called when boss killed
    /// </summary>
    public void InstantDestroyPool()
    {
        if(targetField!= null)
            targetField.enabled = false;

        vanishing = true;
        StartCoroutine(Vanish());
    }
}
