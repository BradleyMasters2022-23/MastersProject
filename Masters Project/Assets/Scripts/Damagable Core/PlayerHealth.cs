/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 21th, 2022
 * Last Edited - October 21th, 2022 by Ben Schuster
 * Description - Controller for player health
 * ================================================================================================
 */
using UnityEngine;

public class PlayerHealth : Damagable
{
    [Header("=====Player Health=====")]

    [Tooltip("Amount of health each section has")]
    [SerializeField] private UpgradableInt healthPerSection;

    [Tooltip("Amount of sections the player has")]
    [SerializeField] private UpgradableInt numOfSections;

    [Tooltip("Time it takes to passively regenerate a segment")]
    [SerializeField] private UpgradableFloat passiveRegenTime;

    [Tooltip("Time player is out of combat before starting passive health regen")]
    [SerializeField] private UpgradableFloat passiveRegenDelay;

    [Tooltip("Time it takes to actively regenerate a segment [Such as healthkit]")]
    [SerializeField] private UpgradableFloat activeRegenTime;

    /// <summary>
    /// All health sections for the player
    /// </summary>
    private PlayerHealthSection[] healthSections;

    /// <summary>
    /// Index for the current player's health
    /// </summary>
    private int healthSectionIndex;

    /// <summary>
    /// Initialize values and healthbars
    /// </summary>
    protected void Awake()
    {
        healthPerSection.Initialize();
        numOfSections.Initialize();
        passiveRegenTime.Initialize();
        passiveRegenDelay.Initialize();
        activeRegenTime.Initialize();

        // Create all segments necessary, initialize them
        healthSections = new PlayerHealthSection[numOfSections.Current];
        for (int i = 0; i < numOfSections.Current; i++)
        {
            healthSections[i] = gameObject.AddComponent<PlayerHealthSection>();
            healthSections[i].InitializeSection(this, healthPerSection.Current,
                passiveRegenTime.Current, passiveRegenDelay.Current, activeRegenTime.Current);
        }
        healthSectionIndex = healthSections.Length-1;

    }

    /// <summary>
    /// Deal damage to current segment.
    /// </summary>
    /// <param name="_dmg">Damage being dealt</param>
    public override void Damage(int _dmg)
    {
        // Cancel if player already killed
        if(killed)
        {
            return;
        }

        bool segmentKilled = healthSections[healthSectionIndex].TakeDamage(_dmg);

        // If segment is killed, move onto next segment. Check if player died.
        if(segmentKilled)
        {
            // If last index, then player has died
            if(healthSectionIndex == 0 && !killed)
            {
                Die();
            }
            else
            {
                healthSectionIndex--;
            }
        }
    }

    /// <summary>
    /// Trigger a game over for the player
    /// </summary>
    protected override void Die()
    {
        killed = true;
        Debug.Log("[PlayerHealth] Player has died! Game over!");

        // TODO - Tie in with other systems for handling game over!
    }

    /// <summary>
    /// Heal the player's current segment and expanding segments
    /// </summary>
    /// <param name="_segmentsToHeal">Number of segments to heal</param>
    /// <returns>Whether or not any healing could happen</returns>
    public bool HealPlayer(int _segmentsToHeal)
    {
        // Cancel if player already killed
        if (killed)
        {
            return false;
        }

        // Check if player is at max health before being healed
        if (healthSectionIndex == healthSections.Length - 1 && healthSections[healthSectionIndex].IsMaxed())
        {
            return false;
        }

        // Check if current segment is at full health. If so, move onto next before healing
        if (healthSections[healthSectionIndex].IsMaxed())
        {
            healthSectionIndex++;
        }

        // Continue to heal sections until all requested segments are healed
        int sectionsHealed = 0;
        while (sectionsHealed < _segmentsToHeal)
        {
            healthSections[healthSectionIndex].ForceHeal();

            // If hit maxed sections, then stop healing and indexing
            if (healthSectionIndex + 1 >= healthSections.Length)
            {
                break;
            }
            else
            {
                healthSectionIndex++;
                sectionsHealed++;
            }
        }

        // Return true since healing has occured
        return true;
    }

    private void Update()
    {
        // If index is pointing to an empty bar, move down to the next active one
        if (!killed && healthSections[healthSectionIndex].CurrentState == PlayerHealthSection.HealthSectionState.EMPTIED)
        {
            healthSectionIndex--;
        }
    }

    public UpgradableInt GetHealthPerSection() {
      return healthPerSection;
    }

    public UpgradableInt GetNumSections() {
      return numOfSections;
    }
}
