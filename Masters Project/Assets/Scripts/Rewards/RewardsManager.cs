using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager instance;
    private List<UpgradeObject> choices = new List<UpgradeObject>();
    private List<UpgradeInteract> containers = new List<UpgradeInteract>();
    [SerializeField] private UpgradableInt numChoices;
    public GameObject container;
    public Transform rewardSpawnPoint;
    public bool linked = true;

    private void Awake()
    {
        numChoices.Initialize();
    }

    private void Start()
    {
        if(AllUpgradeManager.instance.GetAll().Count < numChoices.Current) {
            numChoices.ChangeVal(AllUpgradeManager.instance.GetAll().Count);
        }

        while(choices.Count < numChoices.Current) {
            UpgradeObject tempUpgrade = AllUpgradeManager.instance.GetRandomOption();
            if(!choices.Contains(tempUpgrade)) {
                choices.Add(tempUpgrade);
            }
        }
    }
}
