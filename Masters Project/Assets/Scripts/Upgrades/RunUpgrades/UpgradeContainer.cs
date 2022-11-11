/* ================================================================================================
 * Author - Soma Hannon (base code - Ben Schuster)
 * Date Created - October 25, 2022
 * Last Edited - October 25, 2022 by Soma Hannon
 * Description - Physical object that triggers the upgrade select screen.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeContainer : MonoBehaviour {
  [SerializeField] private UpgradeObject upgrade;
  private List<GameObject> linkedUpgrades = new List<GameObject>();
  public TextMeshProUGUI nameText;
  private Color color;

  /// <summary>
  /// ensures that upgrade is not null and calls SetUp
  /// </summary>
  private void Start() {
      if (upgrade is null) {
        Destroy(this);
      }

      if(upgrade != null) {
        SetUp(upgrade);
      }
  }

  /// <summary>
  /// called exactly once, initializes container
  /// </summary>
  public void SetUp(UpgradeObject obj) {
      upgrade = obj;
      color = upgrade.upgradeColor;
      GetComponent<Renderer>().material.color = color;
      nameText.text = upgrade.displayName;
  }

  public void AddLink(GameObject obj)
  {
      linkedUpgrades.Add(obj);
  }

  /// <summary>
  /// called when player walks into the object. eventually change to a button?
  /// </summary>
  private void OnTriggerEnter(Collider other) {
      if(other.CompareTag("Player")) {
          // TODO: trigger upgrade select screen.
          // buttons on USS call PlayerUpgradeManager.AddUpgrade() for the linked upgrade

          for (int i = linkedUpgrades.Count - 1; i >= 0; i--)
          {
              Destroy(linkedUpgrades[i]);
          }
          linkedUpgrades.Clear();

          PlayerUpgradeManager.instance.AddUpgrade(upgrade);

          Destroy(this.gameObject);
      }
  }
}
