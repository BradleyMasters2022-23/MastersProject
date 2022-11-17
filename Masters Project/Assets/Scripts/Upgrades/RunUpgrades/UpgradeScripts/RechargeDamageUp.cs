using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeDamageUp : IUpgrade
{
    private TimeManager time;
    private PlayerGunController gun;
    private int originalDamage;
    public int damageMultiplier;
    private bool damageUp;
    public override void LoadUpgrade(PlayerController player)
    {
        damageUp = false;
        time = FindObjectOfType<TimeManager>();
        gun = FindObjectOfType<PlayerGunController>();
        originalDamage = gun.GetDamageMultiplier();
    }

    private void Update()
    {
      if(time.GetState() == TimeManager.TimeGaugeState.EMPTIED) {
          damageUp = true;
      }

      if(damageUp) {
          gun.SetDamageMultiplier(originalDamage*damageMultiplier);
          Debug.Log("Damage up!");
          if(time.GetState() != TimeManager.TimeGaugeState.EMPTIED) {
              damageUp = false;
              gun.SetDamageMultiplier(originalDamage);
              Debug.Log("Damage back to normal");
          }
      }
    }
}
