using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSectionUp : IUpgrade
{
    private PlayerHealth hp;
    public int healthIncrease;
    public override void LoadUpgrade(PlayerController player)
    {
        hp = FindObjectOfType<PlayerHealth>();

        int temp = hp.GetHealthPerSection() + healthIncrease;
        hp.SetHealthPerSection(temp);
        PlayerHealthSection[] sections = hp.GetSections();

        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i] != null)
                sections[i].SetMaxHealth(temp);
        }
    }
}