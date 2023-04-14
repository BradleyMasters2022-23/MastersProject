using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CrystalSlotScreen : MonoBehaviour
{
    private CrystalInteract caller;
    /// <summary>
    /// The new crystal being offered
    /// </summary>
    private Crystal newCrystal;
    /// <summary>
    /// The currently selected/hovered crystal
    /// </summary>
    private Crystal selectedCrystal;
    private int selectedCrystalIndex = 0;

    [SerializeField] private Sprite blank;

    [SerializeField] private TextMeshProUGUI newCrystalName;
    [SerializeField] private TextMeshProUGUI newCrystalStats;

    [SerializeField] private TextMeshProUGUI selectedCrystalName;
    [SerializeField] private TextMeshProUGUI selectedCrystalStats;

    [SerializeField] private TextMeshProUGUI pennyStats;

    //[SerializeField] private Image equippedCrystal1;
    //[SerializeField] private Image equippedCrystal2;
    //[SerializeField] private Image equippedCrystal3;
    [SerializeField] private Image newCrystalIcon;

    [Tooltip("All inventory slots available to choose from")]
    [SerializeField] private CrystalUIDisplay[] allSlots;

    [SerializeField] private RectTransform newCrystalDisplay;
    [SerializeField] private RectTransform trashDisplay;

    /// <summary>
    /// Reference to the global instance 
    /// </summary>
    private CrystalManager crystalManager;

    [Tooltip("Color applied to stat text when theres a positive effect")]
    [SerializeField] private Color buffColor;
    [Tooltip("Color applied to stat text when theres a negative effect")]
    [SerializeField] private Color debuffColor;

    /// <summary>
    /// Dictionary used to track Penny's stat display
    /// </summary>
    private Dictionary<IStat, float> pennyDisplayDict;
    /// <summary>
    /// Dictionary used to track the new crystal's display
    /// </summary>
    private Dictionary<IStat, float> newCrystalDict;
    /// <summary>
    /// Dictionary used to track the hovered/selected crystal's display
    /// </summary>
    private Dictionary<IStat, float> hoverCrystalDict;

    /// <summary>
    /// Open the screen, change state
    /// </summary>
    public void OpenScreen(CrystalInteract c)
    {
        caller = c;
        newCrystal = caller.GetCrystal();
        selectedCrystalIndex = 0;
        crystalManager = CrystalManager.instance;

        if(crystalManager == null)
        {
            Debug.LogError("Error! Tried to call open crystal screen, but theres no crystal manager!");
            return;
        }

        // Not needed. Now handled in 'InitializeEquippedCrystals' which was originally 'DisplayIcons'
        //if(crystalManager.CrystalEquipped(selectedCrystalIndex))
        //{
        //    selectedCrystal = crystalManager.GetEquippedCrystal(selectedCrystalIndex);
        //    selectedCrystalName.enabled = true;
        //    selectedCrystalStats.enabled = true;
        //    DisplaySelectedCrystal();
        //} else
        //{
        //    selectedCrystalName.enabled = false;
        //    selectedCrystalStats.enabled = false;
        //}

        InitializeEquippedCrystals();
        DisplayNewCrystalStats();
        // Called by InitializeEquippedCrystals
        //DisplayPennyStats();

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
    }

    private void DisplayPennyStats()
    {
        pennyStats.text = "";

        // get all stats, set to default of 0 modifier
        pennyDisplayDict = new Dictionary<IStat, float>();
        foreach (IStat stat in crystalManager.AllStats())
        {
            pennyDisplayDict.Add(stat, 0f);
        }


        // Load all crystals current stats
        for(int i = 0; i < allSlots.Length; i++)
        {
            pennyDisplayDict = UpdateStatDictionary(crystalManager.GetEquippedCrystal(i), pennyDisplayDict);
        }

        // Use this dictionary to determine the differences between removing the old one and equipping the new one
        Dictionary<IStat, float> equippedDifferences = new Dictionary<IStat, float>();

        // Add in the differences being lost, invert them
        equippedDifferences = UpdateStatDictionary(crystalManager.GetEquippedCrystal(selectedCrystalIndex));
        foreach (KeyValuePair<IStat, float> val in equippedDifferences.ToList())
        {
            equippedDifferences[val.Key] *= -1;
        }

        // Add in the stats being gained IF NOT IN TRASH-INDEX RANGE
        if(selectedCrystalIndex != -1)
            equippedDifferences = UpdateStatDictionary(newCrystal, equippedDifferences);

        // Build the new string
        string displayString = "";
        foreach (KeyValuePair<IStat, float> val in pennyDisplayDict)
        {
            string startChar = "";
            string differenceAppend = "";

            // Determine if the whole stat increased or decreased, prepare text icon
            if (val.Value >= 0)
            {
                startChar = "+";
            }

            // Determine if this stat will be DIRECTLY CHANGED. if so, create an additional colored append that shows the difference
            if (equippedDifferences.ContainsKey(val.Key))
            {
                string colorHex = "";
                string modDiff = "";

                // Determine color and first character of the addition
                if (equippedDifferences[val.Key] > 0)
                {
                    modDiff = "+";
                    colorHex = ColorUtility.ToHtmlStringRGBA(buffColor);
                }
                else
                {
                    colorHex = ColorUtility.ToHtmlStringRGBA(debuffColor);
                }

                // Combine the append
                differenceAppend = $" <color=#{colorHex}>{modDiff}{(equippedDifferences[val.Key]).ToString("0.0")}  ->  {(equippedDifferences[val.Key] + val.Value).ToString("0.0")}</color>";
            }

            // Combine all elements, add new line
            displayString += startChar + val.Value.ToString("0.0") + " " + val.Key.GetStatText() + differenceAppend;
            displayString += "\n";
        }

        // Apply the new text
        pennyStats.text = displayString;

        /*
        for (int i = 0; i < 3; i++)
        {
            // instead of the selected crystal, display the new crystal
            if (selectedCrystalIndex == i)
            {
                pennyStats.text += newCrystalStats.text;
                pennyStats.text += "\n";
            }
            else
            {
                if (crystalManager.CrystalEquipped(i))
                {
                    int statIndex = 0;
                    int mod;

                    foreach (IStat stat in crystalManager.GetEquippedCrystal(i).stats)
                    {
                        mod = crystalManager.GetEquippedCrystal(i).mods[statIndex];
                        // is the stat positive or negative?
                        if (mod > 0)
                        {
                            pennyStats.text += "+";
                        }

                        // what is the modifier of the stat?
                        pennyStats.text += stat.GetStatIncrease(mod).ToString();
                        pennyStats.text += " ";

                        // what stat is it?
                        pennyStats.text += stat.GetStatText();
                        pennyStats.text += "\n";

                        statIndex++;
                    }
                }

            }

        }

        */
    }

    private void InitializeEquippedCrystals()
    {
        for(int i = 0; i < crystalManager.MaxSlots(); i++)
        {
            // Load crystal function will automatically apply blank image if given a null crystal
            allSlots[i].LoadCrystal(this, crystalManager.GetEquippedCrystal(i), i);
        }

        // Set default selection to 0. This automatically calls any needed string functions
        SelectEquipSlot(0);

        // Move selection to first empty slot, if possible
        for (int i = 0; i < allSlots.Length; i++)
        {
            if (!allSlots[i].HasUpgrade())
            {
                selectedCrystalIndex = i;
                allSlots[i].SelectUpgrade();
                //newCrystalDisplay.transform = allSlots[i].GetAnchoredPos();

                break;
            }
        }

        // Load current new icon
        newCrystalIcon.sprite = newCrystal.stats[0].GetIcon();
    }

    private void DisplayNewCrystalStats()
    {
        newCrystalName.text = newCrystal.crystalName;
        newCrystalStats.text = "";

        newCrystalDict = UpdateStatDictionary(newCrystal);
        newCrystalStats.text = StatDictToString(newCrystalDict);
    }

    /// <summary>
    /// called by unity button
    /// </summary>
    public void SelectEquipSlot(int index)
    {
        selectedCrystalIndex = index;
        newCrystalDisplay.position = allSlots[selectedCrystalIndex].GetAnchoredPos();

        if (crystalManager.GetEquippedCrystal(index) != null)
        {
            selectedCrystal = crystalManager.GetEquippedCrystal(index);
            selectedCrystalName.enabled = true;
            selectedCrystalStats.enabled = true;
            DisplaySelectedCrystal();
        } else
        {
            selectedCrystalName.enabled = false;
            selectedCrystalStats.enabled = false;
        }

        DisplayPennyStats();
    }

    /// <summary>
    /// displays a selected currently-equipped crystal
    /// </summary>
    private void DisplaySelectedCrystal()
    {
        //pennyDisplayDict.Clear();
        //UpdateStatDictionary(selectedCrystal, pennyDisplayDict);

        selectedCrystalName.text = selectedCrystal.crystalName;
        selectedCrystalStats.text = "";

        hoverCrystalDict = UpdateStatDictionary(selectedCrystal);
        selectedCrystalStats.text = StatDictToString(hoverCrystalDict);

        //foreach (IStat stat in selectedCrystal.stats)
        //{
        //    mod = selectedCrystal.mods[statIndex];
        //    // is the stat positive or negative?
        //    if (mod > 0)
        //    {
        //        selectedCrystalStats.text += "+";
        //    }

        //    // what is the modifier of the stat?
        //    selectedCrystalStats.text += stat.GetStatIncrease(mod).ToString();
        //    selectedCrystalStats.text += " ";

        //    // what stat is it?
        //    selectedCrystalStats.text += stat.GetStatText();
        //    selectedCrystalStats.text += "\n";

        //    statIndex++;
        //}
    }

    public void Trash()
    {
        newCrystalDisplay.position = trashDisplay.position;

        selectedCrystalIndex = -1;
        selectedCrystalName.enabled = false;
        selectedCrystalStats.enabled = false;
        DisplayPennyStats();
    }

    public void ApplyChanges()
    {
        if(selectedCrystalIndex >= 0)
        {
            crystalManager.LoadCrystal(newCrystal, selectedCrystalIndex);
        }
        caller.BegoneCrystalInteract();
    }

    /// <summary>
    /// Create a new dictionary of stats and values. Send in a starting dict to add onto a dict instead.
    /// </summary>
    /// <param name="crystalStats">Crystal whos stats should be added to the dictionary</param>
    /// <param name="startingDict">Any dictionary to start from</param>
    /// <returns>Dictionary with the stats of the passed in crystal added</returns>
    private Dictionary<IStat, float> UpdateStatDictionary(Crystal crystalStats, Dictionary<IStat, float> startingDict = null)
    {
        Dictionary<IStat, float> newDict;

        // Either create a new dictionary or make a copy of the passed in one
        if (startingDict == null)
            newDict = new Dictionary<IStat, float>();
        else
            newDict = new Dictionary<IStat, float>(startingDict);

        // return early if no new crystal stats to add
        if (crystalStats == null)
        {
            return newDict;
        }
            

        // Iterate through all stats and mods the crystal has
        IStat stat;
        int mod;
        for(int i = 0; i < crystalStats.stats.Count; i++)
        {
            stat = crystalStats.stats[i];
            mod = crystalStats.mods[i];

            // Increase the stat if already counted
            if (newDict.ContainsKey(stat))
            {
                newDict[stat] += stat.GetStatIncrease(mod);
            }
            // Apply the stat if not already acounted
            else
            {
                newDict.Add(stat, stat.GetStatIncrease(mod));
            }
        }

        return newDict;
    }

    /// <summary>
    /// Convert a dictionary of stats and floats into a string and send it to a designated text box
    /// Do not use for the complete stat screen, that needs different functionality
    /// </summary>
    /// <param name="stats">Dictionary of stats to display</param>
    /// <returns>String representation of the new dictionary, with color rich text added</returns>
    private string StatDictToString(Dictionary<IStat, float> stats)
    {
        string displayString = "";
        foreach (KeyValuePair<IStat, float> val in stats)
        {
            string colorHex;
            string startChar = "";
            if (val.Value >= 0)
            {
                startChar = "+";
                colorHex = ColorUtility.ToHtmlStringRGBA(buffColor);
            }
            else
            {
                colorHex = ColorUtility.ToHtmlStringRGBA(debuffColor);
            }

            displayString += "<color=#" + colorHex + ">" + startChar + val.Value.ToString("0.0") + " " + val.Key.GetStatText() + "</color>";
            displayString += "\n";
        }

        return displayString;
    }
}
