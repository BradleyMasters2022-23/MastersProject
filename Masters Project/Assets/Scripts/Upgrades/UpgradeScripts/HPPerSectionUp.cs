using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPerSectionUp : IUpgrade {
  private PlayerHealth hp;
  public override void LoadUpgrade(PlayerController player) {
    hp = FindObjectOfType<PlayerHealth>();
    hp.GetNumSections().ChangeVal(hp.GetNumSections().Current + 1);
    }
}
