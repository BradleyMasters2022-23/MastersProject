using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecretPortalRandomizer : LayoutRandomizer
{
    private SecretPortalInstance initializer;

    /// <summary>
    /// After randomly selecting which to use, initialize that secret portal
    /// </summary>
    public override void Randomize()
    {
        base.Randomize();

        initializer = GetComponent<SecretPortalInstance>();
        PortalTrigger selectedPortal = layouts[chosenIndex].layoutRoot.GetComponentInChildren<PortalTrigger>(true);
        DistortedProp selectedProp = layouts[chosenIndex].layoutRoot.GetComponentInChildren<DistortedProp>(true);
        if (selectedPortal != null)
        {
            initializer.Init(selectedPortal);
            selectedProp.Init();
        }
        else
        {
            Debug.Log("No secret portal found!");
        }
    }
}
