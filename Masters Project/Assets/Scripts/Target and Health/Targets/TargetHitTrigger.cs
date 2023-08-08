/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - August 7th, 2023
 * Last Edited - August 7th, 2023 by Ben Schuster
 * Description - Trigger called when a target is hit. Generic
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetHitTrigger : MonoBehaviour, ITriggerable
{
    [SerializeField] UnityEvent onHitEvents;
    private Trigger mainTrigger;

    private void OnEnable()
    {
        mainTrigger = GetComponent<Trigger>();
        if(mainTrigger != null )
        {
            mainTrigger.Register(this);
        }
    }
    private void OnDisable()
    {
        if (mainTrigger != null)
        {
            mainTrigger.Unregister(this);
        }
    }
    public void Trigger()
    {
        onHitEvents.Invoke();
    }
}
