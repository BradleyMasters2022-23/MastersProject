using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class PortalTrigger : Interactable
{
    [SerializeField] private Collider col;

    [SerializeField] private UnityEvent onInteract;

    [Tooltip("Indicators that play when the portal is summoned")]
    [SerializeField] IIndicator[] summonIndicators;

    [Tooltip("Indicators that play when the portal vanishes")]
    [SerializeField] IIndicator[] vanishIndicators;

    [Tooltip("Indicators that play when the portal is interacted with")]
    [SerializeField] IIndicator[] interactIndicators;

    /// <summary>
    /// Whether the portal is currently usable
    /// </summary>
    [SerializeField, ReadOnly] private bool usable = true;

    /// <summary>
    /// When interacted, perform appropriate action
    /// </summary>
    /// <param name="player">The player reference</param>
    public override void OnInteract(PlayerController player)
    {
        // make sure it can only be used once
        if(usable)
        {
            usable = false;
            Indicators.SetIndicators(interactIndicators, true);
            onInteract.Invoke();
        }
    }

    /// <summary>
    /// Summon the portal effect
    /// </summary>
    public void SummonPortal()
    {
        gameObject.SetActive(true);

        // enable collider
        col.enabled = true;
        col.isTrigger = false;

        // make sure it can be used
        usable = true;

        // DO other artsy things here like sync up animation or start VFX
        Indicators.SetIndicators(summonIndicators, true);
    }

    /// <summary>
    /// Despawn the portal
    /// </summary>
    public void DismissPortal()
    {
        col.enabled = false;

        Indicators.SetIndicators(interactIndicators, false);
        Indicators.SetIndicators(vanishIndicators, true);
    }

    /// <summary>
    /// Try to go to the next room in a run
    /// </summary>
    public void GetNextLevel()
    {
        if (MapLoader.instance != null)
            MapLoader.instance.NextMainPortal();
        else
            Debug.LogError("{name} tried going to next room but couldn't find a MapLoader!");
    }
}
