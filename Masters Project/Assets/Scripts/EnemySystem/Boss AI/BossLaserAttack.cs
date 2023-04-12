using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossLaserAttack : MonoBehaviour
{
    [SerializeField] BFLController phase1Cannon;
    [SerializeField] BFLController phase2Cannon;

    [SerializeField] float phaseChangeCooldown;

    private ScaledTimer tracker;

    private void Awake()
    {
        tracker = new ScaledTimer(phaseChangeCooldown, true);
    }

    private void Update()
    {
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


        tracker?.ResetTimer();
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
    }
}
