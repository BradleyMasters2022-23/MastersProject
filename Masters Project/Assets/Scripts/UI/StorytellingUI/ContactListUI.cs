/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 8th, 2023
 * Last Edited - June 8th, 2023 by Ben Schuster
 * Description - Manages list of all contacts in the conversation system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactListUI : MonoBehaviour
{
    /// <summary>
    /// Whether or not this UI has been initialized
    /// </summary>
    private bool init = false;

    [Tooltip("All contacts to load")]
    [SerializeField] private ContactOptionSO[] contacts;
    [Tooltip("Prefab for each individual contact option")]
    [SerializeField] private GameObject contactPrefab;
    [Tooltip("How much spacing on the sides to apply to the contact prefabs")]
    [SerializeField] private float contactHorizontalSpacing;
    [Tooltip("Scroll manager for the menu")]
    [SerializeField] private ScrollRect scrollTrans;
    [Tooltip("How overflow should be added to the content scroll")]
    [SerializeField] private float containerOverflowRatio = 1;
    
    private void OnEnable()
    {
        // reset scroll bar
        //scrollTrans.verticalScrollbar.value = 1;

        if (init) // Only init once when opened, though do check incoming calls
        {
            CheckForCalls();
            return;
        }

        Debug.Log("Contact list initializing");
        PopulateList(contacts);

        init = true;
    }

    /// <summary>
    /// Populate contacts in list, adjust the scroll accordingly
    /// </summary>
    /// <param name="options">All contacts to load into the list</param>
    private void PopulateList(ContactOptionSO[] options)
    {
        // only populate if theres something to actually... well populate
        if (options.Length <= 0)
            return;

        // Adjust children to scale width of the content bar, keeping height
        Vector2 spareBuffer = 
            new Vector2(scrollTrans.content.sizeDelta.x - (contactHorizontalSpacing*2), 
            contactPrefab.GetComponent<RectTransform>().sizeDelta.y);

        int optionsLoaded = 0;
        // Spawn in new objects, load in data 
        foreach (var c in options)
        {
            // If they dont have their first call available, don't load it
            if (!(CallManager.instance.HasNewCall(c.allConversations[0]) || CallManager.instance.CallInSave(c.allConversations[0])))
            {
                continue;
            }

            GameObject newPanel = Instantiate(contactPrefab, scrollTrans.content);
            newPanel.GetComponent<ContactOptionUI>().InitContact(c); // init call will automatically check for calls
            //newPanel.GetComponent<RectTransform>().sizeDelta = spareBuffer;
            optionsLoaded++;
        }

        // Calculate the new spacing to fit all objects while maintaining spacing, add extra overflow
        float objHeight = contactPrefab.GetComponent<RectTransform>().sizeDelta.y * optionsLoaded;
        VerticalLayoutGroup layoutGp = scrollTrans.content.GetComponent<VerticalLayoutGroup>();
        float spaceBuffer = layoutGp.spacing * optionsLoaded - 1;
        spaceBuffer += layoutGp.padding.top + layoutGp.padding.bottom;
        float newSpacing = (spaceBuffer + objHeight) * containerOverflowRatio;

        // Only apply new size if its greater than the default size. Reset scroll bar
        newSpacing = (newSpacing > scrollTrans.content.sizeDelta.y) ? newSpacing : scrollTrans.content.sizeDelta.y;
        spareBuffer.x = scrollTrans.content.sizeDelta.x;
        spareBuffer.y = newSpacing;
        scrollTrans.content.sizeDelta = spareBuffer;
        StartCoroutine(SetScrollTop());
    }

    private IEnumerator SetScrollTop()
    {
        yield return new WaitUntil(() => scrollTrans.verticalScrollbar.value != 1);
        yield return new WaitForEndOfFrame();
        scrollTrans.verticalScrollbar.value = 1;
    }

    /// <summary>
    /// Check all spawned objects for a call
    /// </summary>
    private void CheckForCalls()
    {
        // Tell each option loaded to check for a new call
        for (int i = 0; i < scrollTrans.content.childCount; i++)
        {
            scrollTrans.content.GetChild(i).GetComponent<ContactOptionUI>()?.GetIncomingCall();
        }
    }
}
