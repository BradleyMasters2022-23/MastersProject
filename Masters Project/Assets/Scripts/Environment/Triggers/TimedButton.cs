using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedButton : MonoBehaviour, ITriggerable
{
    [Header("Gameplay")]

    [SerializeField] private float activeDuration;
    [SerializeField] private bool affectedByTimestop = true;

    private ScaledTimer timer;
    [SerializeField] private bool activated;

    [SerializeField] private bool locked;

    [SerializeField] private Trigger targetTrigger;

    [Header("Indicators")]

    [SerializeField] private IIndicator[] generatorDestroyedIndicator;
    [SerializeField] private IIndicator[] generatorFixedIndicator;

    [SerializeField] private IIndicator[] permDeadIndicators;

    private Target host;

    [SerializeField] private SummonObj summonEffect;

    private void Awake()
    {
        if(targetTrigger == null)
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

        host = GetComponent<Target>();
    }

    public void SummonButton()
    {
        summonEffect.transform.parent= null;
        summonEffect.gameObject.SetActive(true);
        summonEffect.Play();
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

    private void Update()
    {
        if (timer == null || locked) return;

        // if the activation timer is done and still active, disable activated status
        if(timer.TimerDone() && activated)
        {
            activated = false;
            host.enabled = true;
            RepairedIndicators();
        }
        // if the activation timer is running and not set to active, enable activated status
        else if(!timer.TimerDone() && !activated)
        {
            activated = true;
            host.enabled = false;
            DestroyedEffects();
            
            //host.ResetTarget();
        }
    }

    /// <summary>
    /// When triggered, reset the current timer
    /// </summary>
    public void Trigger()
    {
        // Don't do anything while locked
        if (locked) return;

        if(timer == null)
            timer = new ScaledTimer(activeDuration, affectedByTimestop);
        else
            timer.ResetTimer();
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
        timer = null;
        activated = false;
        RepairedIndicators();

        if(host == null)
            host = GetComponent<Target>();

        host.enabled = true;
        host?.ResetTarget();
    }

    public void SetLock(bool locked)
    {
        this.locked = locked;
        host.enabled = !locked;

        if(locked)
        {
            activated = false;
            DestroyedEffects();
        }
        else
        {
            RepairedIndicators();
        }
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
