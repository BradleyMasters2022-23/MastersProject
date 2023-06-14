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
    /// Can be interacted if theres something to execute
    /// </summary>
    /// <returns>Whether this interact can be used</returns>
    public bool CanInteract()
    {
        return true;
    }

    /// <summary>
    /// When interacted, perform the required events
    /// </summary>
    /// <param name="player">Player controller</param>
    public void OnInteract()
    {
        if (onInteract != null) onInteract?.Invoke();
    }
}
