/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 2nd, 2022
 * Last Edited - February 2nd, 2022 by Ben Schuster
 * Description - Control concrete player-based entities
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : Target
{
    protected override void KillTarget()
    {
        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
    }
}
