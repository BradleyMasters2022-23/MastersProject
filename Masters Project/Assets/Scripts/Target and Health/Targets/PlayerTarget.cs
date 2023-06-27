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
using UnityEngine.SceneManagement;
using Cinemachine;

[System.Serializable]
public struct DamageToImpulse
{
    public float minDamage;
    public float maxDamage;
    public float impulse;

    public bool WithinRange(float dmg)
    {
        return (dmg <= maxDamage) && (dmg >= minDamage);
    }
}

public class PlayerTarget : Target
{
    public static PlayerTarget p;


    [Header("Player Damage Impulse")]
    [SerializeField] DamageToImpulse[] impulseRanges;
    [SerializeField] CinemachineImpulseSource impulse;

    [Header("Cheats")]
    [SerializeField] private GameObject godCheatNotification;
    [SerializeField] private float cheatDamage = 25;
    [SerializeField] private float cheatHeal = 25;
    [SerializeField] protected TimeManager timestop;
    [SerializeField] private Ability grenadeAbility;

    private GameControls c;
    private InputAction godCheat;
    private InputAction damageCheat;
    private InputAction healCheat;
    private bool godMode = false;

    protected virtual void Start()
    {
        if (p == null)
        {
            p = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        GlobalStatsManager.data.playerDeaths++;
        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
    }

    private void ToggleGodmode(InputAction.CallbackContext ctx = default)
    {
        godMode = !godMode;

        if (_healthManager != null)
            _healthManager.ToggleGodmode(godMode);

        if (godCheatNotification != null)
            godCheatNotification.SetActive(godMode);

        if (timestop != null)
        {
            timestop.SetCheatMode(godMode);
        }

        if (grenadeAbility != null)
        {
            grenadeAbility.CheatMode(godMode);
        }
    }

    private void CheatHeal(InputAction.CallbackContext ctx = default)
    {
        _healthManager.Heal(cheatHeal);
    }

    private void CheatDamage(InputAction.CallbackContext ctx = default)
    {
        _healthManager.Damage(cheatDamage);
    }

    public override void Knockback(float force, float verticalForce, Vector3 origin)
    {
        if (immuneToKnockback || (force + verticalForce <= 0))
        {
            return;
        }
            

        // kick the player up a tiny bit to reduce any ground drag
        transform.position += Vector3.up * 0.25f;
        base.Knockback(force, verticalForce, origin + Vector3.up * 0.5f);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        godCheat.performed -= ToggleGodmode;
        healCheat.performed -= CheatHeal;
        damageCheat.performed -= CheatDamage;
        p = null;
    }

    public override void RegisterEffect(float dmg)
    {
        base.RegisterEffect(dmg);
        float impactForce = 1;

        // use damage to determine which impulse effect to use
        for(int i = 0; i < impulseRanges.Length; i++)
        {
            if(impulseRanges[i].WithinRange(dmg))
            {
                impactForce = impulseRanges[i].impulse;
            }
        }

        impulse?.GenerateImpulseWithForce(impactForce);
    }
}
