/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 15th, 2023
 * Last Edited - June 15th, 2023 by Ben Schuster
 * Description - Single collectable instance that loads in-world objects 
 * based on completion of a collectable
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCollectableInstance : MonoBehaviour, Interactable
{
    [Tooltip("The collectable data this instance represents")]
    [SerializeField] CollectableSO collectableData;

    [Tooltip("all props for this collectable. Organize the same way" +
        "the fragments are listed in the SO.")]
    [SerializeField] GameObject[] fragmentProps;

    private void Start()
    {
        SyncCollectableData();
    }

    private void SyncCollectableData()
    {
        if (collectableData == null)
        {
            Debug.LogError($"Collectable named {name} does not have its data loaded in! Self destructing!");
            Destroy(gameObject);
            return;
        }

        // initialize any props that are found
        List<int> collectedFragments = CollectableSaveManager.instance.GetSavedFragments(collectableData);
        // if anything is loaded, update each fragment based on their correlation
        if(collectedFragments != null)
        {
            for (int i = 0; i < fragmentProps.Length; i++)
            {
                fragmentProps[i].SetActive(collectedFragments.Contains(i));
            }
        }
        // otherwise if nothing is found, disable itself entirely
        else
        {
            for (int i = 0; i < fragmentProps.Length; i++)
            {
                fragmentProps[i].SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public void OnInteract()
    {
        List<CollectableFragment> collectedFrags = new List<CollectableFragment>();
        for(int i = 0; i < fragmentProps.Length; i++)
        {
            if (fragmentProps[i].activeInHierarchy)
                collectedFrags.Add(collectableData.GetFragment(i));
        }
        PlayerTarget.p.GetComponentInChildren<CollectableViewUI>(true).OpenUI(collectedFrags);
    }
}
