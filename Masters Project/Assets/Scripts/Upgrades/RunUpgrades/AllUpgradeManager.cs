/* ================================================================================================
 * Author - Soma Hannon (base code - Ben Schuster)
 * Date Created - October 25, 2022
 * Last Edited - November 17, 2022 by Ben Schuster
 * Description - Holds list of all upgrades.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllUpgradeManager : MonoBehaviour {
    public static AllUpgradeManager instance;
    [SerializeField] private List<UpgradeObject> allUpgrades;
    private List<UpgradeObject> upgradeOptions;

    /// <summary>
    /// ensures only one instance and initializes upgradeOptions
    /// </summary>
    void Start()
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

        upgradeOptions = new List<UpgradeObject>();

        foreach(UpgradeObject i in allUpgrades)
        {
            upgradeOptions.Add(i);
        }
    }

    /// <summary>
    /// removes an upgrade from the list of options
    /// </summary>
    public void RemoveUpgrade(UpgradeObject uo)
    {
        if(upgradeOptions.Contains(uo))
        {
            upgradeOptions.Remove(uo);
        }
    }

    /// <summary>
    /// returns all extant upgrades
    /// </summary>
    public List<UpgradeObject> GetAll()
    {
        return allUpgrades;
    }

    /// <summary>
    /// returns all upgrade options
    /// </summary>
    public List<UpgradeObject> GetOptions()
    {
        return upgradeOptions;
    }

    /// <summary>
    /// returns a random upgrade option
    /// </summary>
    public UpgradeObject GetRandomOption()
    {
        return upgradeOptions[Random.Range(0, upgradeOptions.Count)];
    }

    /// <summary>
    /// Request a specific number of upgrades
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public UpgradeObject[] GetRandomOptions(int num)
    {
        UpgradeObject[] selected;
        int goal;

        // if there are less upgrades, return an error msg
        if (num > upgradeOptions.Count)
        {
            Debug.LogError("Requesting too many upgrades! returning less");
            selected = new UpgradeObject[upgradeOptions.Count];
            goal = upgradeOptions.Count;
        }
        // Otherwise, meet the requirement
        else
        {
            selected = new UpgradeObject[num];
            goal = num;
        }
        
        // Select up to the goal of options, validate with no dupes
        for(int i = 0; i < goal; i++)
        {
            // select options, validate no dupes
            UpgradeObject chosen;
            bool alreadySelected;
            do
            {
                alreadySelected = false;
                chosen = GetRandomOption();

                for(int j = 0; j < i; j++)
                {
                    if(selected[j] != null && selected[j] == chosen)
                    {
                        alreadySelected = true;
                        continue;
                    }
                }


            } while (alreadySelected);

            selected[i] = chosen;
        }

        // Randomly select upgrades that are unique from eachother

        return selected;
    }

      public void DestroyAUM()
      {
          instance = null;
          Destroy(gameObject);
      }
}
