using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallHistoryUI : MonoBehaviour
{
    [SerializeField] private ContactOptionSO loadedCharcter;
    [SerializeField] private ContactOptionUI dataManager;

    private void OnEnable()
    {
        OpenContactHistory(loadedCharcter);
    }

    public void OpenContactHistory(ContactOptionSO data)
    {
        gameObject.SetActive(true);
        dataManager.InitContact(data);
    }
}
