/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 2nd, 2022
 * Last Edited - June 2nd, 2022 by Ben Schuster
 * Description - Concrete trigger that activates when interacted with
 * ================================================================================================
 */
using UnityEngine;
using UnityEngine.Events;

public class InteractTrigger : Trigger, Interactable
{
    [SerializeField] private UnityEvent onInteract;

    /// <summary>
    /// When interacted, perform the required events
    /// </summary>
    /// <param name="player">Player controller</param>
    public void OnInteract(PlayerController player)
    {
        if (onInteract != null) onInteract?.Invoke();
    }
}
