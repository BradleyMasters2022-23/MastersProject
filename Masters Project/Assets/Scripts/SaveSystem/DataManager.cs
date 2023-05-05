/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - May 4th, 2023
 * Last Edited - May 4th, 2023 by Ben Schuster
 * Description - Global manager that handles saving and retrieving data
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class DataManager : MonoBehaviour
{
    /// <summary>
    /// Global data manager instance
    /// </summary>
    public static DataManager instance;

    /// <summary>
    /// The system that actually writes and reads data
    /// </summary>
    private IDataService saver;
    /// <summary>
    /// Path to the save folder.
    /// </summary>
    private const string saveFolderPath = "/SaveData/";
    /// <summary>
    /// Extension used for saving.
    /// </summary>
    private const string extension = ".json";


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            saver = new JsonDataService();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Check if a folder exists. If not, create a new save data folder
        if (!Directory.Exists(Application.persistentDataPath + saveFolderPath))
        {
            Debug.Log("No save folder detected, creating one now");
            Directory.CreateDirectory(Application.persistentDataPath + saveFolderPath);
        }
    }

    /// <summary>
    /// Request for an object to be saved
    /// </summary>
    /// <typeparam name="T">Type of object being saved</typeparam>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="data">Data to save</param>
    /// <returns>Whether saving was successful</returns>
    public bool Save<T>(string fileName, T data)
    {
        return saver.Save(Application.persistentDataPath + saveFolderPath + fileName + extension, data);
    }
    
    /// <summary>
    /// Request data from a saved object
    /// </summary>
    /// <typeparam name="T">Type of object being requested</typeparam>
    /// <param name="fileName">Name of the file. Name only</param>
    /// <returns>The data requested. Returns null if it doesn't exist</returns>
    public T Load<T>(string fileName)
    {
        return saver.Load<T>(Application.persistentDataPath + saveFolderPath + fileName + extension);
    }

    /// <summary>
    /// Delete the data at a filepath
    /// </summary>
    /// <param name="fileName">name of file to delete</param>
    public void Delete(string fileName)
    {
        saver.Delete(Application.persistentDataPath + saveFolderPath + fileName + extension);
    }

    /// <summary>
    /// Clear all save data
    /// SHOULD ONLY BE CALLED BY ONE THING
    /// </summary>
    public void ClearSaveData()
    {
        // If the path exists, delete it with all data
        if (Directory.Exists(Application.persistentDataPath + saveFolderPath))
        {
            Directory.Delete(Application.persistentDataPath + saveFolderPath, true);
        }

        // Recreate the folder so it still exists 
        Directory.CreateDirectory(Application.persistentDataPath + saveFolderPath);
    }

}
