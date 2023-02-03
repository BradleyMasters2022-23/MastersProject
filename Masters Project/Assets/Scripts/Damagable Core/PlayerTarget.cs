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
using UnityEngine.InputSystem;

public class PlayerTarget : Target
{
    private GameControls c;
    private InputAction godCheat;

    [SerializeField] private GameObject godCheatNotification;

    protected override void KillTarget()
    {
        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
    }

    private void Start()
    {
        c = GameManager.controls;
        godCheat = c.PlayerGameplay.GodCheat;
        godCheat.performed += ToggleGodmode;

        if (godCheatNotification != null)
            godCheatNotification.SetActive(_healthManager.God);
    }

    private void ToggleGodmode(InputAction.CallbackContext ctx = default)
    {
        if (_healthManager.God)
            _healthManager.ToggleGodmode(false);
        else
            _healthManager.ToggleGodmode(true);

        if(godCheatNotification != null)
            godCheatNotification.SetActive(_healthManager.God);
    }

    private void OnDisable()
    {
        godCheat.performed -= ToggleGodmode;
    }
}
