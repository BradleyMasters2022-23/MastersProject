using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPToSpeedTime : IUpgrade {
    [SerializeField] private int healthPerSection;
    [SerializeField] private float timeRegenMultiplier;
    [SerializeField] private float regenDelayMultiplier;
    //[SerializeField] private float timeGaugeMultiplier;
    [SerializeField] private float moveSpeedMultiplier;
    
    //private PlayerHealth hp;
    private TimeManager time;

    // Commented stuff out while removing old health system

    public override void LoadUpgrade(PlayerController player) {
        //hp = player.GetComponent<PlayerHealth>();
        //time = FindObjectOfType<TimeManager>();
        //hp.SetHealthPerSection(healthPerSection);
        //foreach (PlayerHealthSection section in hp.GetSections())
        //{
        //    section.SetMaxHealth(healthPerSection);
        //}

        //hp.ResetSectionIndex();
        //player.SetMoveSpeed(moveSpeedMultiplier);
        //time.SetRegenTime(timeRegenMultiplier);
        //time.SetGaugeMax(timeGaugeMultiplier);
        //time.SetRegenDelay(regenDelayMultiplier);
        //time.ChipRefillGauge();
        

    }

    private void Update()
    {
        //foreach(PlayerHealthSection section in hp.GetSections())
        //{
        //    if(section.GetState() == PlayerHealthSection.HealthSectionState.EMPTIED)
        //    {
        //        section.ChipChangeState(PlayerHealthSection.HealthSectionState.IDLE);
        //        hp.ResetSectionIndex();
        //    }
        //}
    }
}
