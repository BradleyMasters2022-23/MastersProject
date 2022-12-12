using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyGaugeDamageUp : IUpgrade
{
    private TimeManager time;
    private PlayerGunController gun;
    private float originalDamage;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private int duration;
    private ScaledTimer timer;
    private bool damageUp;
    public override void LoadUpgrade(PlayerController player)
    {
        damageUp = false;
        time = FindObjectOfType<TimeManager>();
        gun = FindObjectOfType<PlayerGunController>();
        originalDamage = gun.GetDamageMultiplier();
        timer = new ScaledTimer(duration, false);
    }

    private void Update()
    {
      if(time.GetState() == TimeManager.TimeGaugeState.EMPTIED && !damageUp) {
           damageUp = true;
           Debug.Log("Damage up!");
           timer.ResetTimer();
      }

      if(damageUp) {
          
          gun.SetDamageMultiplier(originalDamage*damageMultiplier);
          if(timer.TimerDone()) {
              damageUp = false;
              gun.SetDamageMultiplier(originalDamage);
              Debug.Log("Damage back to normal.");
          }
      }
    }
}
