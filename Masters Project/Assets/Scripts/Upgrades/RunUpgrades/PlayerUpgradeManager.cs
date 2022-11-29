/* ================================================================================================
 * Author - Soma Hannon (base code - Ben Schuster)
 * Date Created - October 18, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Handles player upgrades.
 * ================================================================================================
 */
 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.SceneManagement;

public class PlayerUpgradeManager : MonoBehaviour
{
    public PlayerController player;
    public List<UpgradeObject> upgrades = new List<UpgradeObject>();
    public static PlayerUpgradeManager instance;

    private void Awake() {
        // ensures only one instance ever exists
        if(instance == null) {
            PlayerUpgradeManager.instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    /// <summary>
    /// called exactly once, initializes PlayerUpgradeManager
    /// </summary>
    private void Start() {
        player = FindObjectOfType<PlayerController>();
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
        // foreach(UpgradeObject upgrade in upgrades) {
        //     if(upgrade == up) {
        //         upgrade.lvl.Increment(1);
        //     }
        //     if(upgrade.lvl.AtMax()) {
        //         AllUpgradeManager.instance.upgradeOptions.Remove(upgrade);
        //     }
        // }
        upgrades.Add(up);
        InitializeUpgrade(up);
    }

    /// <summary>
    /// returns list of player's upgrades
    /// </summary>
    public List<UpgradeObject> GetUpgrades() {
      return new List<UpgradeObject>(upgrades);
    }

    /// <summary>
    /// re-initializes player upgrades in new scenes except Hub
    /// </summary>
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
            instance = null;
            Destroy(gameObject);
            return;
        }

        foreach(UpgradeObject up in upgrades) {
            InitializeUpgrade(up);
        }
    }

    /// <summary>
    /// destroys instance
    /// </summary>
    public void DestroyPUM() {
        PlayerUpgradeManager.instance = null;
        Destroy(gameObject);
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelLoad;
    }

}
