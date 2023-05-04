using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BFLController : MonoBehaviour
{
    [SerializeField] private GenericWeightedList<BaseBossAttack> attacks;
    [SerializeField] private List<Vector2> stageCooldowns;
    [SerializeField, ReadOnly] private int currentStage;
    [SerializeField] private bool onCooldown = false;
    [SerializeField] private bool active = false;
    private ScaledTimer cooldown;
    private BaseBossAttack currentAttack;

    [SerializeField] private AudioClipSO powerDownIndicators;
    [SerializeField] private AudioClipSO powerUpIndicators;

    [SerializeField] private AudioSource source;

    [SerializeField] GameObject mainUnit;
    [SerializeField] GameObject deactivatedUnit;

    private bool disabled;

    private void Start()
    {
        //source = GetComponent<AudioSource>();
        cooldown = new ScaledTimer(0);
        SetCooldown();
    }

    private void Update()
    {
        if(active && currentAttack != null)
        {
            if(currentAttack.AttackDone())
            {
                //Debug.Log("Attack done");
                currentAttack = null;
                active = false;
                SetCooldown();
            }
        }

        if(onCooldown && cooldown.TimerDone())
        {
            //Debug.Log("Cooldown done");
            onCooldown = false;
        }
            
    }

    public void NewStage(int stageNum)
    {
        currentStage = stageNum;
        powerUpIndicators.PlayClip(source);
    }

    public void ChooseAttack()
    {
        if (CanAttack())
        {
            //Debug.Log("Attack selected");
            active = true;
            currentAttack = attacks.Pull();
            currentAttack.Attack();
        }
    }

    public void Inturrupt()
    {
        //Debug.Log($"{name} Inturrupt BFL called");
        powerDownIndicators.PlayClip(source);
        currentAttack?.CancelAttack();
        SetCooldown();
        currentAttack = null;
        active= false;
        //attacks.Pull().RotateToEnabledState();
    }

    private void SetCooldown()
    {
        onCooldown = true;

        float num = Random.Range(
            stageCooldowns[currentStage].x,
            stageCooldowns[currentStage].y);

        cooldown.ResetTimer(num);
    }

    public bool CanAttack()
    {
        return (!onCooldown && !active && !disabled);
    }

    public void DisableBFL()
    {
        //Debug.Log("Disable BFL Called");
        disabled = true;
        mainUnit.SetActive(false);
        deactivatedUnit.SetActive(true);
        attacks.Pull().RotateToDisabledState();
    }

    public void EnableBFL()
    {
        attacks.Pull().RotateToEnabledState();
        mainUnit.SetActive(true);
        deactivatedUnit.SetActive(false);
        disabled = false;
    }
}
