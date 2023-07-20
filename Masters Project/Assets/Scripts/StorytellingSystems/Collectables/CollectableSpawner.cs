/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 18th, 2023
 * Last Edited - July 18th, 2023 by Ben Schuster
 * Description - Spawner container for collectables. Manages choosing objects to spawn
 * and what happens on interact.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CollectableSpawner : MonoBehaviour, Interactable
{
    [Header("Spawning")]
    [Tooltip("Whether to randomize this spawn chance")]
    [SerializeField] bool randomized;
    [Tooltip("Chance of this one spawning. Think % based"), HideIf("@!this.randomized")]
    [SerializeField, Range(0, 100)] float baseSpawnChance;

    [Header("Collectable Settings")]
    [Tooltip("Manual override for a collectable")]
    [SerializeField] CollectableSO collectableOverride;
    [HideIf("@this.collectableOverride != null")]
    [SerializeField] bool utilizeCollectableWhitelist;
    [HideIf("@this.collectableOverride != null || !this.utilizeCollectableWhitelist")]
    [SerializeField] List<CollectableSO> collectableWhitelist;

    [Header("Fragment Settings")]
    [Tooltip("Specific fragment override for this spawn. Set to -1 to keep it randomized.")]
    [HideIf("@this.collectableOverride == null")]
    [SerializeField] int fragmentOverrideIndex = -1;
    [Tooltip("Whether to add a whitelist or exception list. Does not work if fragment override is set.")]
    [HideIf("@this.collectableOverride == null || this.fragmentOverrideIndex > -1")]
    [SerializeField] bool utilizeFragmentWhitelist;
    [HideIf("@this.utilizeFragmentWhitelist != true || this.collectableOverride == null || this.fragmentOverrideIndex > -1")]
    [SerializeField] List<int> fragmentWhitelist;
    
    [Header("Debug")]
    /// <summary>
    /// The chosen collectable this interactable uses
    /// </summary>
    [SerializeField, ReadOnly] private CollectableSO chosenCollectable;
    /// <summary>
    /// The chosen fragment this interactable rewards
    /// </summary>
    [SerializeField, ReadOnly] private int chosenFragmentIndex;
    private CollectableFragment loadedFragment;

    private bool collected;

    private void Start()
    {
        SpawnCollectable();
    }

    /// <summary>
    /// Spawn this collectable. Is success, will initialize itself.
    /// </summary>
    public void SpawnCollectable()
    {
        if(randomized)
        {
            if (!CollectableSaveManager.instance.ShouldSpawn(baseSpawnChance))
            {
                Debug.Log("Failed spawn chance");
                DestroyCollectable();
                return;
            }
        }

        DetermineSelection();
    }

    /// <summary>
    /// Determine what collectable and fragment to utilize
    /// </summary>
    public void DetermineSelection()
    {
        // Get collectable object

        // If there are overrides, process them
        if (collectableOverride != null) 
        {
            chosenCollectable = collectableOverride;
        }
        // if a whitelist is set, randomly select from it
        else if (utilizeCollectableWhitelist && collectableWhitelist.Count > 0)
        {
            // load whitelist, validate it
            List<CollectableSO> tempList= new List<CollectableSO>(collectableWhitelist);
            tempList = CollectableSaveManager.instance.ValidateWhitelist(tempList);

            // randomly choose if one is avaiable
            if(tempList.Count > 0)
                chosenCollectable= tempList[Random.Range(0, tempList.Count)];
        }
        // if no override or whitelist, get one random avaialble option
        else 
        {
            chosenCollectable = CollectableSaveManager.instance.GetRandomCollectable();
        }

        // Self destruct if nothing available
        if (chosenCollectable == null)
        {
            DestroyCollectable();
            return;
        }

        // Get collectable fragment
        chosenFragmentIndex = -1;

        // if override is valid, use that instead
        if (fragmentOverrideIndex >= 0 && collectableOverride != null &&
            fragmentOverrideIndex < chosenCollectable.GetFragmentCount()) 
        {
            if(!CollectableSaveManager.instance.FragmentObtained(collectableOverride, fragmentOverrideIndex))
                chosenFragmentIndex = fragmentOverrideIndex;
        }
        // if a whitelist is set, get random fragment with whitelist
        else if(collectableOverride != null && utilizeFragmentWhitelist
            && fragmentWhitelist.Count > 0)
        {
            chosenFragmentIndex = chosenCollectable.GetRandomFragment(fragmentWhitelist);
        }
        // otherwise, request new fragment
        else
        {
            chosenFragmentIndex = chosenCollectable.GetRandomFragment();
        }

        // if still its original value, self destruct
        if(chosenFragmentIndex == -1)
        {
            DestroyCollectable();
            return;
        }

        collected = false;

        // load prepared fragment
        loadedFragment = chosenCollectable.GetFragment(chosenFragmentIndex);
        if(loadedFragment.ObjectPrefab != null)
        {
            // spawn as child, apply new scale and reposition
            GameObject prop = Instantiate(loadedFragment.ObjectPrefab, transform);
            prop.transform.localScale *= chosenCollectable.PropInteractableScaleMod;
            prop.transform.localRotation = Quaternion.Euler(chosenCollectable.PickupRotationOverride);
            if (loadedFragment.CenterPropChild)
                prop.transform.GetChild(0).localPosition = Vector3.zero;

            // make sure all colliders are disabled for pickup
            Collider[] cols = prop.GetComponentsInChildren<Collider>();
            foreach (Collider c in cols)
                c.enabled = false;
        }
    }

    private void OnValidate()
    {
        if(collectableOverride != null)
        {
            fragmentOverrideIndex = Mathf.Clamp(fragmentOverrideIndex, -1, collectableOverride.GetFragmentCount());
            for(int i = 0; i < fragmentWhitelist.Count; i++)
            {
                fragmentWhitelist[i] = Mathf.Clamp(fragmentWhitelist[i], 0, collectableOverride.GetFragmentCount());
            }
        }
    }

    /// <summary>
    /// Destroy this collectable container
    /// </summary>
    private void DestroyCollectable()
    {
        //Debug.Log("Collectable destoryed");
        Destroy(gameObject);
    }

    #region Interaction 

    public void OnInteract()
    {
        // TODO - zoom to player instead of just deleting
        collected = true;
        CollectableSaveManager.instance.SaveNewFragment(chosenCollectable, chosenFragmentIndex);
        
        CollectableViewUI ui = FindObjectOfType<CollectableViewUI>(true);
        ui.OpenUI(chosenCollectable, chosenFragmentIndex);

        DestroyCollectable();
    }

    public bool CanInteract()
    {
        return (chosenCollectable != null && chosenFragmentIndex >= 0 && !collected);
    }

    #endregion
}
