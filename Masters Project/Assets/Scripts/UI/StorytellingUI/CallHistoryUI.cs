using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallHistoryUI : MonoBehaviour
{
    [SerializeField] private ContactOptionUI dataManager;

    public void OpenContactHistory(Character data)
    {
        gameObject.SetActive(true);
        dataManager.InitContact(data);
    }

    public void CheckStatus()
    {
        // Debug.Log("Checking status of main call log");
        dataManager.CheckStatus();
    }
}
