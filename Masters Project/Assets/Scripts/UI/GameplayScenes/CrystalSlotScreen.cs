using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrystalSlotScreen : MonoBehaviour
{
    private CrystalInteract caller;
    private Crystal selectedCrystal;
    private int selectedCrystalIndex;
    [SerializeField] private TextMeshProUGUI newCrystalName;
    [SerializeField] private TextMeshProUGUI newCrystalStats;

    /// <summary>
    /// Open the screen, change state
    /// </summary>
    public void OpenScreen(CrystalInteract c)
    {
        caller = c;
        DisplayNewCrystalStats();

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
    }

    public void DisplayNewCrystalStats()
    {
        newCrystalName.text = caller.GetCrystal().crystalName;
        int statIndex = 0;
        int mod;
        
        foreach(IStat stat in caller.GetCrystal().stats)
        {
            mod = caller.GetCrystal().mods[statIndex];
            // is the stat positive or negative?
            if (caller.GetCrystal().mods[statIndex] > 0)
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
    public void SelectEquippedCrystal(int index)
    {
        if(CrystalManager.instance.GetEquippedCrystal(index) != null)
        {
            selectedCrystal = CrystalManager.instance.GetEquippedCrystal(index);
        }

        
    }
    public void DisplaySelectedCrystal()
    {

    }

    /// <summary>
    /// Reset the screen to its original initialization
    /// </summary>
    public void ResetScreen()
    {
        caller = null;
    }
}
