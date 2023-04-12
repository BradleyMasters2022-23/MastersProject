using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BFLController : MonoBehaviour
{
    [SerializeField] private GenericWeightedList<BaseBossAttack> attacks;

    [SerializeField] private List<Vector2> stageCooldowns;

    [SerializeField, ReadOnly] private int currentStage;

    private bool active;
    private ScaledTimer cooldown;
    private BaseBossAttack currentAttack;

    private void Start()
    {
        cooldown = new ScaledTimer(0);
    }

    private void Update()
    {
        if(currentAttack != null)
        {
            if(currentAttack.AttackDone())
            {
                currentAttack = null;
                SetCooldown();
            }
        }

        if(onCooldown && cooldown.TimerDone())
            onCooldown= false;
    }

    public void NewStage(int stageNum)
    {
        currentStage = stageNum;
    }

    public void ChooseAttack()
    {
        if (active || !cooldown.TimerDone()) return;

        Debug.Log("Attack chosen");

        active = true;
        currentAttack = attacks.Pull();
        currentAttack.Attack();
    }

    public void Inturrupt()
    {
        currentAttack?.CancelAttack();
        currentAttack = null;
    }

    private void SetCooldown()
    {
        float num = Random.Range(
            stageCooldowns[currentStage].x,
            stageCooldowns[currentStage].y);

        cooldown.ResetTimer(num);

        onCooldown = true;
    }

    public bool CanAttack()
    {
        return cooldown.TimerDone();
    }
}
