/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 9th, 2023
 * Last Edited - June 9th, 2023 by Ben Schuster
 * Description - Reference manager for all screens. Used due to variable number of prefabs
 * that cannot otherwise get a direct reference for buttons
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoRefManager : MonoBehaviour
{
    public static ConvoRefManager instance;
    
    [SerializeField] private ContactListUI contactListUI;
    [SerializeField] private CallHistoryUI callLogUI;
    [SerializeField] private DisplayDialogueUI callUI;

    public void Start()
    {
        if(instance == null)
        {
            instance = this;

            StartCoroutine(Init());
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// While any of the three screens are uninitialized, continually try to get them
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        if (contactListUI == null)
            contactListUI = PlayerTarget.p.GetComponentInChildren<ContactListUI>();
        if (callLogUI == null)
            callLogUI = PlayerTarget.p.GetComponentInChildren<CallHistoryUI>();
        if (callUI == null)
            callUI = PlayerTarget.p.GetComponentInChildren<DisplayDialogueUI>();
        yield return null;
    }

    public ContactListUI GetContactList()
    {
        return contactListUI;
    }
    public CallHistoryUI GetCallHistoryUI()
    {
        return callLogUI;
    }
    public DisplayDialogueUI GetCallUI()
    {
        return callUI;
    }

    public void OpenScreen()
    {
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        contactListUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// Try closing all of the menus in correct order
    /// </summary>
    public void CloseAllScreens()
    {
        if(callUI != null && callUI.isActiveAndEnabled)
            callUI.GetComponent<GameObjectMenu>().CloseButton();
        if (callLogUI != null && callLogUI.isActiveAndEnabled)
            callLogUI.GetComponent<GameObjectMenu>().CloseButton();
        if (contactListUI != null && contactListUI.isActiveAndEnabled)
            contactListUI.GetComponent<GameObjectMenu>().CloseButton();
    }

    public void CheckCallStatuses()
    {
        // Tell main system to check its status
        if (callLogUI != null)
            callLogUI.CheckStatus();

        // Tell each contact list to update its call status incase it was read
        if (contactListUI != null)
            contactListUI.CheckForCalls();
    }
}
