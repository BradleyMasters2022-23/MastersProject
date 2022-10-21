/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 18th, 2022
 * Last Edited - October 20th, 2022 by Soma Hannon
 * Description - Handles player upgrades.
 * ================================================================================================
 */
 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.SceneManagement;

namespace Masters.PlayerUpgradeMechanics {
  public abstract class PlayerUpgradeManager : MonoBehaviour {
    // TODO: connect to PlayerControllerRB when implemented
    // TODO: write this class lol


    public List<UpgradeObject> upgrades = new List<UpgradeObject>();
  }
}
