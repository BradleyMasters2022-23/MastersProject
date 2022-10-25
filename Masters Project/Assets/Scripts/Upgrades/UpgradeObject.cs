/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 20th, 2022
 * Last Edited - October 21st, 2022 by Soma Hannon
 * Description - Base upgrade object.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Masters.CoreUpgradeVariables;

[CreateAssetMenu(menuName = "Gameplay/Upgrade Data")]
public class UpgradeObject : ScriptableObject {
  public string displayName;
  public string displayDesc;
  public int ID;
  public UpgradableInt lvl;
  public GameObject upgradePrefab;
}
