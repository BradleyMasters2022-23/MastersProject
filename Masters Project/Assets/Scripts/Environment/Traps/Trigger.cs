using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private List<ITriggerable> subscribers;

    private void Awake()
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
