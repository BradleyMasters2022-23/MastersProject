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

public class PlayerTarget : Target, IDifficultyObserver
{
    /// <summary>
    /// Current player instance
    /// </summary>
    public static PlayerTarget p;

    [Tooltip("The main UI for this player object. Should be in children")]
    [SerializeField] private Canvas mainUI;
    public Canvas MainUI { get { return mainUI; }}

    [Header("Death State")]
    [Tooltip("Animator controlling player death animation")]
    [SerializeField] Animator playerDeathAnimator;
    [Tooltip("Animator controlling player gun. Needed for start of death animation.")]
    [SerializeField] Animator playerGunController;
    [Tooltip("Channel used to reset player look on death")]
    [SerializeField] AimController aimController;
    [Tooltip("Channel called when the player immediately dies")]
    [SerializeField] ChannelVoid onPlayerKilled;

    [Header("Player Damage Flinch")]
    [Tooltip("Lookup table for damages and impulse strength")]
    [SerializeField] DamageToImpulse[] impulseRanges;
    [Tooltip("The impulse source to utilize")]
    [SerializeField] CinemachineImpulseSource impulse;
    /// <summary>
    /// The damage taken in the last frame
    /// </summary>
    private float frameDamage;
    /// <summary>
    /// Cooldown tracker for impulse
    /// </summary>
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
        if(frameDamage > 0 && impulseCD.TimerDone() && !_killed)
        {
            ApplyDamageImpulse(frameDamage);
            impulseCD.ResetTimer();
        }
        frameDamage = 0;

    }

    #region Player Death

    [SerializeField] AudioClipSO immediateDeathSound;
    [SerializeField] AudioClipSO deathHitGroundSound;
    [SerializeField] AmbientSFXSource playerWeaponSoundManager;

    /// <summary>
    /// On kill, go into game over state
    /// </summary>
    protected override void KillTarget()
    {
        _killed = true;

        InputManager.Controls.PlayerGameplay.Disable();

        playerGunController.enabled = false;
        _rb.velocity = Vector3.zero;
        
        playerDeathAnimator.enabled = true;
        aimController.ResetLook();
    }

    /// <summary>
    /// Call channel that player died. Called by animator.
    /// </summary>
    protected void CallPlayerKilled()
    {
        onPlayerKilled.RaiseEvent();
    }

    /// <summary>
    /// Call the game to game over. Called via player animator
    /// </summary>
    protected virtual void CallGameOver()
    {
        GlobalStatsManager.data.playerDeaths++;
        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
        Time.timeScale = 0;
    }

    private void PlayDeathAudio(int stage)
    {
        switch (stage)
        {
            case 0:
                {
                    immediateDeathSound.PlayClip(audioSource);
                    break;
                }
            case 1:
                {
                    deathHitGroundSound.PlayClip(audioSource);
                    break;
                }
            case 2:
                {
                    playerWeaponSoundManager.Play();
                    break;
                }
                case 3:
                {
                    playerWeaponSoundManager.Stop();
                    break;
                }
        }
    }

    #endregion

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
    public override void RegisterEffect(float dmg, Vector3 origin)
    {
        // log damage taken to damage taken this frame
        dmg *= difficultyPlayerDamageMod;
        frameDamage += dmg;
        base.RegisterEffect(dmg, origin);
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

        // dont do disable funcs if not the instanced version
        if (p != this) return;

        godCheat.performed -= ToggleGodmode;
        healCheat.performed -= CheatHeal;
        damageCheat.performed -= CheatDamage;
        tiedye.performed -= Tiedye;

        p = null;

        GlobalDifficultyManager.instance.Unsubscribe(this, difficultyPlayerVulnerabilityMod);
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

    #region Difficulty Settings

    /// <summary>
    /// player damage modifier managed by difficulty
    /// </summary>
    private float difficultyPlayerDamageMod = 1;
    /// <summary>
    /// key used to lookup difficulty setting
    /// </summary>
    private const string difficultyPlayerVulnerabilityMod = "PlayerDamageVulnerability";

    /// <summary>
    /// Update player damage modifier
    /// </summary>
    /// <param name="newModifier">New damage taken modifier</param>
    public void UpdateDifficulty(float newModifier)
    {
        difficultyPlayerDamageMod = newModifier;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GlobalDifficultyManager.instance.Subscribe(this, difficultyPlayerVulnerabilityMod);
    }

    #endregion
}
