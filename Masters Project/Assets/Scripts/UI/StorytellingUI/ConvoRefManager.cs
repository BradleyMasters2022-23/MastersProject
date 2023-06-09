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

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
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
