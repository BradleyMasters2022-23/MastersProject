using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BFLController : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] private GenericWeightedList<BaseBossAttack> attacks;
    [SerializeField] private List<Vector2> stageCooldowns;
    [SerializeField, ReadOnly] private int currentStage;
    private BFLTarget target;

    [Header("Status")]
    [SerializeField, ReadOnly] private bool onCooldown = false;
    [SerializeField, ReadOnly] private bool currentAttackActive = false;
    [SerializeField, ReadOnly] private bool activeThisPhase = false;
    [SerializeField, ReadOnly] private bool disabled;
    /// <summary>
    /// cooldown tracker 
    /// </summary>
    private ScaledTimer cooldown;
    /// <summary>
    /// current attack loaded
    /// </summary>
    private BaseBossAttack currentAttack;

    [Header("Audio")]
    [Tooltip("SFX for powering down")]
    [SerializeField] private AudioClipSO powerDownIndicators;
    [Tooltip("SFX for powering up")]
    [SerializeField] private AudioClipSO powerUpIndicators;
    [SerializeField] private AudioSource source;

    [Header("Skins")]
    [Tooltip("Skin used while active")]
    [SerializeField] GameObject mainUnit;
    [Tooltip("Skin used while deactivated")]
    [SerializeField] GameObject deactivatedUnit;

    private void Start()
    {
        target = GetComponent<BFLTarget>();
        cooldown = new ScaledTimer(0);
        GoToCooldown();
    }

    private void Update()
    {
        if(currentAttackActive&& currentAttack != null)
        {
            if(currentAttack.AttackDone())
            {
                currentAttack = null;
                currentAttackActive= false;
                GoToCooldown();
            }
        }

        if(onCooldown && cooldown.TimerDone())
        {
            onCooldown = false;
        }
            
    }

    public void NewStage(int stageNum)
    {
        currentStage = stageNum;
        powerUpIndicators.PlayClip(source);
        target.NewPhase();
    }

    public void ChooseAttack()
    {
        if (CanAttack())
        {
            currentAttackActive= true;
            currentAttack = attacks.Pull();
            currentAttack.Attack();
        }
    }

    public void Inturrupt()
    {
        //Debug.Log($"{name} Inturrupt BFL called");
        powerDownIndicators.PlayClip(source);
        currentAttack?.CancelAttack();
        GoToCooldown();
        currentAttack = null;
        currentAttackActive = false;
        //attacks.Pull().RotateToEnabledState();
    }

    private void GoToCooldown()
    {
        onCooldown = true;

        float num = Random.Range(
            stageCooldowns[currentStage].x,
            stageCooldowns[currentStage].y);

        cooldown.ResetTimer(num);
    }

    public bool CanAttack()
    {
        return (!onCooldown && !currentAttackActive && !disabled);
    }

    public void DisableBFL()
    {
        //Debug.Log("Disable BFL Called");
        disabled = true;
        mainUnit.SetActive(false);
        deactivatedUnit.SetActive(true);
        attacks.Pull().RotateToDisabledState();

        target.SetHealthbarStatus(false);
    }

    public void EnableBFL()
    {
        attacks.Pull().RotateToEnabledState(()=>disabled = false);
        mainUnit.SetActive(true);
        deactivatedUnit.SetActive(false);
        target.SetHealthbarStatus(true);
    }

    public void SetPhaseStatus(bool active)
    {
        activeThisPhase = active;
    }
    public bool EnabledThisPhase()
    {
        return activeThisPhase;
    }

    public bool CurrentlyAttacking()
    {
        return (currentAttackActive && !disabled);
    }
}
