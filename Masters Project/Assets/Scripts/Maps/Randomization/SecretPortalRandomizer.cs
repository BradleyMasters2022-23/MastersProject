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

        // Debug.Log($"Chosen index: {chosenIndex} of {layouts.Length}");
        SecretPortalTrigger selectedPortal = layouts[chosenIndex].layoutRoot.GetComponentInChildren<SecretPortalTrigger>(true);
        DistortedProp selectedProp = layouts[chosenIndex].layoutRoot.GetComponentInChildren<DistortedProp>(true);
        if (selectedPortal != null)
        {
            initializer.Init(selectedPortal);
            selectedPortal.AssignSecretRef(initializer);
            selectedProp.Init();
        }
        else
        {
            Debug.Log("No secret portal found!");
        }
    }
}
