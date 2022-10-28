using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSectionUp : IUpgrade {
  private PlayerHealth hp;
  private UpgradableInt healthPerSection;
  public override void LoadUpgrade(PlayerController player) {
    hp = FindObjectOfType<PlayerHealth>();
    hp.HealthPerSectionUp(5);
    foreach(PlayerHealthSection section in hp.GetSections()) {
      section.MaxHealthUp(5);
    }
  }
}
