/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 25, 2022
 * Last Edited - October 25, 2022 by Soma Hannon
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
  void Start() {
    if(instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    } else {
        Destroy(this.gameObject);
    }

    upgradeOptions = new List<UpgradeObject>();

    foreach(UpgradeObject i in allUpgrades) {
      upgradeOptions.Add(i);
    }
  }

  /// <summary>
  /// removes an upgrade from the list of options
  /// </summary>
  public void RemoveUpgrade(UpgradeObject uo) {
    if(upgradeOptions.Contains(uo)) {
      upgradeOptions.Remove(uo);
    }
  }

  /// <summary>
  /// returns all extant upgrades
  /// </summary>
  public List<UpgradeObject> GetAll() {
    return allUpgrades;
  }

  /// <summary>
  /// returns all upgrade options
  /// </summary>
  public List<UpgradeObject> GetOptions() {
    return upgradeOptions;
  }

  /// <summary>
  /// returns a random upgrade option
  /// </summary>
  public UpgradeObject GetRandomOption() {
    return upgradeOptions[Random.Range(0, upgradeOptions.Count)];
  }

}
