/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 21st, 2023
 * Last Edited - July 21st, 2023 by Ben Schuster
 * Description - Abstract class for any container that stores save data
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveDataContainer : MonoBehaviour
{
    [Tooltip("Channel called when data is cleared. Will reset this container's loaded data")]
    [SerializeField] ChannelVoid ResetSaveChannel;

    protected virtual void OnEnable()
    {
        ResetSaveChannel.OnEventRaised += ResetData;
    }
    protected virtual void OnDisable()
    {
        ResetSaveChannel.OnEventRaised -= ResetData;
    }

    /// <summary>
    /// Reset the current data, but don't save it
    /// </summary>
    public abstract void ResetData();
}
