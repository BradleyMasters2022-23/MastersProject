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

    private void Start()
    {
        source = GetComponent<AudioSource>();
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
            onCooldown = false;
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
            //Debug.Log("Attacking");
            active = true;
            currentAttack = attacks.Pull();
            currentAttack.Attack();
        }
    }

    public void Inturrupt()
    {
        powerDownIndicators.PlayClip(source);
        currentAttack?.CancelAttack();
        currentAttack = null;
        active= false;
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
        return (!onCooldown && !active);
    }
}
