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

    private ScaledTimer tracker;

    private void Awake()
    {
        tracker = new ScaledTimer(phaseChangeCooldown, false);
    }

    private void Update()
    {
        tracker?.SetModifier(Timescale);

        if(tracker.TimerDone())
        {
            if (phase1Cannon.isActiveAndEnabled)
            {
                phase1Cannon.ChooseAttack();
            }
            if (phase2Cannon.isActiveAndEnabled)
            {
                phase2Cannon.ChooseAttack();
            }

        }
    }

    public void PhaseChange(int newPhase)
    {
        Inturrupt();

        phase1Cannon?.NewStage(newPhase);
        phase2Cannon?.NewStage(newPhase);


        tracker?.ResetTimer(phaseChangeCooldown);
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

        tracker?.ResetTimer(inturruptCooldown);
    }
}
