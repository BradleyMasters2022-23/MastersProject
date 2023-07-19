using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class LoadCollectedUpgradesUI : MonoBehaviour
{
    private CrystalManager source;

    [SerializeField] private CrystalUIDisplay[] options;

    [SerializeField] private AllStatsDisplay allStatsDisplay;

    ConfirmationBox deleteConfirm;

    int targetIndex = -1;

    private void OnEnable()
    {
        source = CrystalManager.instance;

        PopulateList();
    }


    private void PopulateList()
    {
        if(source == null)
        {
            OnDisable();
            return;
        }

        // Load in UI for each upgrade
        for(int i = 0; i < source.MaxSlots(); i++)
        {
            if(i >= options.Length)
            {
                Debug.LogError("Trying to load too many options!");
                return;
            }

            options[i].LoadCrystal(null, source.GetEquippedCrystal(i), i);
            options[i].gameObject.SetActive(true);
        }

        // Calculate all stats
        allStatsDisplay.LoadEquippedStats();
    }

    private void OnDisable()
    {
        // Unload all stats 
        for (int i = 0; i < options.Length; i++)
        {
            options[i].LoadEmpty();
            options[i].gameObject.SetActive(false);
        }

        // Clear display
        allStatsDisplay.Clear();
    }

    /// <summary>
    /// Request to trash an upgrade.
    /// </summary>
    /// <param name="i">target index to trash</param>
    public void RequestTrash(int i)
    {
        if(deleteConfirm == null)
            deleteConfirm = FindObjectOfType<ConfirmationBox>(true);
        if (deleteConfirm == null)
            return;
        targetIndex = i;
        string txt = $"Discard <b>{source.GetEquippedCrystal(targetIndex).crystalName}?</b>";
        deleteConfirm.RequestConfirmation(TrashCrystal, txt);
    }
    /// <summary>
    /// Trash the last targeted crystal, update UI accordingly
    /// </summary>
    private void TrashCrystal()
    {
        if (targetIndex >= 0 && source.CrystalEquipped(targetIndex))
        {
            CrystalManager.instance.LoadCrystal(null, targetIndex);
            options[targetIndex].LoadEmpty();
            allStatsDisplay.LoadEquippedStats();
        }
    }
}
