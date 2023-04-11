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

    public List<string> statsDesc = new List<string>();

    /// <summary>
    /// current stat to be added. doubles as total # of stats after initialization
    /// </summary>
    public int statIndex = 0;

    /// <summary>
    /// current cost of all stats added. should come out equal to par ater initialization.
    /// </summary>
    public int cost = 0;

    /// <summary>
    /// number from which to generate stats. the "level" of the crystal
    /// </summary>
    public int par;

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
    /// <param name="index">Determines cost of stat</param>
    private void CalculateStatCost(int index)
    {
        switch (index)
        {
            // for the first stat, cost is between par/2 and par+par/2
            case 0:
                mods.Add(Random.Range(Mathf.FloorToInt(par / 2), Mathf.CeilToInt(par + par / 2)));
                cost += mods[index];
                break;

            // for the second stat
            case 1:
                // if current cost is less than par, second stat's cost is between 
                if (cost < par)
                {
                    mods.Add(Random.Range(Mathf.FloorToInt(par - cost), Mathf.CeilToInt(par)));
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
}
