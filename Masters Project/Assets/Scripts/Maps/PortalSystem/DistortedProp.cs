/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 2nd, 2022
 * Last Edited - June 2nd, 2022 by Ben Schuster
 * Description - A corrupted prop that can be interacted with
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortedProp : MonoBehaviour
{
    /// <summary>
    /// VFX that plays when the prop is primed
    /// </summary>
    [SerializeField] private IIndicator[] primedIndicators;
    /// <summary>
    /// VFX that plays when the prop is interactable 
    /// </summary>
    [SerializeField] private IIndicator[] interactableIndicators;

    private bool distorted = true;

    /// <summary>
    /// On enable, set distored and subscribe
    /// </summary>
    public void Init()
    {
        if(distorted)
        {
            Debug.Log("Initializing distorted prop");
            Indicators.SetIndicators(primedIndicators, true);
            MapLoader.instance.RegisterOnEncounterComplete(EnableInteraction);
        }
    }

    /// <summary>
    /// Enable the interaction with the prop
    /// </summary>
    public void EnableInteraction()
    {
        Indicators.SetIndicators(interactableIndicators, true);
    }

    /// <summary>
    /// Clear all of the distortion effects of this prop
    /// </summary>
    public void ClearDistortion()
    {
        distorted = false;
        Indicators.SetIndicators(primedIndicators, false);
        Indicators.SetIndicators(interactableIndicators, false);
    }
}
