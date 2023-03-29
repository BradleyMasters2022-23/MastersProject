/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - March 16 2023
 * Last Edited - March 29 2023
 * Description - holds and generates all crystals
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public static CrystalManager instance;
    public List<Crystal> playerCrystals = new List<Crystal>();
    public List<IStat> stats = new List<IStat>();
    private PlayerController player;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
}
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Crystal GenerateCrystal(int p)
    {
        Crystal newCrystal = new Crystal(p);

        for (int i = 0; i < 3; i++)
        {
            if (newCrystal.cost != newCrystal.par)
            {
                newCrystal.AddStat(stats[Random.Range(0, stats.Count)]);
            }
        }

        return newCrystal;
    }

    public void LoadCrystal(Crystal crystal)
    {
        
    }
}
