using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrystalManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Crystal GenerateCrystal(int p)
    {
        Crystal newCrystal = new Crystal(p);

        for (int i = 0; i < 3; i++)
        {
            if (newCrystal.cost != newCrystal.par)
            {
                newCrystal.AddStat(stats[Random.Range(0, stats.Length)]);
                newCrystal.CalculateStatCost(i);
            }
        }

        return newCrystal;
    }
}
