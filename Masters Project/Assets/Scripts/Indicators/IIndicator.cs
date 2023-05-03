/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22nd, 2022
 * Last Edited - February 22nd, 2022 by Ben Schuster
 * Description - Interface for any indicator type object to make them easier to work with
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IIndicator : MonoBehaviour
{ 
    public abstract void Activate();
    public abstract void Deactivate();
}

public class Indicators
{
    /// <summary>
    /// Set all indicators in an array to on or off
    /// </summary>
    /// <param name="indicators">Indicators to change status of</param>
    /// <param name="enabled">Whether to turn them on</param>
    public static void SetIndicators(IIndicator[] indicators, bool enabled)
    {
        foreach(IIndicator indicator in indicators)
        {
            if (indicator == null)
                continue;

            if(enabled)
                indicator.Activate();
            else
                indicator.Deactivate();
        }
    }
}
