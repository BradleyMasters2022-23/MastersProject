using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : TimeAffectedEntity
{
    [Header("Boss Stuff")]

    [Tooltip("Sets of attack that can be used by phase")]
    [SerializeField] private GenericWeightedList<BossAttack>[] phaseAttacks = new GenericWeightedList<BossAttack>[3];

    /// <summary>
    /// Current phase to use attacks from
    /// </summary>
    private int currentPhase;

    [Tooltip("Range of cooldown after each attack")]
    [SerializeField] private Vector2 cooldownRange;

    /// <summary>
    /// Tracker for cooldown
    /// </summary>
    private ScaledTimer cooldownTracker;

    /// <summary>
    /// Reference to target 
    /// </summary>
    private Transform target;
    /// <summary>
    /// Reference to current attack
    /// </summary>
    private BossAttack currAttack;

    private bool disabled;

    /// <summary>
    /// Setup for main boss
    /// </summary>
    protected void Awake()
    {
        currentPhase = 0;
        cooldownTracker = new ScaledTimer(0f, false);
    }

    
    /// <summary>
    /// Main controller routine for AI
    /// </summary>
    /// <returns></returns>
    protected IEnumerator MainAIController()
    {
        WaitForFixedUpdate tick = new WaitForFixedUpdate();

        while (true)
        {
            // update cooldown tracker
            cooldownTracker.SetModifier(Timescale);

            // Try attacking on cooldown 
            if(cooldownTracker.TimerDone() && !disabled)
            {
                BossAttack chosenAttack = phaseAttacks[currentPhase].Pull();

                // if chosen attack on cooldown, do another pull
                if(!chosenAttack.CanDoAttack())
                {
                    yield return tick;
                    continue;
                }
                // Otherwise, do the new attack
                else
                {
                    yield return StartCoroutine(HandleAttack(chosenAttack));
                    cooldownTracker.ResetTimer(Random.Range(cooldownRange.x, cooldownRange.y));
                }
            }


            yield return tick;
        }
    }

    /// <summary>
    /// Routine for controlling attacks
    /// </summary>
    /// <param name="atk">attack to perform</param>
    /// <returns></returns>
    private IEnumerator HandleAttack(BossAttack atk)
    {
        WaitForFixedUpdate tick = new WaitForFixedUpdate();

        currAttack = atk;

        atk.Attack(target);
        while (atk.currentAttackState != AttackState.Ready)
        {
            yield return tick;
        }

        currAttack = null;

        yield return null;
        // Debug.Log("Attack finished");
    }

    /// <summary>
    /// Start the boss AI
    /// </summary>
    public void InitBoss()
    {
        target = PlayerTarget.p.Center;
        Debug.Log("init boss called");
        // DO INITIAL STUFF HERE LIKE MONOLOGUE
        StartCoroutine(MainAIController());
    }

    /// <summary>
    /// Inturrupt the boss' current attack
    /// </summary>
    public void Inturrupt()
    {
        // If attack exists, reset it.
        currAttack?.ResetAttack();
        cooldownTracker.ResetTimer(cooldownRange.y);
        // Do anything else like audio here
    }

    public void SetPhase(int phase)
    {
        currentPhase= phase;
    }

    public void DisableBoss()
    {
        disabled = true;
        if(currAttack!= null)
        {
            Debug.Log("Attack found, trying to disable");
            currAttack.ResetAttack();
        }
    }
    public void EnableBoss()
    {
        disabled = false;
    }

    public bool GetAfflicted()
    {
        return Affected;
    }
    public float GetTimescale()
    {
        return Timescale;
    }
    public float GetDeltatime()
    {
        return DeltaTime;
    }
}
