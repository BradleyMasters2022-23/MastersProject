using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossLaserAttack : TimeAffectedEntity
{
    [SerializeField] BFLController phase1Cannon;
    [SerializeField] BFLController phase2Cannon;

    [SerializeField] float phaseChangeCooldown;
    [SerializeField] float inturruptCooldown;

    private LocalTimer phaseChangeCooldownTracker;

    private void Awake()
    {
        phaseChangeCooldownTracker = GetTimer(phaseChangeCooldown);
    }

    private void Update()
    {
        if(phaseChangeCooldownTracker.TimerDone())
        {
            if (phase1Cannon.CanAttack() && !phase2Cannon.CurrentlyAttacking())
            {
                phase1Cannon.ChooseAttack();
            }
            if (phase2Cannon.CanAttack() && !phase1Cannon.CurrentlyAttacking())
            {
                phase2Cannon.ChooseAttack();
            }
        }
    }

    public void PhaseChange(int newPhase)
    {
        //Inturrupt();

        phase1Cannon?.NewStage(newPhase);
        phase2Cannon?.NewStage(newPhase);


        phaseChangeCooldownTracker?.ResetTimer(phaseChangeCooldown);
    }

    public void Inturrupt()
    {
        if (phase1Cannon.isActiveAndEnabled)
        {
            phase1Cannon?.Inturrupt();
        }
        if (phase2Cannon.isActiveAndEnabled)
        {
            phase2Cannon?.Inturrupt();
        }

        phaseChangeCooldownTracker?.ResetTimer(inturruptCooldown);
    }
}
