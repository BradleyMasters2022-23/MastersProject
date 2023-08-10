using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSaveButton : MonoBehaviour
{
    [SerializeField] GameObject[] saveDataOptions;
    [SerializeField] GameObject[] noSaveDataOptions;

    private void Start()
    {
        // check if theres any save data. This data should ALWAYS exist
        if (GlobalStatsManager.data != null && GlobalStatsManager.data.convoTicks > 0)
            SetSaveData();
        else
            SetNoSaveData();
    }

    private void SetSaveData()
    {
        foreach (var o in saveDataOptions)
            o.SetActive(true);

        foreach (var o in noSaveDataOptions)
            o.SetActive(false);
    }
    private void SetNoSaveData()
    {
        foreach (var o in saveDataOptions)
            o.SetActive(false);

        foreach (var o in noSaveDataOptions)
            o.SetActive(true);
    }


    public void RequestClearData()
    {
        ConfirmationBox b = FindObjectOfType<ConfirmationBox>(true);

        if(b != null)
            b.RequestConfirmation(ClearData, "Clear save data? This cannot be undone");
        else
            ClearData();
    }

    private void ClearData()
    {
        bool success = DataManager.instance.ClearSaveData();
        
        if(success)
            SetNoSaveData();
    }
}
