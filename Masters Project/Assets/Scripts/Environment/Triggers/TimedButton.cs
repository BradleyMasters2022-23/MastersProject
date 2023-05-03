using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedButton : MonoBehaviour, ITriggerable
{
    [Header("Gameplay")]

    [SerializeField] private float activeDuration;
    [SerializeField] private bool affectedByTimestop = true;

    private ScaledTimer timer;
    private bool activated;

    private bool locked;

    [SerializeField] private Trigger targetTrigger;

    [Header("Indicators")]

    [SerializeField] private IIndicator[] generatorDestroyedIndicator;
    [SerializeField] private IIndicator[] generatorFixedIndicator;

    private Target host;

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

    private void OnEnable()
    {
        if(targetTrigger!= null)
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
        if (timer == null) return;

        // if the activation timer is done and still active, disable activated status
        if(timer.TimerDone() && activated)
        {
            activated = false;
            DeactivatedEffects();
        }
        // if the activation timer is running and not set to active, enable activated status
        else if(!timer.TimerDone() && !activated)
        {
            activated = true;
            ActivatedEffects();
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
    private void ActivatedEffects()
    {
        Indicators.SetIndicators(generatorFixedIndicator, false);

        Indicators.SetIndicators(generatorDestroyedIndicator, true);
    }
    /// <summary>
    /// Disable any active indicators, enable any deactivated indicators
    /// </summary>
    private void DeactivatedEffects()
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
        DeactivatedEffects();

        if(host == null)
            host = GetComponent<Target>();

        host?.ResetTarget();
    }

    public void SetLock(bool locked)
    {
        this.locked = locked;

        if(locked)
        {
            activated = false;
            DeactivatedEffects();
        }
    }
}
