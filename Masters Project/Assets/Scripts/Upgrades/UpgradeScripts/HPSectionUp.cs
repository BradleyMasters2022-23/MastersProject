using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSectionUp : IUpgrade {
  private PlayerHealth hp;
  private UpgradableInt healthPerSection;
  public override void LoadUpgrade(PlayerController player) {
    hp = FindObjectOfType<PlayerHealth>();
    hp.HealthPerSectionUp(5);
        PlayerHealthSection[] sections = hp.GetSections();

        for(int i = 0; i < sections.Length; i++)
        {
            if (sections[i] != null)
                sections[i].MaxHealthUp(5);
        }
  }
}
