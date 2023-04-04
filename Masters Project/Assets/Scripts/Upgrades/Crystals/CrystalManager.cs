/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - March 16 2023
 * Last Edited - March 30 2023
 * Description - holds and generates all crystals
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{

    [Tooltip("Number of crystals we want the player to be able to have equipped at once.")]
    [SerializeField] private int crystalSlots;

    [Tooltip("Number of crystals we want the player to be able to hold in inventory at once.")]
    [SerializeField] private int inventorySize;

    public static CrystalManager instance;

    /// <summary>
    /// All crystals the player currently holds. 
    /// </summary>
    private List<Crystal> crystalInventory = new List<Crystal>();

    /// <summary>
    /// All crystals the player currently has equipped. 
    /// </summary>
    private List<Crystal> equippedCrystals = new List<Crystal>();

    [Tooltip("List of possible stats. Should contain stat prefabs.")]
    [SerializeField] private List<IStat> stats = new List<IStat>();

    private PlayerController player;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
}
        else
        {
            Destroy(this.gameObject);
        }

        player = FindObjectOfType<PlayerController>();
    }

    /// <summary>
    /// Generates a crystal.
    /// </summary>
    /// <param name="p">Designates par of a crystal.</param>
    /// <returns>Generated crystal.</returns>
    public Crystal GenerateCrystal(int p)
    {
        Crystal newCrystal = new Crystal();
        newCrystal.par = p;

        // need to gate this from picking the same stat multiple times
        for (int i = 0; i < 3; i++)
        {
            if (newCrystal.cost != newCrystal.par)
            {
                newCrystal.AddStat(stats[Random.Range(0, stats.Count-1)]);
            }
        }

        return newCrystal;
    }

    /// <summary>
    /// Loads a crystal to the player
    /// </summary>
    /// <param name="crystal">Crystal to be loaded. Stored in the CrystalInteract object</param>
    public void LoadCrystal(Crystal crystal)
    {
        // note: currently this adds crystal to equipped and to inventory and equips it. eventually
        if(equippedCrystals.Count < crystalSlots)
        {
            crystal.EquipCrystal(player);
            equippedCrystals.Add(crystal);
            equippedCrystals.Add(crystal);
        }
        
    }

    /// <summary>
    /// unloads a crystal
    /// </summary>
    /// <param name="crystal">crystal to be unloaded</param>
    /// <param name="store"> whether the crystal should be stored in inv or dropped</param>
    public void UnloadCrystal(Crystal crystal, bool store)
    {
        if(crystalInventory.Contains(crystal))
        {
            crystal.DequipCrystal(player);
            equippedCrystals.Remove(crystal);
        }

        if(store)
        {
            PickUpCrystal(crystal);
        } else
        {
            DropCrystal(crystal);
        }
    }

    /// <summary>
    /// pick up a crystal and add it to inventory, but don't equip it
    /// </summary>
    /// <param name="crystal"></param>
    public void PickUpCrystal(Crystal crystal)
    {
        if(crystalInventory.Count < inventorySize)
        {
            crystalInventory.Add(crystal);
        }
    }

    /// <summary>
    /// removes crystal from inventory
    /// </summary>
    /// <param name="crystal"></param>
    public void DropCrystal(Crystal crystal)
    {
        
        if(equippedCrystals.Contains(crystal))
        {
            equippedCrystals.Remove(crystal);
        }

        crystalInventory.Remove(crystal);
        // TODO: make this actually drop a physical crystalinteract object with the crystal attached
    }
}
