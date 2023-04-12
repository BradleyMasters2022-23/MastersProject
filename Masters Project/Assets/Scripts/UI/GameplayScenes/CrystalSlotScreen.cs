using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrystalSlotScreen : MonoBehaviour
{
    private CrystalInteract caller;
    private Crystal selectedCrystal;
    private int selectedCrystalIndex = 0;

    [SerializeField] private TextMeshProUGUI newCrystalName;
    [SerializeField] private TextMeshProUGUI newCrystalStats;

    [SerializeField] private TextMeshProUGUI selectedCrystalName;
    [SerializeField] private TextMeshProUGUI selectedCrystalStats;

    [SerializeField] private Image equippedCrystal1;
    [SerializeField] private Image equippedCrystal2;
    [SerializeField] private Image equippedCrystal3;

    /// <summary>
    /// Open the screen, change state
    /// </summary>
    public void OpenScreen(CrystalInteract c)
    {
        caller = c;
        if(CrystalManager.instance.CrystalEquipped(selectedCrystalIndex))
        {
            selectedCrystal = CrystalManager.instance.GetEquippedCrystal(selectedCrystalIndex);
            DisplaySelectedCrystal();
        }
        //DisplayCrystalIcons();
        DisplayNewCrystalStats();

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
    }

    private void DisplayCrystalIcons()
    {
        if(CrystalManager.instance.CrystalEquipped(0))
        {
            equippedCrystal1 = CrystalManager.instance.GetEquippedCrystal(0).icon;
        } else
        {
            equippedCrystal1.enabled = false;
        }

        if (CrystalManager.instance.CrystalEquipped(1))
        {
            equippedCrystal2 = CrystalManager.instance.GetEquippedCrystal(1).icon;
        }
        else
        {
            equippedCrystal2.enabled = false;
        }

        if (CrystalManager.instance.CrystalEquipped(2))
        {
            equippedCrystal3 = CrystalManager.instance.GetEquippedCrystal(2).icon;
        }
        else
        {
            equippedCrystal3.enabled = false;
        }
    }

    private void DisplayNewCrystalStats()
    {
        newCrystalName.text = caller.GetCrystal().crystalName;
        int statIndex = 0;
        int mod;
        
        foreach(IStat stat in caller.GetCrystal().stats)
        {
            mod = caller.GetCrystal().mods[statIndex];
            // is the stat positive or negative?
            if (mod > 0)
            {
                newCrystalStats.text = "+";
            } else
            {
                newCrystalStats.text = "+";
            }

            // what is the modifier of the stat?
            newCrystalStats.text += stat.GetStatIncrease(mod).ToString();
            newCrystalStats.text += " ";

            // what stat is it?
            newCrystalStats.text += stat.GetStatText();
            newCrystalStats.text += "\n";
            
            statIndex++;
        }
    }

    /// <summary>
    /// called by unity button
    /// </summary>
    public void SelectEquipSlot(int index)
    {
        selectedCrystalIndex = index;
        if(CrystalManager.instance.GetEquippedCrystal(index) != null)
        {
            selectedCrystal = CrystalManager.instance.GetEquippedCrystal(index);
            DisplaySelectedCrystal();
        }
    }

    /// <summary>
    /// displays a selected currently-equipped crystal
    /// </summary>
    private void DisplaySelectedCrystal()
    {
        selectedCrystalName.text = selectedCrystal.crystalName;
        int statIndex = 0;
        int mod;

        foreach (IStat stat in selectedCrystal.stats)
        {
            mod = selectedCrystal.mods[statIndex];
            // is the stat positive or negative?
            if (mod > 0)
            {
                selectedCrystalStats.text = "+";
            }
            else
            {
                selectedCrystalStats.text = "+";
            }

            // what is the modifier of the stat?
            selectedCrystalStats.text += stat.GetStatIncrease(mod).ToString();
            selectedCrystalStats.text += " ";

            // what stat is it?
            selectedCrystalStats.text += stat.GetStatText();
            selectedCrystalStats.text += "\n";

            statIndex++;
        }
    }

    public void Trash()
    {
        selectedCrystalIndex = -1;
    }

    public void ApplyChanges()
    {
        if(selectedCrystalIndex >= 0)
        {
            CrystalManager.instance.LoadCrystal(caller.GetCrystal(), selectedCrystalIndex);
            Destroy(caller.gameObject);
        }

        ResetScreen();
    }

    /// <summary>
    /// Reset the screen to its original initialization
    /// </summary>
    public void ResetScreen()
    {
        caller = null;
    }
}
