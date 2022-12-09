/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 21th, 2022
 * Last Edited - October 21th, 2022 by Ben Schuster
 * Description - Controller for player health
 * ================================================================================================
 */
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

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

    [Tooltip("Sound when player takes damage")]
    [SerializeField] private AudioClip playerDamage;
    private AudioSource source;

    /// <summary>
    /// All health sections for the player
    /// </summary>
    private PlayerHealthSection[] healthSections;

    /// <summary>
    /// Index for the current player's health
    /// </summary>
    private int healthSectionIndex;

    /// <summary>
    /// Get current health
    /// </summary>
    private int currHealth;
    /// <summary>
    /// Get current health
    /// </summary>
    public int CurrHealth
    {
        get { return currHealth;  }
    }

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

        healthSections = new PlayerHealthSection[numOfSections.Current];

        source = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Load in health from game manager
    /// </summary>
    private void Start()
    {
        
        // If last health not saved, load fresh health
        if (GameManager.instance.lastPlayerHealth <= 0 || GameManager.instance.CurrentState == GameManager.States.HUB)
        {
            // Create all segments necessary, initialize them
            for (int i = 0; i < numOfSections.Current; i++)
            {
                healthSections[i] = gameObject.AddComponent<PlayerHealthSection>();
                healthSections[i].InitializeSection(this, healthPerSection.Current, healthPerSection.Current,
                    passiveRegenTime.Current, passiveRegenDelay.Current, activeRegenTime.Current);

                currHealth += healthPerSection.Current;
            }
            healthSectionIndex = healthSections.Length - 1;
        }
        // If last health was saved, load that instead
        else
        {
            currHealth = GameManager.instance.lastPlayerHealth;
            int amountLoaded = 0;

            // Create all segments necessary
            for (int i = 0; i < numOfSections.Current; i++)
            {
                healthSections[i] = gameObject.AddComponent<PlayerHealthSection>();

                // If loading in will exceed amount, then load in the differnece
                if (amountLoaded + healthPerSection.Current >= currHealth)
                {
                    //Debug.Log("Loading partial section");
                    healthSections[i].InitializeSection(this, currHealth - amountLoaded, healthPerSection.Current,
                    passiveRegenTime.Current, passiveRegenDelay.Current, activeRegenTime.Current);

                    amountLoaded = currHealth;
                    healthSectionIndex = i;
                }
                // If already maxed, then load in at 0
                else if (amountLoaded == currHealth)
                {
                    //Debug.Log("Loading empty section");
                    healthSections[i].InitializeSection(this, 0, healthPerSection.Current, 
                    passiveRegenTime.Current, passiveRegenDelay.Current, activeRegenTime.Current);
                }
                // Otherwise, load in to full
                else
                {
                    //Debug.Log("Loading full section");
                    healthSections[i].InitializeSection(this, healthPerSection.Current, healthPerSection.Current,
                    passiveRegenTime.Current, passiveRegenDelay.Current, activeRegenTime.Current);

                    amountLoaded += healthPerSection.Current;
                }
            }
        }
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

        // Update total health
        UpdateHealth();

        source.PlayOneShot(playerDamage, 0.5f);
    }

    /// <summary>
    /// Trigger a game over for the player
    /// </summary>
    protected override void Die()
    {
        if (killed)
            return;


        killed = true;
        Debug.Log("[PlayerHealth] Player has died! Game over!");

        GameManager.instance.ChangeState(GameManager.States.GAMEOVER);
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
        UpdateHealth();
    }

    /// <summary>
    /// Get players max health
    /// </summary>
    /// <returns>max health this can have</returns>
    public int MaxHealth()
    {
        return healthPerSection.Current * numOfSections.Current;
    }

    private void UpdateHealth()
    {
        // Update total health
        float hpCount = 0;
        for (int i = 0; i <= healthSectionIndex; i++)
        {
            hpCount += healthSections[i].CurrHealth;
        }
        
        currHealth = Mathf.CeilToInt(hpCount);
    }

    public PlayerHealthSection[] GetSections() {
      return healthSections;
    }

    public void HealthPerSectionUp(int increment) {
      healthPerSection.Increment(increment);
    }

    private void OnDisable()
    {
        GameManager.instance.lastPlayerHealth = currHealth;
    }
}
