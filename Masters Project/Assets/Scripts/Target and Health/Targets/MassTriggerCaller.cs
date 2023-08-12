using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassTriggerCaller : MonoBehaviour
{
    [SerializeField] GameObject[] targetTriggers;

    public void HitAllTriggers()
    {
        ITriggerable temp;
        foreach (var trigger in targetTriggers)
        {
            temp = trigger.GetComponent<ITriggerable>();
            if (temp != null)
                temp.Trigger();
        }
    }
}
