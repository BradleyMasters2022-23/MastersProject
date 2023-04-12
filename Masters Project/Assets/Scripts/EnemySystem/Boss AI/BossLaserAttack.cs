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
    private float currentInterval;

    private void Awake()
    {
        tracker = new ScaledTimer(1f, true);
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

            tracker.ResetTimer(currentInterval);
        }
    }

    public void PhaseChange(int newPhase)
    {
        Inturrupt();

        phase1Cannon.NewStage(newPhase);
        phase2Cannon.NewStage(newPhase);

        tracker.ResetTimer(phaseChangeCooldown);
    }

    public void Inturrupt()
    {
        if (phase1Cannon.isActiveAndEnabled)
        {
            phase1Cannon.Inturrupt();
        }
        if (phase2Cannon.isActiveAndEnabled)
        {
            phase2Cannon.Inturrupt();
        }
    }
}
