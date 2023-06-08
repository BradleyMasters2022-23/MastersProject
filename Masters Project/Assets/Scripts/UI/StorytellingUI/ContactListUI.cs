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
    private bool init = false;

    [SerializeField] private ContactOptionSO[] contacts;
    [SerializeField] private GameObject contactPrefab;
    [SerializeField] private ScrollRect scrollTrans;
    [SerializeField] private float containerOverflowRatio = 1;
    
    private void OnEnable()
    {
        if (init) return;

        Debug.Log("Contact list initializing");
        PopulateList(contacts);

        init = true;
    }

    /// <summary>
    /// Populate contacts in list, adjust the scroll accordingly
    /// </summary>
    /// <param name="options"></param>
    private void PopulateList(ContactOptionSO[] options)
    {
        if (options.Length <= 0)
            return;

        // Spawn in new objects, load in data 
        foreach (var c in options)
        {
            GameObject newPanel = Instantiate(contactPrefab, scrollTrans.content);
            newPanel.GetComponent<ContactOptionUI>().InitContact(c);
        }

        // Adjust the width of the contact scroll display
        float objWidth = contactPrefab.GetComponent<RectTransform>().sizeDelta.x * options.Length;
        float spaceBuffer = scrollTrans.content.GetComponent<HorizontalLayoutGroup>().spacing * options.Length-1;
        scrollTrans.content.sizeDelta = new Vector2((spaceBuffer+objWidth) * containerOverflowRatio, scrollTrans.content.sizeDelta.y);
        scrollTrans.horizontalScrollbar.value = 0;
    }
}
