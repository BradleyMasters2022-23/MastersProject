/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 2nd, 2022
 * Last Edited - June 2nd, 2022 by Ben Schuster
 * Description - Initializer for a secret room
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoomInitializer : RoomInitializer
{
    [Tooltip("Portal within the secret room itself")]
    [SerializeField] private PortalTrigger secretRoomPortalRef;

    /// <summary>
    /// Initializer for the current secret room
    /// </summary>
    public static SecretRoomInitializer instance;

    /// <summary>
    /// Get an instance for ease of reference
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }

    private void OnDisable()
    {
        instance = null;
    }

    /// <summary>
    /// The portal to return to when leaving
    /// </summary>
    private PortalTrigger returnPortal;

    /// <summary>
    /// Link this secret room to a return portal
    /// </summary>
    /// <param name="p">Return portal to teleport to on exit</param>
    public PortalTrigger LinkPortals(PortalTrigger p)
    {
        returnPortal = p;
        return secretRoomPortalRef;
    }

    /// <summary>
    /// Return to the original room
    /// </summary>
    public void ReturnToRoom()
    {
        returnPortal.TeleportToPortal();
    }

}
