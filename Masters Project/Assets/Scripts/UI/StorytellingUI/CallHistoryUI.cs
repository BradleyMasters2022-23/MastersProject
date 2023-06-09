using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallHistoryUI : MonoBehaviour
{
    private ContactOptionSO loadedCharcter;
    [SerializeField] private ContactOptionUI dataManager;

    public void OpenContactHistory(ContactOptionSO data)
    {
        loadedCharcter = data;
        gameObject.SetActive(true);
        dataManager.InitContact(data);
    }
}
