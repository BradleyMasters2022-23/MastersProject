/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 2nd, 2022
 * Last Edited - June 2nd, 2022 by Ben Schuster
 * Description - The instance in a level that handles summoning secret rooms
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SecretPortalInstance : MonoBehaviour
{
    [Tooltip("Portal within the level that leads to the secret room")]
    [SerializeField] private PortalTrigger secretPortalRef;

    [SerializeField, ReadOnly] private PortalTrigger secretRoomPortal;

    /// <summary>
    /// If there is a secret room loaded, then initialize and link with it\
    /// </summary>
    public void Init(PortalTrigger portal)
    {
        secretPortalRef = portal;
        StartCoroutine(WaitToLoad());
    }
    /// <summary>
    /// Wait for the initializer to instantiate before starting
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToLoad()
    {
        yield return new WaitUntil(() => SecretRoomInitializer.instance != null);

        SecretRoomInitializer.instance.Init(null);
        LinkToSecretRoom(SecretRoomInitializer.instance);
        secretPortalRef.DismissPortal();
        secretRoomPortal.SummonPortal();
    }

    /// <summary>
    /// Summon the portal that goes to the secret room
    /// </summary>
    public void SummonSecretPortal()
    {
        secretPortalRef.SummonPortal();
    }

    /// <summary>
    /// Dismiss the portal that goes to the secret room
    /// </summary>
    public void ClosePortal()
    {
        secretPortalRef.DismissPortal();
    }

    /// <summary>
    /// To to the secret room. Called by unity event in inspector.
    /// </summary>
    public void GoToSecretRoom()
    {
        //Debug.Log("Going to secret room");
        
        secretRoomPortal.TeleportToPortal();
    }

    /// <summary>
    /// Link the portals between this and the secret room itself
    /// </summary>
    /// <param name="secretRoom"></param>
    private void LinkToSecretRoom(SecretRoomInitializer secretRoom)
    {
        secretRoomPortal = secretRoom.LinkPortals(secretPortalRef);
    }
}
