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
        while(contactListUI == null || callLogUI == null || callUI == null)
        {
            if(contactListUI == null)
                contactListUI = PlayerTarget.p.GetComponentInChildren<ContactListUI>();
            if(callLogUI == null)
                callLogUI = PlayerTarget.p.GetComponentInChildren<CallHistoryUI>();
            if(callUI== null)
                callUI = PlayerTarget.p.GetComponentInChildren<DisplayDialogueUI>();

            yield return new WaitForSecondsRealtime(1);
        }
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
}
