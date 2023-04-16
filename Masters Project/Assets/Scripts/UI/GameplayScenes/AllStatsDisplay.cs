using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class AllStatsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI display;

    private Dictionary<IStat, float> allStats;

    private CrystalManager source;

    private void OnEnable()
    {
        if(display == null)
            display = GetComponentInChildren<TextMeshProUGUI>();

        if (source == null)
            source = CrystalManager.instance;
    }

    public void LoadEquippedStats()
    {
        if (source == null)
        {
            source = CrystalManager.instance;
            if(source == null)
            {
                display.text = "No Stats Found";
                return;
            }
        }

        // Get all stats
        allStats = BuildStatBlock();

        // Create string based on all stats
        string displayString = "";
        foreach (KeyValuePair<IStat, float> val in allStats)
        {
            string startChar = "";

            // Determine if the whole stat increased or decreased, prepare text icon
            if (val.Value >= 0)
            {
                startChar = "+";
            }

            // Combine all elements, add new line
            displayString += startChar + val.Value.ToString("0.0") + " " + val.Key.GetStatText();
            displayString += "\n";
        }

        display.text = displayString;
    }

    public void DetermineDifferences(Crystal newCrystal, Crystal oldCrystal)
    {
        if (source == null)
        {
            source = CrystalManager.instance;
            if (source == null)
            {
                display.text = "No Stats Found";
                return;
            }
        }

        allStats = BuildStatBlock();

        // Create dictionary to track differences
        Dictionary<IStat, float> differences= new Dictionary<IStat, float>();

        // If theres an old crystal being replaced, get its stats and invert them
        if(oldCrystal != null)
        {
            differences = oldCrystal.GetStatDict();
            foreach (KeyValuePair<IStat, float> val in differences.ToList())
            {
                differences[val.Key] *= -1;
            }
        }

        // If theres a new crystal being equipped, load their differences in as well in addition to the old one
        if(newCrystal != null)
        {
            differences = newCrystal.GetStatDict(differences);
        }

        // Create the new string from all stats and the newly generated differences 
        string displayString = "";
        foreach (KeyValuePair<IStat, float> val in allStats)
        {
            string startChar = "";
            string differenceAppend = "";

            // Determine if the whole stat increased or decreased, prepare text icon
            if (val.Value >= 0)
            {
                startChar = "+";
            }

            // Determine if this stat will be DIRECTLY CHANGED. if so, create an additional colored append that shows the difference
            if (differences.ContainsKey(val.Key))
            {
                string colorHex = "";
                string modDiff = "";

                // Determine color and first character of the addition
                if (differences[val.Key] >= 0)
                {
                    modDiff = "+";
                    colorHex = CrystalManager.instance.PositiveTextHex();
                }
                else
                {
                    colorHex = CrystalManager.instance.NegativeTextHex();
                }

                // Combine the append
                differenceAppend = $" <color=#{colorHex}>{modDiff}{(differences[val.Key]).ToString("0.0")}  ->  {(differences[val.Key] + val.Value).ToString("0.0")}</color>";
            }

            // Combine all elements, add new line
            displayString += startChar + val.Value.ToString("0.0") + " " + val.Key.GetStatText() + differenceAppend;
            displayString += "\n";
        }

        display.text = displayString;
    }

    private Dictionary<IStat, float> BuildStatBlock()
    {
        if (source == null)
        {
            source = CrystalManager.instance;
            if (source == null)
            {
                display.text = "No Stats Found";
                return null;
            }
        }

        // Get all stats
        Dictionary<IStat, float> newDict = new Dictionary<IStat, float>();
        
        foreach (IStat stat in source.AllStats())
        {
            newDict.Add(stat, 0f);
        }

        // Get all modified stats that are equipped
        for (int i = 0; i < source.MaxSlots(); i++)
        {
            Crystal c = source.GetEquippedCrystal(i);
            if (c != null)
            {
                newDict = c.GetStatDict(newDict);
            }
        }

        return newDict;
    }

    public void Clear()
    {
        if(allStats != null)
            allStats.Clear();

        display.text = "";
    }
}
