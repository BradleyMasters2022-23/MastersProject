/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - May 4th, 2023
 * Last Edited - May 4th, 2023 by Ben Schuster
 * Description - Data class tracking generic gameplay data 
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaythroughSaveData
{
    public int runsAttempted = 0;
    public int runsCompleted = 0;
    public int playerDeaths = 0;
    public int crystalsCollected = 0;
    
    public PlaythroughSaveData()
    {
        runsAttempted = 0;
        runsCompleted = 0;
        playerDeaths = 0;
        crystalsCollected = 0;
    }

    /// <summary>
    /// Print data to console. Only use for testing
    /// </summary>
    public void PrintData()
    {
        Debug.Log($"Rus attempted {runsAttempted}");
        Debug.Log($"Runs completed {runsCompleted}");
        Debug.Log($"player deaths {playerDeaths}");
        Debug.Log($"crystals collected {crystalsCollected}");
    }
}
