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

public class PlayerUpgradeManager : MonoBehaviour {
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
        PlayerUpgradeManager.instance = this;
        DontDestroyOnLoad(this);
    } else {
        Destroy(this);
    }

    SceneManager.sceneLoaded += OnLevelLoad;
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

  public void OnLevelLoad(Scene scene, LoadSceneMode mode) {
      if (scene.name == "Hub") {
          DestroyPUM();
          return;
      }

      int c = 0;
      do {
          c++;
          if (c >= 10000)
              break;

          player = FindObjectOfType<PlayerController>();
      } while (player == null);

      if (player == null)
      {
          Debug.LogError("cannot load player upgrades; no player instance found!");
          return;
      }

      foreach(UpgradeObject up in upgrades) {
          InitializeUpgrade(up);
      }
  }

  public void DestroyPUM() {
      PlayerUpgradeManager.instance = null;
      Destroy(gameObject);
  }

  private void OnDisable() {
      SceneManager.sceneLoaded -= OnLevelLoad;
  }

}
