using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour {
    public static RewardsManager instance;
    private UpgradeObject[] choices;
    private UpgradeContainer[] containers;
    [SerializeField] private UpgradableInt numChoices;
    public GameObject container;
    public Transform rewardSpawnPoint;

    private void Awake() {
      numChoices.Initialize();
      choices = new UpgradeObject[numChoices.Current];
      containers = new UpgradeContainer[numChoices.Current];
    }

    private void Start() {
      for(int i = 0; i < numChoices.Current; i++) {
        UpgradeObject tempUpgrade = AllUpgradeManager.instance.GetRandomOption();
        choices[i] = tempUpgrade;
      }
    }

    public void SpawnUpgrades() {
      // Spawn all upgrades cascading to the right
      for(int i = 0; i < numChoices.Current; i++) {
        Vector3 temp = rewardSpawnPoint.transform.position;
        temp += Vector3.right * (i * 5);
        GameObject obj = Instantiate(container, temp, rewardSpawnPoint.rotation);
        obj.GetComponent<UpgradeContainer>().SetUp(choices[i]);
        containers[i] = obj.GetComponent<UpgradeContainer>();
      }

      foreach (UpgradeContainer container in containers) {
          container.GetComponent<Collider>().enabled = true;
      }
    }
}
