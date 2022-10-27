using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPerSectionUp : IUpgrade {
  private PlayerHealth hp;
  private int maxHP;
  public override void LoadUpgrade(PlayerController player) {
    hp = FindObjectOfType<PlayerHealth>();
    maxHP = hp.GetHealthPerSection().Current * hp.GetNumSections().Current;

    hp.GetNumSections().ChangeVal(hp.GetNumSections().Current + 1);
    hp.GetHealthPerSection().ChangeVal((maxHP/hp.GetNumSections().Current));
    }
}
