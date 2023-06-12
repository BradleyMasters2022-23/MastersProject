using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallHistoryUI : MonoBehaviour
{
    private Character loadedCharacter;
    [SerializeField] private ContactOptionUI dataManager;

    public void OpenContactHistory(Character data)
    {
        loadedCharacter = data;
        gameObject.SetActive(true);
        dataManager.InitContact(data);
    }
}
