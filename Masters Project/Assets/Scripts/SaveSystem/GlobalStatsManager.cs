/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - May 5th, 2023
 * Last Edited - May 5th, 2023 by Ben Schuster
 * Description - Global manager that tracks generic global data 
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStatsManager : MonoBehaviour
{
    public static GlobalStatsManager Instance;
    public static PlaythroughSaveData data;
    private const string fileName = "globalStats";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Load or initialize data 
    /// </summary>
    private void Start()
    {
        data = DataManager.instance.Load<PlaythroughSaveData>(fileName);
        if(data == null)
            data = new PlaythroughSaveData();

        // only print for testing reaons
        //data?.PrintData();
    }

    /// <summary>
    /// Attempt to save data 
    /// </summary>
    public static void SaveData()
    {
        // only save if non default values
        if(data.DataChanged())
            DataManager.instance.Save<PlaythroughSaveData>(fileName, data);
        //Debug.Log($"global stats save successful: {s}");

        // only print for testing reasons
        //data?.PrintData();
    }

    /// <summary>
    /// Try to save data when the game quits
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveData();
    }


    /// <summary>
    /// Reset the data with a new object. Do this when data gets cleared
    /// </summary>
    public void ResetData()
    {
        data = new PlaythroughSaveData();
        //Debug.Log("Global data reset");
    }
}
