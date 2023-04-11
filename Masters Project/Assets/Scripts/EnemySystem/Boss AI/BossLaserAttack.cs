using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossLaserAttack : MonoBehaviour
{
    [SerializeField] BossCannonController phase1Cannon;
    [SerializeField] BossCannonController phase2Cannon;

    private bool canAttack;

    [SerializeField] float phaseChangeCooldown;
    [SerializeField] float attackIntervalPhase1;
    [SerializeField] float attackIntervalPhase2;
    [SerializeField] float attackIntervalPhase3;
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

        switch (newPhase)
        {
            case 1:
                {
                    currentInterval = attackIntervalPhase1;
                    break;
                }
            case 2:
                {
                    currentInterval = attackIntervalPhase2;
                    break;
                }
            case 3:
                {
                    currentInterval = attackIntervalPhase3;
                    break;
                }
        }

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
