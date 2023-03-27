using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public List<Crystal> playerCrystals = new List<Crystal>();
    public List<GameObject> stats = new List<GameObject>();

    public Crystal GenerateCrystal(int p)
    {
        Crystal newCrystal = new Crystal(p);

        for (int i = 0; i < 3; i++)
        {
            if (newCrystal.cost != newCrystal.par)
            {
                newCrystal.AddStat(stats[Random.Range(0, stats.Count)]);
                newCrystal.CalculateStatCost(i);
            }
        }

        return newCrystal;
    }
}
