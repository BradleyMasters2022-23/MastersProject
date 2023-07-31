/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 31st, 2023
 * Last Edited - July 31st, 2023 by Ben Schuster
 * Description - Control health pool for player. Includes bonus functions for difficulty and polish
 * ================================================================================================
 */
using System.Collections.Generic;

public class PlayerHealthManager : HealthManager, IDifficultyObserver
{
    #region Healing Buffer

    /// <summary>
    /// Current buffer of incoming health
    /// </summary>
    private List<HealthOrb> healingBuffer = new List<HealthOrb>();

    /// <summary>
    /// Add an orb to potential healing buffer
    /// </summary>
    /// <param name="orb">Orb to add to buffer</param>
    public void AddToBuffer(HealthOrb orb)
    {
        if (!healingBuffer.Contains(orb))
        {
            healingBuffer.Add(orb);
        }

    }
    /// <summary>
    /// Remove orb from potential healing buffer
    /// </summary>
    /// <param name="orb">Orb to remove from buffer</param>
    public void RemoveFromBuffer(HealthOrb orb)
    {
        if (healingBuffer.Contains(orb))
        {
            healingBuffer.Remove(orb);
        }

    }
    /// <summary>
    /// Check if the healing potential is above the necessary to fully heal
    /// </summary>
    private bool BufferFull()
    {
        float healingPot = 0;
        foreach (HealthOrb o in healingBuffer.ToArray())
        {
            healingPot += o.GetAmt() * difficultyHealingMod;
        }

        return (CurrentHealth(0) + healingPot) >= MaxHealth(0);
    }

    /// <summary>
    /// Keep track of healing buffer the player has
    /// </summary>
    /// <param name="healthType">Type of health to heal</param>
    /// <returns>Whether the player can currently heal</returns>
    public override bool CanHeal(BarType healthType)
    {
        return base.CanHeal(healthType) && !BufferFull();
    }

    #endregion

    #region Difficulty - Health Orb Efficiency

    /// <summary>
    /// Difficulty modifier to apply to healing sources
    /// </summary>
    private float difficultyHealingMod = 1;
    /// <summary>
    /// Key for looking up healing modifier setting
    /// </summary>
    private const string difficultyHealingModKey = "HealthReplenishRate";

    public void UpdateDifficulty(float newModifier)
    {
        difficultyHealingMod = newModifier;
    }

    private void OnEnable()
    {
        GlobalDifficultyManager.instance.Subscribe(this, difficultyHealingModKey);
    }

    private void OnDisable()
    {
        GlobalDifficultyManager.instance.Unsubscribe(this, difficultyHealingModKey);
    }

    /// <summary>
    /// Heal the player, applying difficulty modifiers
    /// </summary>
    /// <param name="hp">HP to recover, before difficulty mod</param>
    /// <param name="healthType">type of health to heal</param>
    /// <returns>Whether any healing was done</returns>
    public override bool Heal(float hp, BarType healthType = BarType.NA)
    {
        return base.Heal(hp * difficultyHealingMod, healthType);
    }

    #endregion
}
