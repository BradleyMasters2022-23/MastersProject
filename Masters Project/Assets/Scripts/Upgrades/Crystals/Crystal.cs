/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - March 16 2023
 * Last Edited - March 29 2023
 * Description - defines a crystal
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public enum CrystalGroup
    {
        HEALTH,
        GUN,
        GRENADE,
        MOVEMENT,
        TIMESTOP,
        SHIELD
    }
    public string crystalName;
    public CrystalGroup group;

    public List<IStat> stats = new List<IStat>();
    public List<int> mods = new List<int>();
    public int statIndex = 0;
    public int cost = 0;
    public int par;

    public Crystal(int p)
    {
        par = p;
    }

    public void AddStat(IStat stat)
    {
        if (statIndex < 3)
        {
            stats.Add(stat);
            CalculateStatCost(statIndex);
            if (mods[statIndex] >= 0)
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

        crystalName += "Crystal";
    }

    private void CalculateStatCost(int index)
    {
        switch (index)
        {
            case 0:
                mods[index] = Random.Range(Mathf.FloorToInt(par / 2), Mathf.CeilToInt(par + par / 2));
                cost += mods[index];
                break;

            case 1:
                if (cost < par)
                {
                    mods[index] = Random.Range(Mathf.FloorToInt(par - cost), Mathf.CeilToInt(par));
                    cost += mods[index];
                }

                if (cost > par)
                {
                    mods[index] = (cost - par);
                    cost += mods[index];
                }
                break;

            case 2:
                mods[index] = (cost - par);
                cost += mods[index];
                break;
        }

    }

    public void EquipCrystal(PlayerController player)
    {
        for(int i = 0; i < 3; i++)
        {
            stats[i].LoadStat(player, mods[i]);
        }
    }

    public void DequipCrystal(PlayerController player)
    {
        for (int i = 0; i < 3; i++)
        {
            stats[i].UnloadStat(player, mods[i]);
        }
    }
}
