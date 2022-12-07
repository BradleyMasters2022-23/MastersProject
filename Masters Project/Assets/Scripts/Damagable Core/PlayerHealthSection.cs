/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 21th, 2022
 * Last Edited - October 21th, 2022 by Ben Schuster
 * Description - Functionality for each independent player health section
 * ================================================================================================
 */
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHealthSection : MonoBehaviour
{
    public enum HealthSectionState
    {
        IDLE,
        EMPTIED,
        REGENERATING,
        FORCEHEAL
    }

    /// <summary>
    /// What is this segment's current state.
    /// </summary>
    private HealthSectionState currentState;
    /// <summary>
    /// What is this segment's current state.
    /// </summary>
    public HealthSectionState CurrentState
    {
        get { return currentState; }
    }

    /// <summary>
    /// Reference to the core health controller.
    /// </summary>
    private PlayerHealth healthController;

    #region health variables

    /// <summary>
    /// Max health of this segment
    /// </summary>
    private float maxHealth;
    /// <summary>
    /// current health of this segment
    /// </summary>
    private float currHealth;
    /// <summary>
    /// current health of this segment
    /// </summary>
    public float CurrHealth
    {
        get { return currHealth; }
    }

    /// <summary>
    /// Time it takes for the player to passively regenerate this segment.
    /// </summary>
    private float passiveRegenTime;

    /// <summary>
    /// Time the player needs to be out of combat before regenerating segment.
    /// </summary>
    private ScaledTimer passiveRegenDelay;

    /// <summary>
    /// Time it takes for the player to actively regenerate this segment (such as a healing kit).
    /// </summary>
    private float activeRegenTime;

    /// <summary>
    /// How much this current state of regeneration has recovered
    /// </summary>
    private float cappedRegenBuffer;

    /// <summary>
    /// Initialize the health segment
    /// </summary>
    /// <param name="_controller">Health controller to report back to</param>
    /// <param name="_max">Max health of this segment</param>
    /// <param name="_passRegenTime">Time it takes to passively regenerate health</param>
    /// <param name="_passRegenDelay">Time needed out of combat before regenerating</param>
    /// <param name="_actRegenTime">Time it takes to actively regenerate health</param>
    public void InitializeSection(PlayerHealth _controller, int _curr, int _max, float _passRegenTime, float _passRegenDelay, float _actRegenTime)
    {
        healthController = _controller;
        currHealth = _curr;
        maxHealth = _max;
        passiveRegenTime = _passRegenTime;
        passiveRegenDelay = new ScaledTimer(_passRegenDelay, false);
        activeRegenTime = _actRegenTime;

        cappedRegenBuffer = 0;

        if (currHealth <= 0)
            ChangeState(HealthSectionState.EMPTIED);
    }

    #endregion

    /// <summary>
    /// Deal damage to this segment.
    /// </summary>
    /// <param name="_dmg">damage being dealt</param>
    /// <returns>Whether or not this healthbar is emptied</returns>
    public bool TakeDamage(int _dmg)
    {
        // Reset timer, set state to idle
        // TODO - implement other "out of combat" strat
        passiveRegenDelay.ResetTimer();
        if(currentState == HealthSectionState.REGENERATING)
        {
            ChangeState(HealthSectionState.IDLE);
        }

        if(currHealth - _dmg <= 0)
        {
            currHealth = 0;
            ChangeState(HealthSectionState.EMPTIED);
            return true;
        }
        else
        {
            currHealth -= _dmg;
            return false;
        }
    }

    private void FixedUpdate()
    {
        StateUpdateFunction();
    }

    #region states

    /// <summary>
    /// Do any state-exclusive update functionality
    /// </summary>
    private void StateUpdateFunction()
    {
        switch (currentState)
        {
            case HealthSectionState.IDLE:
                {
                    // If missing health and delay is done, start regenerating
                    if(!IsMaxed() && passiveRegenDelay.TimerDone())
                    {
                        ChangeState(HealthSectionState.REGENERATING);
                    }

                    break;
                }
            case HealthSectionState.EMPTIED:
                {
                    break;
                }
            case HealthSectionState.REGENERATING:
                {
                    // Regnerate health. If at max, return to idle
                    Regenerate(passiveRegenTime);
                    if(IsMaxed())
                    {
                        ChangeState(HealthSectionState.IDLE);
                    }

                    break;
                }
            case HealthSectionState.FORCEHEAL:
                {
                    // Return to idle if it has healed the full segment amount
                    Regenerate(activeRegenTime);
                    if (IsMaxed() || cappedRegenBuffer >= maxHealth)
                    {
                        ChangeState(HealthSectionState.IDLE);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// Change the current state, activate any cross-section functionality
    /// </summary>
    /// <param name="_newState">new state to go to</param>
    private void ChangeState(HealthSectionState _newState)
    {
        switch(_newState)
        {
            case HealthSectionState.IDLE:
                {
                    // When returning to idle, reset the passive regeneration delay timer
                    passiveRegenDelay.ResetTimer();

                    break;
                }
            case HealthSectionState.EMPTIED:
                {
                    break;
                }
            case HealthSectionState.REGENERATING:
                {
                    break;
                }
            case HealthSectionState.FORCEHEAL:
                {
                    // When doing a new heal, reset healing buffer
                    cappedRegenBuffer = 0;
                    break;
                }
        }

        currentState = _newState;
    }

    public void ChipChangeState(HealthSectionState _newState)
    {
        ChangeState(_newState);
    }

    public HealthSectionState GetState()
    {
        return currentState;
    }

    #endregion

    /// <summary>
    /// Regenerate health segment
    /// </summary>
    /// <param name="_healRate">Time it takes to heal (from empty)</param>
    private void Regenerate(float _healRate)
    {
        // calculate amount to recharge, rounding down. Add to buffer
        float replenishAmount = (maxHealth / _healRate) / 50;
        cappedRegenBuffer += replenishAmount;

        // Replenish the gauge, determine if state should change
        if (currHealth + replenishAmount >= maxHealth)
        {
            currHealth = maxHealth;
        }
        else
        {
            currHealth += replenishAmount;
        }
    }

    /// <summary>
    /// Force this segment of health to regenerate its max value
    /// </summary>
    public void ForceHeal()
    {
        ChangeState(HealthSectionState.FORCEHEAL);
    }

    /// <summary>
    /// Get current health segment value
    /// </summary>
    /// <returns>Whether or not segment is max health</returns>
    public bool IsMaxed()
    {
        return currHealth == maxHealth;
    }

    public void SetMaxHealth(int _maxHealth) {
        maxHealth = _maxHealth;
        currHealth = _maxHealth;
    }
}
