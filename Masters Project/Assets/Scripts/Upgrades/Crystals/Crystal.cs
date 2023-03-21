using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : IUpgrade
{
    public List<GameObject> stats = new List<GameObject>();
    public List<int> mods = new List<int>();
    public int statIndex = 0;
    public int cost = 0;
    public int par;

    public Crystal(int p)
    {
        par = p;
    }

    public void AddStat(GameObject stat)
    {
        if (statIndex < 3)
        {
            stats.Add(stat);
            CalculateStatCost(statIndex);
            statIndex++;
        }
    }

    public void CalculateStatCost(int index)
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

    public override void LoadUpgrade(PlayerController player)
    {

    }
}
