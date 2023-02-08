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
    private InputAction damageCheat;
    private InputAction healCheat;

    [Header("Cheats")]
    [SerializeField] private GameObject godCheatNotification;
    [SerializeField] private float cheatDamage = 25;
    [SerializeField] private float cheatHeal = 25;
    

    private void Start()
    {
        c = GameManager.controls;
        godCheat = c.PlayerGameplay.GodCheat;
        godCheat.performed += ToggleGodmode;

        healCheat = c.PlayerGameplay.HealCheat;
        damageCheat = c.PlayerGameplay.DamageCheat;

        healCheat.performed += CheatHeal;
        damageCheat.performed += CheatDamage;

        if (godCheatNotification != null)
            godCheatNotification.SetActive(_healthManager.God);
    }

    protected override void KillTarget()
    {
        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
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

    private void CheatHeal(InputAction.CallbackContext ctx = default)
    {
        _healthManager.Heal(cheatHeal);
    }

    private void CheatDamage(InputAction.CallbackContext ctx = default)
    {
        _healthManager.Damage(cheatDamage);
    }

    private void OnDisable()
    {
        godCheat.performed -= ToggleGodmode;
        healCheat.performed -= CheatHeal;
        damageCheat.performed -= CheatDamage;
    }
}
