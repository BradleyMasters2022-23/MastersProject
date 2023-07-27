/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 25th, 2022
 * Last Edited - October 25th, 2022 by Ben Schuster
 * Description - Choose items to unpack from a core object. Useful for singleton managers
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UnpackScript : MonoBehaviour
{
    [Tooltip("Whether anything should be deleted on start")]
    [SerializeField] private bool deleteOnStart;

    [Tooltip("What items should be deleted on start. " +
        "Useful for UI elements not supposed to be seen in game")]
    [ShowIf("deleteOnStart")]
    [SerializeField] private GameObject[] itemsToDelete;


    [Tooltip("Whether anything should be deleted on start")]
    [SerializeField] private bool unparentOnStart;

    [ShowIf("unparentOnStart")]
    [Tooltip("Whether anything should be deleted on start")]
    [SerializeField] private bool unparentAll;

    [Tooltip("What items should be unparented on start. " +
        "Useful for managers that need to have no parent")]
    [ShowIf("@this.unparentOnStart && !unparentAll")]
    [SerializeField] private GameObject[] itemsToUnparent;

    [Tooltip("Whether this gameobject should be destroyed after this executes.")]
    [SerializeField] private bool selfDestruct;

    protected virtual void Awake()
    {
        // Delete marked items
        for(int i = itemsToDelete.Length-1; i >= 0; i--)
        {
            Destroy(itemsToDelete[i]);
        }

        itemsToUnparent = new GameObject[transform.childCount];

        for(int i = 0; i < itemsToUnparent.Length; i++)
        {
            itemsToUnparent[i] = transform.GetChild(i).gameObject;
        }

        // Unparent marked items
        for (int i = itemsToUnparent.Length - 1; i >= 0; i--)
        {
            itemsToUnparent[i].transform.SetParent(null, true);
        }

        // Destroy self if set
        if (selfDestruct)
            Destroy(gameObject);
        else
            Destroy(this);

        
    }
}
