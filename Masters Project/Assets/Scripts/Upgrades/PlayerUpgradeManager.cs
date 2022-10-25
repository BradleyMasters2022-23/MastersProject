/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 18, 2022
 * Last Edited - October 25, 2022 by Soma Hannon
 * Description - Handles player upgrades.
 * ================================================================================================
 */
 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.SceneManagement;

public abstract class PlayerUpgradeManager : MonoBehaviour {
  // TODO: connect to PlayerController when implemented
  // TODO: write this class lol

  public PlayerController player;
  public List<UpgradeObject> upgrades = new List<UpgradeObject>();
  public static PlayerUpgradeManager instance;

  /// <summary>
  /// called exactly once, initializes PlayerUpgradeManager
  /// </summary>
  private void Start() {
    player = FindObjectOfType<PlayerController>();
    // ensures only one instance ever exists
    if(instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    } else {
        Destroy(this.gameObject);
    }
  }

  /// <summary>
  /// initializes upgrade and attaches it to player
  /// </summary>
  private void InitializeUpgrade(UpgradeObject up) {
    IUpgrade temp = Instantiate(up.upgradePrefab, player.gameObject.transform).GetComponent<IUpgrade>();
    temp.LoadUpgrade(player);
  }

  /// <summary>
  /// adds upgrade to list of players upgrades and initializes
  /// </summary>
  public void AddUpgrade(UpgradeObject up) {
    upgrades.Add(up);
    InitializeUpgrade(up);
  }

  /// <summary>
  /// returns list of player's upgrades
  /// </summary>
  public List<UpgradeObject> GetUpgrades() {
    return new List<UpgradeObject>(upgrades);
  }

}
