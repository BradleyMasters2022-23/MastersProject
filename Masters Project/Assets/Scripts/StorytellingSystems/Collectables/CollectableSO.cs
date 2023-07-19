/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 18th, 2023
 * Last Edited - July 18th, 2023 by Ben Schuster
 * Description - Main data object for fragments.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct CollectableFragment
{
    [SerializeField, Tooltip("Main image that shows up on pickup")]
    private Sprite image;
    [SerializeField, Tooltip("Opposite side of the image used when flipped")]
    private Sprite altImage;
    [SerializeField, TextArea, Tooltip("Main image that shows up on pickup")]
    private string text;
    private string altText;
    [SerializeField, Tooltip("Prefab of how this object is represented as a pickup")]
    private GameObject interactableGameObjectProp;
    [Tooltip("Scale to apply to the gameobject when it spawns as a pickup")]
    public float objectPropScaleMultiplier;

    /// <summary>
    /// Get main image for this fragment
    /// </summary>
    /// <returns></returns>
    public Sprite Sprite()
    {
        return image;
    }
    /// <summary>
    /// Get alt image for this fragment
    /// </summary>
    /// <returns></returns>
    public Sprite AltSprite()
    {
        return altImage;
    }
    /// <summary>
    /// Get text description for this item
    /// </summary>
    /// <returns></returns>
    public string Text()
    {
        return text;
    }
    public string AltText()
    {
        return altText;
    }
    /// <summary>
    /// Get reference to prefab of the spawn prop
    /// </summary>
    /// <returns></returns>
    public GameObject GetSpawnProp()
    {
        return interactableGameObjectProp;
    }
}

[CreateAssetMenu(menuName = "Storytelling/Collectable Data", fileName = "New Collectable Item")]
public class CollectableSO : ScriptableObject
{
    [SerializeField] string collectableName;
    [SerializeField] CollectableFragment[] allFragments;

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
    /// Get the description of this collectable. 
    /// </summary>
    /// <returns>The combined description of items</returns>
    public virtual string GetDesc()
    {
        string data = "";
        foreach(CollectableFragment fragment in allFragments)
        {
            // TODO - check against save data
            data += fragment.Text() + " ";
        }
        return data;
    }
    /// <summary>
    /// Get all images from this collectable
    /// </summary>
    /// <returns>All images found [NOT DONE, RETURNS ALL]</returns>
    public Sprite[] GetImages()
    {
        Sprite[] foundImgs = new Sprite[allFragments.Length];
        for(int i = 0; i < allFragments.Length; i++)
        {
            foundImgs[i] = allFragments[i].Sprite();
        }
        return foundImgs;
    }
    /// <summary>
    /// Get all alt images from this collectable
    /// </summary>
    /// <returns>All alt images found [NOT DONE, RETURNS ALL]</returns>
    public Sprite[] GetAltTextures()
    {
        Sprite[] foundImgs = new Sprite[allFragments.Length];
        for (int i = 0; i < allFragments.Length; i++)
        {
            foundImgs[i] = allFragments[i].AltSprite();
        }
        return foundImgs;
    }
}
