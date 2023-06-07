/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 1st, 2022
 * Last Edited - June 6th, 2022 by Ben Schuster
 * Description - Trigger for the portals that can teleport the player
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class PortalTrigger : MonoBehaviour, Interactable
{
    [Header("Setup")]

    [Tooltip("The point to teleport to")]
    [SerializeField] private Transform exitPoint;
    [Tooltip("Reference to the collider used for interaction")]
    [SerializeField] private Collider col;
    [Tooltip("Whether this portal should summon immediately")]
    [SerializeField] private bool instantlyOpen;
    [Tooltip("Whether this portal is currently usable")]
    [SerializeField, ReadOnly] private bool usable = true;
    [Tooltip("Events to execute on interact")]
    [SerializeField] private UnityEvent onInteract;
    
    [Header("VFX/SFX Indicators")]

    [Tooltip("Indicators that play when the portal is summoned")]
    [SerializeField] IIndicator[] summonIndicators;
    [Tooltip("Indicators that play when the portal vanishes")]
    [SerializeField] IIndicator[] vanishIndicators;
    [Tooltip("Indicators that play when the portal is interacted with")]
    [SerializeField] IIndicator[] interactIndicators;

    [Header("Scaling over Distance")]
    
    [Tooltip("Scale of the X and Z axis over distance")]
    [SerializeField] private AnimationCurve horizontalScaleOverDistance;
    [Tooltip("Transform of the portal that sacles over distance")]
    [SerializeField] private Transform portalScaleObject;
    /// <summary>
    /// Reference to the player transform
    /// </summary>
    private Transform playerRef;
    /// <summary>
    /// Current scale modifier
    /// </summary>
    private Vector3 horScale;

    private SecretPortalInstance secretRef;

    /// <summary>
    /// If set to instantly open, do it
    /// </summary>
    private void Awake()
    {
        horScale = portalScaleObject.localScale;

        if (instantlyOpen)
            SummonPortal();
        else
            DismissPortal();
    }

    private void Update()
    {
        // Continually update the scale of the shard based on player distance
        HandleSquish();
    }

    /// <summary>
    /// Modify the X and Z scales based on distance
    /// </summary>
    private void HandleSquish()
    {
        // Get the player reference if not already acquired
        if (playerRef == null)
            playerRef = PlayerTarget.p.transform;

        // If properly set up, update the scale based on the current distance to the player
        if (portalScaleObject != null)
        {
            float dist = Vector3.Distance(transform.position, playerRef.position);
            horScale.x = horizontalScaleOverDistance.Evaluate(dist);
            horScale.z = horizontalScaleOverDistance.Evaluate(dist);
            portalScaleObject.localScale = horScale;
        }
    }

    /// <summary>
    /// When interacted, perform appropriate action
    /// </summary>
    /// <param name="player">The player reference</param>
    public void OnInteract(PlayerController player)
    {
        // make sure it can only be used once
        if(usable)
        {
            Indicators.SetIndicators(interactIndicators, true);
            onInteract.Invoke();
        }
    }

    /// <summary>
    /// Summon the portal effect
    /// </summary>
    public void SummonPortal()
    {
        // Get reference for the scale, enable it
        gameObject.SetActive(true);
        horScale = portalScaleObject.localScale;

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
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Try to go to the next room in a run
    /// </summary>
    public void GetNextLevel()
    {
        if (MapLoader.instance != null)
            MapLoader.instance.NextMainPortal();
        else
            Debug.LogError($"{name} tried going to next room but couldn't find a MapLoader!");
    }

    public void TeleportToPortal()
    {
        // Debug.Log($"Player : {playerRef != null} | Exit point : {exitPoint !=  null}");

        playerRef.position = exitPoint.position;
        playerRef.rotation = exitPoint.rotation;
    }

    public void AssignSecretRef(SecretPortalInstance s)
    {
        secretRef = s;
    }
    public void GoToSecretRoom()
    {
        secretRef?.GoToSecretRoom();
    }
}
