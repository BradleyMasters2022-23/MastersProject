/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 18th, 2023
 * Last Edited - July 18th, 2023 by Ben Schuster
 * Description - Data objects for collectables and their fragments.
 * ================================================================================================
 */
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CollectableFragment
{
    [SerializeField, Tooltip("Prefab of this collectable")]
    private GameObject objectPrefab;

    [SerializeField, Tooltip("Whether to center the interactable prop on spawn." +
        " Useful for fragment prefabs.")] 
    private bool centerPropChild;

    [SerializeField, TextArea, Tooltip("Main text that shows up on pickup")]
    private string text;
    [SerializeField, TextArea, Tooltip("Altenate text that shows up when flipped over")]
    private string altText;

    

    #region Getters

    public GameObject ObjectPrefab
    {
        get { return objectPrefab; }
    }
    public string Text
    {
        get { return text; }
    }
    public string AltText
    {
        get { return altText; }
    }

    public bool CenterPropChild
    {
        get { return centerPropChild; }
    }
    #endregion
}

[CreateAssetMenu(menuName = "Storytelling/Collectable Data", fileName = "New Collectable Item")]
public class CollectableSO : ScriptableObject
{
    [Header("Core")]

    [Tooltip("Name of this collectable. Can be displayed in UI (not implemented)")]
    [SerializeField] string collectableName;
    [Tooltip("Whether this collectable's drop order is ordered by allFragments or random")]
    [SerializeField] bool dropInOrder = false;
    [Tooltip("All fragments of this collectable. ")]
    [SerializeField] CollectableFragment[] allFragments;

    [Header("Scaling")]
    [SerializeField, Tooltip("Scale to apply to prop when in-world pickup")]
    private float propInteractableScaleMod = 1;
    [SerializeField, Tooltip("Scale to apply to prop when viewed in UI")]
    private float propUIScaleMod = 1;
    public float PropInteractableScaleMod
    {
        get { return propInteractableScaleMod; }
    }
    public float PropUIScaleMod
    {
        get { return propUIScaleMod; }
    }

    [Tooltip("Rotation override applied when spawning as a pickup")]
    [SerializeField] Vector3 pickupRotationOverride;
    public Vector3 PickupRotationOverride { get { return pickupRotationOverride; } }

    [Header("Text Settings")]
    [Tooltip("Normal text displayed before any fragment text")]
    [SerializeField] string preTextDesc;
    [Tooltip("Normal text displayed after any fragment text")]
    [SerializeField] string postTextDesc;
    [Tooltip("Text printed when missing a fragment")]
    [SerializeField] string missingFragmentText;

    [Tooltip("Whether this collectable has unique text on flip")]
    [SerializeField] bool flipDescription;
    [Tooltip("Flipped text displayed before any fragment text")]
    [SerializeField] string flippedPreTextDesc;
    [Tooltip("Flipped text displayed after any fragment text")]
    [SerializeField] string flippedPostTextDesc;
    
    public bool FlipDescription {  get { return flipDescription; } }

    /// <summary>
    /// Get the name of this collectable
    /// </summary>
    /// <returns></returns>
    public string Name()
    {
        return collectableName;
    }

    /// <summary>
    /// Get a ref to a concrete collectable
    /// </summary>
    /// <param name="index">Index to use</param>
    /// <returns>Collectable data. If nothing, will return default values</returns>
    public CollectableFragment GetFragment(int index)
    {
        if (index > allFragments.Length || index < 0)
        {
            Debug.LogError($"Collectable Data {collectableName} had a fragment request outside its bounds!" +
                $" It has {allFragments.Length} objects but was asked for {index}");
            return default;
        }
        else
        {
            return allFragments[index];
        }
    }
    
    /// <summary>
    /// Get number of fragments for this collectable
    /// </summary>
    /// <returns></returns>
    public int GetFragmentCount()
    {
        return allFragments.Length;
    }

    /// <summary>
    /// Get description based on target fragments found
    /// </summary>
    /// <param name="front">whether its the front side</param>
    /// <param name="targetFragments">fragments to include</param>
    /// <returns>text description of current collectable</returns>
    public virtual string GetDesc(bool front, List<int> targetFragments)
    {
        // If set to not change description, treat it as front
        if (!flipDescription)
            front = true;

        // prepare pre-text
        string data = ((front) ? preTextDesc : flippedPreTextDesc) + " ";

        // get found fragments, filling in blanks
        for (int i = 0; i < allFragments.Length; i++)
        {
            if(targetFragments.Contains(i))
                data += (front) ? allFragments[i].Text : allFragments[i].AltText;
            else
                data += missingFragmentText;
        }

        // append post-text
        data += " " + ((front) ? postTextDesc : flippedPostTextDesc);

        return data;
    }

    /// <summary>
    /// Get a fragment from this collectable. 
    /// </summary>
    /// <returns>Return index for a random fragment. -1 if nothing available.</returns>
    public int GetRandomFragment(List<int> whitelist = null)
    {
        // if a whitelist is passed in, load that. Otherwise, prepare nothing
        List<int> pool;
        if (whitelist != null)
            pool = new List<int>(whitelist);
        else
            pool = new List<int>();

        List<int> foundFragments = CollectableSaveManager.instance.GetSavedFragments(this);
        // Populate list based on whitelist data
        for(int i = 0; i < allFragments.Length; i++)
        {
            // if a whitelist was loaded, remove anything already saved
            if (whitelist != null)
            {
                if (foundFragments.Contains(i))
                {
                    pool.Remove(i);
                }
            }
            // if a whitelist is not passed in, only add items not already saved
            else
            {
                if (foundFragments == null || !foundFragments.Contains(i))
                {
                    pool.Add(i);
                }
            }
        }

        // exit if nothing is left to choose from
        if (pool.Count <= 0)
            return -1;

        // choose option based on drop setting

        // if dropping in order, return the first option
        // only case this fails if if passed in whitelist is out of order
        if(dropInOrder)
        {
            return pool[0];
        }
        // otherwise, randomly choose
        else
        {
            return pool[Random.Range(0, pool.Count)];
        }
    }
}
