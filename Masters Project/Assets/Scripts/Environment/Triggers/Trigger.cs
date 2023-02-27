/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 17th, 2022
 * Last Edited - February 17th, 2022 by Ben Schuster
 * Description - Core base for flexible trigger system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    protected List<ITriggerable> subscribers;

    protected virtual void Awake()
    {
        subscribers= new List<ITriggerable>();
    }

    public void Register(ITriggerable triggerable)
    {
        if(subscribers!= null && !subscribers.Contains(triggerable))
        {
            subscribers.Add(triggerable);
        }
    }

    public void Unregister(ITriggerable triggerable)
    {
        if(subscribers!= null && subscribers.Contains(triggerable))
        {
            subscribers.Remove(triggerable);
            subscribers.TrimExcess();
        }
    }

    public void Activate()
    {
        if (subscribers == null)
            return;

        foreach(ITriggerable triggerable in subscribers)
        {
            triggerable.Trigger();
        }
    }
}
