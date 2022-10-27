using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour {
    private List<UpgradeObject> choices = new List<UpgradeObject>();
    private List<UpgradeContainer> containers = new List<UpgradeContainer>();
    [SerializeField] private UpgradableInt numChoices;
    public GameObject container;
    public Transform rewardSpawnPoint;

    private void Awake() {
      numChoices.Initialize();
    }

    private void Start() {
      for(int i = 0; i < numChoices-1; i++) {
        UpgradeObject tempUpgrade = AllUpgradeManager.instance.GetRandomOption();
        choices.Add(tempUpgrade);
      }
    }

    public void SpawnUpgrades() {
      // Spawn all upgrades cascading to the right
      for(int i = 0; i < numChoices-1; i++) {
        Vector3 temp = rewardSpawnPoint.transform.position;
        temp += Vector3.right * (i * 5);
        GameObject obj = Instantiate(container, temp, rewardSpawnPoint.rotation);
        obj.GetComponent<UpgradeContainer>().SetUp(chosenUpgrades[i]);
        containers.Add(obj.GetComponent<UpgradeContainer>());
      }

      foreach (UpgradeContainer upgrade in containers) {
          upgrade.GetComponent<Collider>().enabled = true;
      }
    }
}
