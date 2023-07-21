using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveDataContainer : MonoBehaviour
{
    [SerializeField] ChannelVoid ResetSaveChannel;

    protected virtual void OnEnable()
    {
        ResetSaveChannel.OnEventRaised += ResetData;
    }
    protected virtual void OnDisable()
    {
        ResetSaveChannel.OnEventRaised -= ResetData;
    }

    public abstract void ResetData();
}
