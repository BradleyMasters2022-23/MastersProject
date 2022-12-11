using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadCollectedUpgradesUI : MonoBehaviour
{
    private List<UpgradeObject> collectedUpgrades;

    [SerializeField] private UpgradeSelectModule[] options;

    [SerializeField] private SmartExpandContent contentDrawer;

    private Dictionary<UpgradeObject, int> upgradeAndCount = new Dictionary<UpgradeObject, int>();

    private void OnEnable()
    {
        collectedUpgrades = new List<UpgradeObject>(PlayerUpgradeManager.instance.upgrades);
        PopulateList();
    }


    private void PopulateList()
    {
        if(collectedUpgrades.Count > options.Length)
        {
            Debug.LogError("[LoadCollectedUpgradesUI] Warning! there are too many upgrades for the UI to load!" +
                "Increase the buffer for the options available!");
            return;
        }

        upgradeAndCount.Clear();

        // Condense and count repeats
        for (int i = 0; i < collectedUpgrades.Count; i++)
        {
            if (upgradeAndCount.ContainsKey(collectedUpgrades[i]))
            {
                upgradeAndCount[collectedUpgrades[i]]++;
            }
            else
            {
                upgradeAndCount.Add(collectedUpgrades[i], 1);
            }
        }


        // Load the initial ones 
        for (int i = 0; i < upgradeAndCount.Count; i++)
        {
            options[i].InitializeUIElement(upgradeAndCount.ElementAt(i).Key, "X" + upgradeAndCount.ElementAt(i).Value.ToString());
            options[i].gameObject.SetActive(true);
        }

        // hide the rest
        for(int i = upgradeAndCount.Count; i < options.Length; i++)
        {
            options[i].ClearUI();
            options[i].gameObject.SetActive(false);
        }

        contentDrawer.CalculateHeight();
    }

    private void OnDisable()
    {
        // Load the initial ones 
        for (int i = 0; i < collectedUpgrades.Count; i++)
        {
            options[i].ClearUI();
            options[i].gameObject.SetActive(false);
        }
    }
}
