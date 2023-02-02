using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerHealthSection;

public class ResourceBar : MonoBehaviour
{
    protected enum State
    {
        FULL, 
        IDLE,
        EMPTIED,
        REGENERATING
    }

    [SerializeField] private State _currState;

    [SerializeField] private float _maxAmount;
    [SerializeField] private float _currAmount;

    [SerializeField] private bool _regen;
    [SerializeField] private float _regenRate;
    [SerializeField] private float _regenDelay;

    private ScaledTimer _regenTracker;

    private void Start()
    {
        if (_regen)
            _regenTracker = new ScaledTimer(_regenDelay, false);

        // Set current state
        NonRegenStateCheck();

        InvokeRepeating("TestDamage", 5, 2);
    }

    public void TestDamage()
    {
        ModifyCurrent(-10);
    }

    public void Init(float max, float curr, bool regen, float reganRate, float regenDelay)
    {
        _maxAmount= max;
        _currAmount= curr;
        _regen = regen;
        _regenRate= reganRate;
        _regenDelay= regenDelay;

        if (_regen)
            _regenTracker = new ScaledTimer(_regenDelay, false);

        // Set current state
        NonRegenStateCheck();
    }

    private void FixedUpdate()
    {
        StateUpdateFunction();
    }

    #region State Functionality

    /// <summary>
    /// Check what the current state should be, resetting any potential states.
    /// Used for taking damage or finishing healing.
    /// </summary>
    private void NonRegenStateCheck()
    {
        // Set current state
        if (_currAmount == _maxAmount)
        {
            ChangeState(State.FULL);
        }
        else if (_currAmount <= 0)
        {
            ChangeState(State.EMPTIED);
        }
        else
        {
            ChangeState(State.IDLE);
        }
    }

    /// <summary>
    /// Change the current state, activate any cross-section functionality
    /// </summary>
    private void StateUpdateFunction()
    {
        switch (_currState)
        {
            case State.FULL:
                {
                    break;
                }
            case State.IDLE:
                {
                    if(CheckRegen())
                    {
                        ChangeState(State.REGENERATING);
                    }

                    break;
                }
            case State.EMPTIED:
                {
                    if (CheckRegen())
                    {
                        ChangeState(State.REGENERATING);
                    }

                    break;
                }
            case State.REGENERATING:
                {
                    RegenTick();
                    if (IsFull())
                        ChangeState(State.FULL);

                    break;
                }
        }
    }


    /// <summary>
    /// Change the state and perform any cross-function functionality
    /// </summary>
    /// <param name="newState">New state to move to</param>
    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.FULL:
                {
                    ResetRegenRate();
                    break;
                }
            case State.IDLE:
                {
                    ResetRegenRate();
                    break;
                }
            case State.EMPTIED:
                {
                    ResetRegenRate();
                    break;
                }
            case State.REGENERATING:
                {
                    break;
                }
        }

        _currState = newState;
    }

    

    #endregion

    #region Current Value Modifying Functions

    /// <summary>
    /// Change this resource bar's current value. 
    /// Pass a positive value to increase or a negative value to decrease
    /// </summary>
    /// <param name="amt">Amount to increase or decrease by. sign dependent</param>
    /// <returns>Overflow from the change bar</returns>
    public float ModifyCurrent(float amt)
    {
        if(amt <= 0)
        {
            // quickly invert the sign for ease
            return -1 * Decrease(-amt);
        }
        else
        {
            return Increase(amt);
        }
    }

    /// <summary>
    /// Decrease the current gauge of this resource bar
    /// </summary>
    /// <param name="amt">amount to reduce this resource by</param>
    /// <returns>Overflow after decreasing this gauge to 0</returns>
    private float Decrease(float amt)
    {
        if(_currAmount - amt >= 0)
        {
            _currAmount -= amt;

            //ResetRegenRate();
            NonRegenStateCheck();
            return 0;
        }
        else
        {
            float overflow = amt - _currAmount;
            _currAmount = 0;

            //ResetRegenRate();
            NonRegenStateCheck();
            return overflow;
        }
    }
    /// <summary>
    /// Increase the current gauge of this resource bar
    /// </summary>
    /// <param name="amt">amount to increase this resource by</param>
    /// <returns>Overflow after increasing this gauge to max</returns>
    private float Increase(float amt)
    {
        if (_currAmount + amt <= _maxAmount)
        {
            _currAmount += amt;

            return 0;
        }
        else
        {
            float overflow = amt - (_maxAmount - _currAmount);
            _currAmount = _maxAmount;

            return overflow;
        }
    }

    #endregion

    #region Getters

    public bool IsFull()
    {
        return _currAmount == _maxAmount;
    }

    public bool IsEmptied()
    {
        return _currAmount <= 0;
    }

    #endregion

    #region Regenerating functions
    
    /// <summary>
    /// Check if this bar can regenerate naturally
    /// </summary>
    /// <returns>Whether this bar can begin regenerating</returns>
    private bool CheckRegen()
    {
        if(!_regen)
        {
            return false;
        }
        else
        {
            return _regenTracker.TimerDone();
        }
    }

    public void ResetRegenRate()
    {
        if(_regen)
            _regenTracker.ResetTimer();
    }

    /// <summary>
    /// Regenerate 1 tick of this resource bar
    /// </summary>
    private void RegenTick()
    {
        // If not allowed to regen, then exit
        if (!_regen)
            return;

        // calculate amount to recharge, rounding down. Add to buffer
        float replenishAmount = (_maxAmount / _regenRate) / 50;

        // Replenish the gauge, determine if state should change
        ModifyCurrent(replenishAmount);
    }

    #endregion

    #region UpgradeFunctions

    /// <summary>
    /// Increment the maximum of this resource bar
    /// </summary>
    /// <param name="increment">Amount to incremenent health by</param>
    /// <param name="increaseCurrent">Whether to increase the current by the same amount</param>
    public void IncreaseMax(float increment, bool increaseCurrent)
    {
        _maxAmount += increment;

        if(increaseCurrent)
        {
            _currAmount += increment;
        }

        NonRegenStateCheck();
    }

    /// <summary>
    /// Set the maximum of this resource bar
    /// </summary>
    /// <param name="max">New max to set the resource bar to</param>
    /// <param name="newCurr">New current value to set this bar to</param>
    public void SetMax(float max, float newCurr = -1)
    {
        _maxAmount = max;

        if(newCurr >= 0)
            _currAmount = Mathf.Clamp(newCurr, 0, _maxAmount);

        NonRegenStateCheck();
    }

    #endregion
}
