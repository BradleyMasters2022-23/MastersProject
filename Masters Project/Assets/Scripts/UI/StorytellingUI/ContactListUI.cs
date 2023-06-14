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
    [Tooltip("Prefab for each individual contact option")]
    [SerializeField] private GameObject contactPrefab;
    [Tooltip("Scroll manager for the menu")]
    [SerializeField] private ScrollRect scrollTrans;
    [Tooltip("How overflow should be added to the content scroll")]
    [SerializeField] private float containerOverflowRatio = 1;

    private void OnEnable()
    {
        PopulateList(CallManager.instance.characters.ToArray());
    }
    private void OnDisable()
    {
        ResetCallLog();
    }

    /// <summary>
    /// Populate contacts in list, adjust the scroll accordingly
    /// </summary>
    /// <param name="options">All contacts to load into the list</param>
    private void PopulateList(Character[] options)
    {
        // only populate if theres something to actually... well populate
        if (options.Length <= 0)
            return;

        // Adjust children to scale width of the content bar, keeping height
        int optionsLoaded = 0;
        // Spawn in new objects, load in data 
        foreach (var c in options)
        {
            // If they dont have their first call available, don't load it
            if (!c.CharacterMet(CallManager.instance))
            {
                continue;
            }

            GameObject newPanel = Instantiate(contactPrefab, scrollTrans.content);
            newPanel.GetComponent<ContactOptionUI>().InitContact(c); // init call will automatically check for calls
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
        Vector2 sizeBuffer = new Vector2(scrollTrans.content.sizeDelta.x, newSpacing);
        scrollTrans.content.sizeDelta = sizeBuffer;
        StartCoroutine(SetScrollTop());
    }

    private IEnumerator SetScrollTop()
    {
        yield return new WaitUntil(() => scrollTrans.verticalScrollbar.value != 1);
        //yield return new WaitForEndOfFrame();
        scrollTrans.verticalScrollbar.value = 1;
    }

    /// <summary>
    /// Check all spawned objects for a call
    /// </summary>
    public void CheckForCalls()
    {
        // Tell each option loaded to check for a new call
        for (int i = 0; i < scrollTrans.content.childCount; i++)
        {
            scrollTrans.content.GetChild(i).GetComponent<ContactOptionUI>()?.CheckStatus();
        }
    }

    private void ResetCallLog()
    {
        // clear all logs so they can be properly reinitialized next time
        for (int i = scrollTrans.content.childCount-1; i >= 0 ; i--)
        {
            Destroy(scrollTrans.content.GetChild(i).gameObject, 0.1f);
        }
    }
}
