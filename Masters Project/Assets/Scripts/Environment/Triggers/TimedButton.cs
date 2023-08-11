using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedButton : TimeAffectedEntity, ITriggerable
{
    [Header("Gameplay")]

    [SerializeField] private float activeDuration;

    [SerializeField] private bool activated;

    [SerializeField] private bool locked;

    [SerializeField] private Trigger targetTrigger;

    [Header("Indicators")]

    [Tooltip("Indicators that play when a generator is destroyed")]
    [SerializeField] private IIndicator[] generatorDestroyedIndicator;
    [Tooltip("Indicators that play when a generator is repaired")]
    [SerializeField] private IIndicator[] generatorFixedIndicator;

    [Tooltip("Indicators that play when a generator is locked")]
    [SerializeField] private IIndicator[] lockedIndicator;
    [Tooltip("Indicators that play when a generator is permenately offline")]
    [SerializeField] private IIndicator[] permDeadIndicators;

    [SerializeField] private Target host;

    [SerializeField] private SummonObj summonEffect;

    private void Awake()
    {
        if (targetTrigger == null)
        {
            targetTrigger = GetComponent<Trigger>();
            if (targetTrigger == null)
            {
                Debug.LogError("[TIMEDBUTTON] Tried to activate, but could not find a trigger " +
                    "reference! Remember to attach a trigger reference onto its object!");
                gameObject.SetActive(false);
            }
        }
        
        activated = false;
        //Indicators.SetIndicators(activatedIndicators, false);
        //Indicators.SetIndicators(generatorFixedIndicator, true);
    }

    public void SummonButton()
    {
        gameObject.SetActive(true);
        summonEffect.transform.parent= null;

        summonEffect.gameObject.SetActive(true);
        summonEffect.Play();

        SetLock(true);
    }

    private void OnEnable()
    {
        if(targetTrigger != null)
        {
            targetTrigger.Register(this);
            
            Indicators.SetIndicators(generatorFixedIndicator, true);
        }
    }
    private void OnDisable()
    {
        if(targetTrigger != null)
        {
            targetTrigger.Unregister(this);
        }
    }

    /// <summary>
    /// When triggered, reset the current timer
    /// </summary>
    public void Trigger()
    {
        // Don't do anything while locked
        if (locked) return;

        activated = true;

        DestroyedEffects();
    }

    private void RepairButton()
    {
        activated = false;
        host.enabled = true;
        RepairedIndicators();
    }

    /// <summary>
    /// Disable any deactivated indicators, enable any activated indicators
    /// </summary>
    private void DestroyedEffects()
    {
        Indicators.SetIndicators(generatorFixedIndicator, false);
        Indicators.SetIndicators(generatorDestroyedIndicator, true);
    }
    /// <summary>
    /// Disable any active indicators, enable any deactivated indicators
    /// </summary>
    private void RepairedIndicators()
    {
        // Debug.Log($"{name} repair indicators called");
        Indicators.SetIndicators(generatorDestroyedIndicator, false);
        Indicators.SetIndicators(generatorFixedIndicator, true);
    }

    /// <summary>
    /// Whether or not this button is currently activated
    /// </summary>
    /// <returns>If its currently online and activated</returns>
    public bool Activated()
    {
        return activated || !isActiveAndEnabled;
    }

    /// <summary>
    /// Force the timed button to reset
    /// </summary>
    public void ResetButton()
    {
        if(gameObject.activeSelf)
            StartCoroutine(TryRepair());
        
        if(host == null)
            host = GetComponent<Target>();

        host?.ResetTarget();
    }

    private IEnumerator TryRepair()
    {
        yield return new WaitUntil(() => !this.locked);
        RepairButton();
    }

    public void SetLock(bool locked)
    {
        this.locked = locked;
        host.enabled = !locked;
        activated = false;

        if (locked)
        {
            Indicators.SetIndicators(generatorDestroyedIndicator, false);
            Indicators.SetIndicators(generatorFixedIndicator, false);
        }
        else
        {
            Indicators.SetIndicators(generatorFixedIndicator, true);
        }

        Indicators.SetIndicators(lockedIndicator, locked);
    }

    public void Die()
    {
        Indicators.SetIndicators(generatorFixedIndicator, false);
        Indicators.SetIndicators(generatorDestroyedIndicator, false);
        Indicators.SetIndicators(permDeadIndicators, true);


        this.enabled = false;
        host.enabled = false;
    }
}
