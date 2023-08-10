using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public struct UpgradeColorOption
{
    public IStat.StatGroup group;
    public GameObject objectRepresentation;
}

// TODO: make it possible to create one of these with an existing crystal,
// so players can drop and pick up their crystals
public class CrystalInteract : MonoBehaviour, Interactable
{
    private Crystal crystal;
    private int par;

    [SerializeField] private UpgradeColorOption[] key;

    [SerializeField] private GameObject[] allOptions;

    [Tooltip("Par multiplier. Increases overall stats of a crystal. MUST be at least 1.")]
    [SerializeField] private int parMod = 1;

    private CrystalManager crystalManagerInstance;

    public void RandomizeCrystal()
    {
        RandomizeCrystalSetStats(MapLoader.instance.PortalDepth());
    }

    public void RandomizeCrystalSetStats(int newPar)
    {
        if (crystalManagerInstance == null)
            crystalManagerInstance = CrystalManager.instance;

        // apply new level, clamp it to make sure its above 1
        par = Mathf.Clamp(newPar * parMod, 1, 9999);
        //par++;

        // loads crystal
        if (crystalManagerInstance != null && crystal == null)
        {
            crystal = crystalManagerInstance.GenerateCrystal(par);
        }

        // loop through key, activate relevant group and disable the rest
        IStat.StatGroup targetGroup = crystal.stats[0].GetGroup();
        for (int i = 0; i < key.Length; i++)
        {
            key[i].objectRepresentation.SetActive(targetGroup == key[i].group);
        }

        //switch (crystal.stats[0].GetGroup())
        //{
        //    case IStat.StatGroup.HEALTH:
        //        health.GetComponent<Renderer>().material.color = crystal.GetColor();
        //        health.SetActive(true);
        //        break;
        //    case IStat.StatGroup.GUN:
        //        gun.GetComponent<Renderer>().material.color = crystal.GetColor();
        //        gun.SetActive(true);
        //        break;
        //    case IStat.StatGroup.GRENADE:
        //        grenade.GetComponent<Renderer>().material.color = crystal.GetColor();
        //        grenade.SetActive(true);
        //        break;
        //    case IStat.StatGroup.MOVEMENT:
        //        movement.GetComponent<Renderer>().material.color = crystal.GetColor();
        //        movement.SetActive(true);
        //        break;
        //    case IStat.StatGroup.TIMESTOP:
        //        time.GetComponent<Renderer>().material.color = crystal.GetColor();
        //        time.SetActive(true);
        //        break;
        //    case IStat.StatGroup.SHIELD:
        //        shield.GetComponent<Renderer>().material.color = crystal.GetColor();
        //        shield.SetActive(true);
        //        break;
        //}
    }

    public void OnInteract()
    {
        if (GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
        {
            // Debug.Log("Not in a state where the player can interact with ths object");
            return;
        }

        if (crystal == null)
        {
            RandomizeCrystal();
        }

        crystalManagerInstance.GetCurrentScreen().OpenScreen(this);
    }

    public bool CanInteract()
    {
        return true;
    }

    public Crystal GetCrystal()
    {
        return crystal;
    }

    public void BegoneCrystalInteract()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
