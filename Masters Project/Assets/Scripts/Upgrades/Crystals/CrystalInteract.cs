using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make it possible to create one of these with an existing crystal,
// so players can drop and pick up their crystals
public class CrystalInteract : MonoBehaviour, Interactable
{
    private Crystal crystal;
    private int par;

    [SerializeField] private GameObject health;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject time;
    [SerializeField] private GameObject movement;
    [SerializeField] private GameObject grenade;
    [SerializeField] private GameObject shield;

    private GameObject[] shapes;

    [Tooltip("Par multiplier. Increases overall stats of a crystal. MUST be at least 1.")]
    [SerializeField] private int parMod = 1;

    private CrystalManager crystalManagerInstance;

    private void Awake()
    {
        shapes = new GameObject[6];
        shapes[0] = health;
        shapes[1] = gun;
        shapes[2] = time;
        shapes[3] = movement;
        shapes[4] = grenade;
        shapes[5] = shield;
    }

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
        
        // loads crystal
        if (crystalManagerInstance != null && crystal == null)
        {
            crystal = crystalManagerInstance.GenerateCrystal(par);
        }

        switch (crystal.stats[0].GetGroup())
        {
            case IStat.StatGroup.HEALTH:
                health.GetComponent<Renderer>().material.color = crystal.GetColor();
                health.SetActive(true);
                break;
            case IStat.StatGroup.GUN:
                gun.GetComponent<Renderer>().material.color = crystal.GetColor();
                gun.SetActive(true);
                break;
            case IStat.StatGroup.GRENADE:
                grenade.GetComponent<Renderer>().material.color = crystal.GetColor();
                grenade.SetActive(true);
                break;
            case IStat.StatGroup.MOVEMENT:
                movement.GetComponent<Renderer>().material.color = crystal.GetColor();
                movement.SetActive(true);
                break;
            case IStat.StatGroup.TIMESTOP:
                time.GetComponent<Renderer>().material.color = crystal.GetColor();
                time.SetActive(true);
                break;
            case IStat.StatGroup.SHIELD:
                shield.GetComponent<Renderer>().material.color = crystal.GetColor();
                shield.SetActive(true);
                break;
        }
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
