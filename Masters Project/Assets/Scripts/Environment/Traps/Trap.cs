using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour, ITriggerable
{
    [SerializeField] private Trigger[] activationTriggers;

    private void Start()
    {
        foreach(Trigger trigger in activationTriggers)
        {
            trigger.Register(this);
        }
    }

    private void OnDisable()
    {
        foreach (Trigger trigger in activationTriggers)
        {
            trigger.Unregister(this);
        }
    }

    private void Activate()
    {
        Debug.Log($"{gameObject.name} Trigger activated, starting now!");
    }

    public void Trigger()
    {
        Activate();
    }
}
