/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 27th, 2022
 * Last Edited - October 27th, 2022 by Ben Schuster
 * Description - Core channel base for one value
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChannelBase<T> : ScriptableObject
{
    /// <summary>
    /// Action(s) invoked when the event is raised
    /// </summary>
    public UnityAction<T> OnEventRaised;

    /// <summary>
    /// Raise all linked events, if possible
    /// </summary>
    /// <param name="value">Value to pass into the hose channel</param>
    public virtual void RaiseEvent(T value = default(T))
    {
        OnEventRaised?.Invoke(value);
    }
}
