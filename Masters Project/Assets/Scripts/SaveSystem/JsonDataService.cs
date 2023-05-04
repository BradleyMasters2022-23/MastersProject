using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class JsonDataService : IDataService
{
    /// <summary>
    /// Try to save data to a given path
    /// </summary>
    /// <typeparam name="T">Type of object to save</typeparam>
    /// <param name="path">File path. Must include directory, name, and file extension</param>
    /// <param name="data">Data to save</param>
    /// <returns>Whether or not saving was successful</returns>
    public bool Save<T>(string path, T data)
    {
        // delete old file if it already exists
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        // Try writing new data to path using JsonUtility
        try
        {
            FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonUtility.ToJson(data));
            return true;
        }
        catch(Exception e)
        {
            Debug.Log($"Unable to save data due to {e.Message} {e.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Try to retrieved saved data at a given path
    /// </summary>
    /// <typeparam name="T">Type of object to load</typeparam>
    /// <param name="path">File path. Must include directory, name, and file extension</param>
    /// <returns>Loaded data. Returns default if not found</returns>
    public T Load<T>(string path)
    {
        // If it exists, retrieve it and convert from Json
        if(File.Exists(path))
        {
            try
            {
                string s = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(s);
            }
            catch (Exception e)
            {
                Debug.Log($"Unable to save data due to {e.Message} {e.StackTrace}");
                return default;
            }
        }
        // Otherwise, return default
        else
        {
            Debug.Log("No file found to load");
            return default;
        }
    }

    /// <summary>
    /// Delete a file at a given path
    /// </summary>
    /// <param name="path">File path. Must include directory, name, and file extension</param>
    public void Delete(string path)
    {
        // Try deleting file if it exists
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch(Exception e)
            {
                Debug.Log($"Unable to delete data due to {e.Message} {e.StackTrace}");
            }
        }
        else
        {
            Debug.Log($"File at path {path} not found");
        }
    }
}
