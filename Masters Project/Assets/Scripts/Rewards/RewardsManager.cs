using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager instance;
    private List<UpgradeObject> choices = new List<UpgradeObject>();
    private List<UpgradeContainer> containers = new List<UpgradeContainer>();
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

    public void SpawnUpgrades()
    {
        // Spawn all upgrades cascading to the right
        for(int i = 0; i < numChoices.Current; i++)
        {
            Vector3 temp = rewardSpawnPoint.transform.position;
            temp += Vector3.right * (i * 5);
            GameObject obj = Instantiate(container, temp, rewardSpawnPoint.rotation);
            obj.GetComponent<UpgradeContainer>().SetUp(choices[i]);
            containers[i] = obj.GetComponent<UpgradeContainer>();
        }

        if(linked && numChoices.Current > 1)
        {
            foreach(UpgradeContainer up in containers)
            {
                foreach(UpgradeContainer link in containers)
                {
                    if(up != link)
                    {
                        up.AddLink(link.gameObject);
                    }
                }
            }
        }

        foreach (UpgradeContainer container in containers)
        {
            container.GetComponent<Collider>().enabled = true;
        }
    }
}
