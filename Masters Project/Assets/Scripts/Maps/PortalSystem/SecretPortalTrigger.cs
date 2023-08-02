using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPortalTrigger : PortalTrigger
{
    /// <summary>
    /// reference to the secret portal
    /// </summary>
    private SecretPortalInstance secretRef;

    public void AssignSecretRef(SecretPortalInstance s)
    {
        secretRef = s;
    }
    public void GoToSecretRoom()
    {
        MapLoader.instance.PlaySecretPortalSFX();
        secretRef?.GoToSecretRoom();
    }
}
