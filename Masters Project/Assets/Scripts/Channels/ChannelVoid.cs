/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 27th, 2022
 * Last Edited - October 27th, 2022 by Ben Schuster
 * Description - Concrete channel that takes no input
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Void Channel", menuName = "Channels/Void Channel")]
public class ChannelVoid : ScriptableObject
{
    /// <summary>
    /// Action(s) invoked when the event is raised
    /// </summary>
    public UnityAction OnEventRaised;

    /// <summary>
    /// Raise all linked events, if possible
    /// </summary>
    public virtual void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
