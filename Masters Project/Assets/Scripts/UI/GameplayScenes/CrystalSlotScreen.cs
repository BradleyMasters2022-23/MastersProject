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

    [SerializeField] private Sprite blank;

    [SerializeField] private TextMeshProUGUI newCrystalName;
    [SerializeField] private TextMeshProUGUI newCrystalStats;

    [SerializeField] private TextMeshProUGUI selectedCrystalName;
    [SerializeField] private TextMeshProUGUI selectedCrystalStats;

    [SerializeField] private Image equippedCrystal1;
    [SerializeField] private Image equippedCrystal2;
    [SerializeField] private Image equippedCrystal3;
    [SerializeField] private Image newCrystal;

    /// <summary>
    /// Open the screen, change state
    /// </summary>
    public void OpenScreen(CrystalInteract c)
    {
        caller = c;
        selectedCrystalIndex = 0;
        if(CrystalManager.instance.CrystalEquipped(selectedCrystalIndex))
        {
            selectedCrystal = CrystalManager.instance.GetEquippedCrystal(selectedCrystalIndex);
            selectedCrystalName.enabled = true;
            selectedCrystalStats.enabled = true;
            DisplaySelectedCrystal();
        } else
        {
            selectedCrystalName.enabled = false;
            selectedCrystalStats.enabled = false;
        }
        DisplayCrystalIcons();
        DisplayNewCrystalStats();

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
    }

    private void DisplayCrystalIcons()
    {
        if(CrystalManager.instance.CrystalEquipped(0))
        {
            equippedCrystal1.sprite = CrystalManager.instance.GetEquippedCrystal(0).stats[0].GetIcon();
            if (CrystalManager.instance.GetEquippedCrystal(0).GetNumStats() > 1)
            {
                equippedCrystal1.color = CrystalManager.instance.GetEquippedCrystal(0).stats[1].GetColor();
            } else
            {
                equippedCrystal3.color = CrystalManager.instance.GetEquippedCrystal(0).stats[0].GetColor();
            }
        } else
        {
            equippedCrystal1.sprite = blank;
        }

        if (CrystalManager.instance.CrystalEquipped(1))
        {
            equippedCrystal2.sprite = CrystalManager.instance.GetEquippedCrystal(1).stats[0].GetIcon();
            if (CrystalManager.instance.GetEquippedCrystal(1).GetNumStats() > 1)
            {
                equippedCrystal2.color = CrystalManager.instance.GetEquippedCrystal(1).stats[1].GetColor();
            } else
            {
                equippedCrystal3.color = CrystalManager.instance.GetEquippedCrystal(1).stats[0].GetColor();
            }
        } else
        {
            equippedCrystal2.sprite = blank;
        }

        if (CrystalManager.instance.CrystalEquipped(2))
        {
            equippedCrystal3.sprite = CrystalManager.instance.GetEquippedCrystal(2).stats[0].GetIcon();
            if (CrystalManager.instance.GetEquippedCrystal(0).GetNumStats() > 1)
            {
                equippedCrystal3.color = CrystalManager.instance.GetEquippedCrystal(2).stats[1].GetColor();
            } else
            {
                equippedCrystal3.color = CrystalManager.instance.GetEquippedCrystal(2).stats[0].GetColor();
            }
        } else
        {
            equippedCrystal3.sprite = blank;
        }

        newCrystal.sprite = caller.GetCrystal().stats[0].GetIcon();
        if (caller.GetCrystal().GetNumStats() > 1)
        {
            newCrystal.color = caller.GetCrystal().stats[1].GetColor();
        } else
        {
            newCrystal.color = caller.GetCrystal().stats[0].GetColor();
        }
    }

    private void DisplayNewCrystalStats()
    {
        newCrystalName.text = caller.GetCrystal().crystalName;
        int statIndex = 0;
        int mod;
        newCrystalStats.text = "";
        
        foreach(IStat stat in caller.GetCrystal().stats)
        {
            mod = caller.GetCrystal().mods[statIndex];
            // is the stat positive or negative?
            if (mod > 0)
            {
                newCrystalStats.text += "+";
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
            selectedCrystalName.enabled = true;
            selectedCrystalStats.enabled = true;
            DisplaySelectedCrystal();
        } else
        {
            selectedCrystalName.enabled = false;
            selectedCrystalStats.enabled = false;
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
        selectedCrystalStats.text = "";

        foreach (IStat stat in selectedCrystal.stats)
        {
            mod = selectedCrystal.mods[statIndex];
            // is the stat positive or negative?
            if (mod > 0)
            {
                selectedCrystalStats.text += "+";
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
        }
        caller.BegoneCrystalInteract();
        
    }

    
}
