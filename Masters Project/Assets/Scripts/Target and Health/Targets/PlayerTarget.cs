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
    /// <summary>
    /// Current player instance
    /// </summary>
    public static PlayerTarget p;

    [Header("Player Damage Flinch")]
    [Tooltip("Lookup table for damages and impulse strength")]
    [SerializeField] DamageToImpulse[] impulseRanges;
    [Tooltip("The impulse source to utilize")]
    [SerializeField] CinemachineImpulseSource impulse;
    /// <summary>
    /// The damage taken in the last frame
    /// </summary>
    private float frameDamage;
    private ScaledTimer impulseCD;
    /// <summary>
    /// Init instance, apply cheat inputs
    /// </summary>
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

        impulseCD = new ScaledTimer(0.15f, false);

        c = GameManager.controls;
        godCheat = c.PlayerGameplay.GodCheat;
        godCheat.performed += ToggleGodmode;

        healCheat = c.PlayerGameplay.HealCheat;
        damageCheat = c.PlayerGameplay.DamageCheat;

        healCheat.performed += CheatHeal;
        damageCheat.performed += CheatDamage;

        tiedye = c.PlayerGameplay.TieDye;
        tiedye.performed += Tiedye;

        if (godCheatNotification != null)
            godCheatNotification.SetActive(_healthManager.God);
    }

    /// <summary>
    /// At the end of each frame, apply the impulse based on damage taken this entire frame
    /// </summary>
    private void LateUpdate()
    {
        if(frameDamage > 0 && impulseCD.TimerDone())
        {
            ApplyDamageImpulse(frameDamage);
            impulseCD.ResetTimer();
        }
        frameDamage = 0;

    }

    /// <summary>
    /// On kill, go into game over state
    /// </summary>
    protected override void KillTarget()
    {
        GlobalStatsManager.data.playerDeaths++;
        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
    }

    /// <summary>
    /// Add offset to the player before knockback to make it work
    /// </summary>
    /// <param name="force">force to apply</param>
    /// <param name="verticalForce">additional vertical force to apply</param>
    /// <param name="origin">source of the force</param>
    public override void Knockback(float force, float verticalForce, Vector3 origin)
    {
        if (immuneToKnockback || (force + verticalForce <= 0))
        {
            return;
        }
            

        // kick the player up a tiny bit to reduce any ground drag
        transform.position += Vector3.up * 0.5f;
        base.Knockback(force, verticalForce, origin + Vector3.up * 0.5f);
    }

    /// <summary>
    /// In addition to taking damage, also log damage taken
    /// </summary>
    /// <param name="dmg">damage to apply</param>
    public override void RegisterEffect(float dmg)
    {
        // log damage taken to damage taken this frame
        frameDamage += dmg;
        base.RegisterEffect(dmg);

    }

    /// <summary>
    /// Apply an impact force based on amount of damage passed in
    /// </summary>
    /// <param name="dmg">Damage taken</param>
    private void ApplyDamageImpulse(float dmg)
    {
        if (godMode) return;

        float impactForce = 1;
        // use damage to determine which impulse effect to use
        for (int i = 0; i < impulseRanges.Length; i++)
        {
            if (impulseRanges[i].WithinRange(dmg))
            {
                impactForce = impulseRanges[i].impulse;
            }
        }
        impulse?.GenerateImpulseWithForce(impactForce);
    }

    #region Cheats

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
    private InputAction tiedye;

    public Material tieDyeMat;

    private bool godMode = false;
    protected override void OnDisable()
    {
        base.OnDisable();

        godCheat.performed -= ToggleGodmode;
        healCheat.performed -= CheatHeal;
        damageCheat.performed -= CheatDamage;
        tiedye.performed -= Tiedye;

        p = null;
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

    public void Tiedye(InputAction.CallbackContext ctx = default)
    {
        MeshRenderer[] beegTemp = FindObjectsOfType<MeshRenderer>(true);
        Debug.Log($"Got {beegTemp.Length} items to tiedye");

        foreach(var ren in beegTemp)
        {
            ren.material = tieDyeMat;
        }

        SkinnedMeshRenderer[] temp = FindObjectsOfType<SkinnedMeshRenderer>(true);
        foreach(var ren in temp)
        {
            ren.material = tieDyeMat;
        }
    }

    #endregion
}
