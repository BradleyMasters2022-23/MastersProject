/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - August 8th, 2023
 * Last Edited - August 8th, 2023 by Ben Schuster
 * Description - Shoot function with added animation support for the turret enemy
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShoot : SimpleShoot
{
    [Tooltip("Animator controlling turret gun")]
    [SerializeField] Animator turretGunAnimator;

    protected override void ShowIndicator()
    {
        base.ShowIndicator();
        turretGunAnimator.SetBool("Shooting", true);
    }

    protected override void ShowAttackDone()
    {
        base.HideAttackDone();
        turretGunAnimator.SetBool("Shooting", false);
    }
}
