/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 18th, 2023
 * Last Edited - July 18th, 2023 by Ben Schuster
 * Description - Save data and functions for the collectable system
 * ================================================================================================
 */
using System.Collections.Generic;
using UnityEngine;
public class CollectableSaveData
{
    /// <summary>
    /// Dictionary containing save data for collectables
    /// -
    /// KEY : ID of the collectable, made by Hashcode of the SO name
    /// -
    /// VALUE : List of collectable IDs, represented as fragment index number
    /// </summary>
    public Dictionary<int, List<int>> savedCollectables;

    /// <summary>
    /// Saved luck that applies to the spawning collectable system
    /// </summary>
    public float savedBonusLuck;
    private const float minimumBonusLuck = 0;
    private const float maximumBonusLuck = 100;

    public CollectableSaveData()
    {
        savedCollectables = new Dictionary<int, List<int>>();
    }

    /// <summary>
    /// Get a reference to all fragments of a collectable that are collected. Returns Null if none
    /// </summary>
    /// <param name="collectable">The collectable to check</param>
    /// <returns>All fragment indexes found. Returns null if none</returns>
    public List<int> GetSavedFragments(CollectableSO collectable)
    {
        int collectableID = collectable.name.GetHashCode();

        if (savedCollectables.ContainsKey(collectableID))
        {
            // Make sure to return a copy of the list so it doesnt get edited
            return new List<int>(savedCollectables[collectableID]);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Save a fragment to the save data
    /// </summary>
    /// <param name="collectable">ID of the collectable the fragment belongs to</param>
    /// <param name="fragmentID">ID of the fragment to save</param>
    public void SaveNewFragment(CollectableSO collectable, int fragmentID)
    {
        int collectableID = collectable.name.GetHashCode();

        // If no data, create it
        if (!savedCollectables.ContainsKey(collectableID))
        {
            savedCollectables.Add(collectableID, new List<int>());
        }
        // Make sure not to save duplicates
        if (!savedCollectables[collectableID].Contains(fragmentID))
        {
            savedCollectables[collectableID].Add(fragmentID);
        }
    }

    /// <summary>
    /// Get the number of fragments found for a specific collectable
    /// </summary>
    /// <param name="collectable">collectable ID to check</param>
    /// <returns>Number of fragments of that collectable found</returns>
    public int GetNumberOfFragmentsFound(CollectableSO collectable)
    {
        int collectableID = collectable.name.GetHashCode();

        if (savedCollectables.ContainsKey(collectableID))
        {
            return savedCollectables[collectableID].Count;
        }
        else
            return 0;
    }

    /// <summary>
    /// Get whether a fragment has been obtained
    /// </summary>
    /// <param name="collectable">ID of the collectable the fragment belongs to</param>
    /// <param name="fragmentID">fragment ID (index) to check</param>
    /// <returns>Whether that collectable's fragment is found</returns>
    public bool FragmentObtained(CollectableSO collectable, int fragmentID)
    {
        int collectableID = collectable.name.GetHashCode();

        // if nothing saved, then not found
        if (!savedCollectables.ContainsKey(collectableID))
            return false;

        // if saved, iterate over its list and check
        for(int i = 0; i < savedCollectables[collectableID].Count; i++)
        {
            if (savedCollectables[collectableID][i] == fragmentID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Increment the bonus luck by a set amount. Clamps to internal values
    /// </summary>
    /// <param name="amt">amount to increase by</param>
    public void AddLuck(float amt)
    {
        savedBonusLuck = Mathf.Clamp(savedBonusLuck + amt, minimumBonusLuck, maximumBonusLuck);
    }
    /// <summary>
    /// Reset the luck to minimum value
    /// </summary>
    public void ResetLuck()
    {
        savedBonusLuck = minimumBonusLuck;
    }
}
