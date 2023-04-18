/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - March 16 2023
 * Last Edited - March 30 2023
 * Description - defines a crystal
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal
{
    /// <summary>
    /// Name of crystal. Populated as stats are added.
    /// </summary>
    public string crystalName;

    /// <summary>
    /// List of stats the crystal has.
    /// </summary>
    public List<IStat> stats = new List<IStat>();

    /// <summary>
    /// List of the modifiers for the stats in stats.
    /// </summary>
    public List<int> mods = new List<int>();

    /// <summary>
    /// current stat to be added. doubles as total # of stats after initialization
    /// </summary>
    public int statIndex = 0;

    /// <summary>
    /// current cost of all stats added. should come out equal to par after initialization.
    /// </summary>
    public int cost = 0;

    /// <summary>
    /// number from which to generate stats. the "level" of the crystal
    /// </summary>
    public int par;

    public Image icon;

    private Dictionary<IStat, float> statDict;

    /// <summary>
    /// Adds a stat to the crystal
    /// </summary>
    /// <param name="stat">Stat to be added</param>
    public void AddStat(IStat stat)
    {
        if (statIndex < 3 && cost != par)
        {
            stats.Add(stat);

            CalculateStatCost(statIndex);
            if (mods[statIndex] > 0)
            {
                crystalName += stat.GetPlusKeyword();
                crystalName += " ";
            } else
            {
                crystalName += stat.GetMinusKeyword();
                crystalName += " ";
            }
            
            statIndex++;
        }
    }

    /// <summary>
    /// Calculates the cost of the current stat based on its index
    /// </summary>
    /// <param name="index">Determines cost formula</param>
    private void CalculateStatCost(int index)
    {
        switch (index)
        {
            // for the first stat, cost is between par/2 and par+par/2
            case 0:
                int val = Random.Range(Mathf.CeilToInt(par / 2f), Mathf.CeilToInt(par + (par / 2f)));
                //Debug.Log($"Set par : {par}");
                //Debug.Log($"Calculating random between {Mathf.CeilToInt(par / 2f)} and {Mathf.CeilToInt(par + (par / 2f))}");
                //Debug.Log($"Modded value at mod[0] is : {val}");
                mods.Add(val);
                cost += mods[index];
                break;

            // for the second stat
            case 1:
                // if current cost is less than par, second stat's cost is between par-cost and par
                if (cost < par)
                {
                    mods.Add(Random.Range(Mathf.CeilToInt(par - cost), Mathf.CeilToInt(par)));
                    cost += mods[index];
                }

                // if current cost is more than par, cost of second stat is the difference between them
                if (cost > par)
                {
                    mods.Add(par - cost);
                    cost += mods[index];
                }
                
                break;

            case 2:
                mods.Add(cost - par);
                cost += mods[index];
                break;
        }

    }

    /// <summary>
    /// Loads all stats onto the player 
    /// </summary>
    /// <param name="player">Player reference</param>
    public void EquipCrystal(PlayerController player)
    {
        int i = 0;
        foreach(IStat stat in stats)
        {
            stat.LoadStat(player, mods[i]);
            i++;
        }
    }

    /// <summary>
    /// Unloads all stats from the player
    /// </summary>
    /// <param name="player">Player reference</param>
    public void DequipCrystal(PlayerController player)
    {
        int i = 0;
        foreach(IStat stat in stats)
        {
            stat.UnloadStat(player, mods[i]);
            i++;
        }
    }

    public int GetNumStats()
    {
        int i = 0;
        foreach (IStat stat in stats)
        {
            if(stat != null)
            {
                i++;
            }
        }

        return i;
    }

    /// <summary>
    /// Get the icon for this crystal
    /// </summary>
    /// <returns>this icon's crystal</returns>
    public Sprite Icon()
    {
        return stats[0].GetIcon();
    }

    /// <summary>
    /// Get the color for this crystal. Will be the second stats color, if available
    /// </summary>
    /// <returns>The color for this crystal</returns>
    public Color GetColor()
    {
        if (stats.Count > 1 && stats[1] != null)
            return stats[1].GetColor();
        else
            return stats[0].GetColor();
    }
    public Dictionary<IStat, float> GetStats()
    {
        if (statDict != null)
            return new Dictionary<IStat, float>(statDict);
        else
            return GetStatDict();
    }

    /// <summary>
    /// Create a new dictionary of stats and values. Send in a starting dict to add onto a dict instead.
    /// </summary>
    /// <param name="startingDict">Any dictionary to start from</param>
    /// <returns>Dictionary with the stats of the passed in crystal added</returns>
    public Dictionary<IStat, float> GetStatDict(Dictionary<IStat, float> startingDict = null)
    {
        Dictionary<IStat, float> newDict;

        // Either create a new dictionary or make a copy of the passed in one
        if (startingDict == null)
            newDict = new Dictionary<IStat, float>();
        else
            newDict = new Dictionary<IStat, float>(startingDict);

        // return early if no new crystal stats to add
        if (stats == null)
        {
            return newDict;
        }


        // Iterate through all stats and mods the crystal has
        IStat stat;
        int mod;
        for (int i = 0; i < stats.Count; i++)
        {
            stat = stats[i];
            mod = mods[i];

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

}
