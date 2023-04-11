using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make it possible to create one of these with an existing crystal,
// so players can drop and pick up their crystals
public class CrystalInteract : Interactable
{
    private Crystal crystal;
    private int par;

    private CrystalSlotScreen ui;

    [Tooltip("Amount to add to room count to make par. Increases overall stats of a crystal. MUST be at least 1.")]
    [SerializeField] private int parMod = 1;

    private void Awake()
    {
        ui = FindObjectOfType<CrystalSlotScreen>(true);
    }

    private void Start()
    {
        if(LinearSpawnManager.instance != null)
        {
            par = LinearSpawnManager.instance.GetCombatRoomCount() + parMod;

        } else
        {
            Debug.Log("No linear spawn manager found");
        }

        if (CrystalManager.instance != null)
        {
            crystal = CrystalManager.instance.GenerateCrystal(par);

        } else
        {
            Debug.Log("No crystal manager found");
        }

    }

    public override void OnInteract(PlayerController player)
    {
        if (crystal != null)
        {
            if (GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
            {
                Debug.Log("Not in a state where the player can interact with ths object");
                return;
            }

            ui.OpenScreen(this);
        } else
        {
            Debug.Log("No crystal found");
        }
        
    }

    public Crystal GetCrystal()
    {
        return crystal;
    }
}
