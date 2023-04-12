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

    private void Start()
    {
        cooldown = new ScaledTimer(0);
    }

    private void Update()
    {
        if(active && currentAttack != null)
        {
            if(currentAttack.AttackDone())
            {
                Debug.Log("Attack done");
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
    }

    public void ChooseAttack()
    {
        if (CanAttack())
        {
            Debug.Log("Attacking");
            active = true;
            currentAttack = attacks.Pull();
            currentAttack.Attack();
        }
    }

    public void Inturrupt()
    {
        currentAttack?.CancelAttack();
        currentAttack = null;
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
