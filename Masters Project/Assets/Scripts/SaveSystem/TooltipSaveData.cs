/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 15th, 2023
 * Last Edited - June 15th, 2023 by Ben Schuster
 * Description - Save data and functions for the tooltip system
 * ================================================================================================
 */
using System.Collections.Generic;

public class TooltipSaveData
{
    /// <summary>
    /// Dictionary containing save data for tooltip
    /// -
    /// KEY : ID of the tooltip as a hashcode of tooltip's titleText
    /// -
    /// VALUE : Number of times the tooltip has been displayed
    /// </summary>
    public Dictionary<int, int> savedTooltips;

    public TooltipSaveData() 
    {
        savedTooltips= new Dictionary<int, int>();
    }

    /// <summary>
    /// Whether this tooltip has been acquired 
    /// </summary>
    /// <param name="tp">Tooltip to check</param>
    /// <returns>Whether its been acquired</returns>
    public bool HasTooltip(TooltipSO tp)
    {
        return savedTooltips.ContainsKey(tp.titleText.GetHashCode());
    }

    /// <summary>
    /// Check if a tooltip's display limit has been reached
    /// </summary>
    /// <param name="tp">Tooltip to check</param>
    /// <returns>Whether its been reached</returns>
    public bool LimitReached(TooltipSO tp)
    {
        int id = tp.titleText.GetHashCode();
        if (savedTooltips.ContainsKey(id))
        {
            // If recorded, check its value vs the TP's limit 
            return savedTooltips[id] >= tp.timesToDisplay;

        }
        else // If it hasen't been found, then no limit reached
        {
            return false;
        }
    }
    /// <summary>
    /// Increment tooltip's occurance count to store
    /// </summary>
    /// <param name="tp">Tooltip to increment</param>
    public void IncrementTooltip(TooltipSO tp)
    {
        // If it doesn't have one, add it
        if(!HasTooltip(tp))
        {
            savedTooltips.Add(tp.titleText.GetHashCode(), 0);
        }
        // Increment it once
        savedTooltips[tp.titleText.GetHashCode()]++;
    }
}
