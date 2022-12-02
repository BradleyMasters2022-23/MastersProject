/* ================================================================================================
 * Author - Soma Hannon (base code - Ben Schuster)
 * Date Created - October 20, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Base upgrade object.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Masters.CoreUpgradeVariables;

[CreateAssetMenu(menuName = "Gameplay/Upgrade Data")]
public class UpgradeObject : ScriptableObject {
    [Tooltip("Upgrade display name.")]
    public string displayName;

    [Tooltip("Upgrade display description.")]
    public string displayDesc;

    [Tooltip("Upgrade container color.")]
    public Color upgradeColor;

    [Tooltip("Upgrade ID #.")]
    public int ID;

    [Tooltip("Upgrade level start, min, max.")]
    public UpgradableInt lvl;

    [Tooltip("Holds the upgrade script itself plus other game info.")]
    public GameObject upgradePrefab;

    [Tooltip("Icon used to represent this upgrade")]
    public Sprite displayIcon;
}
