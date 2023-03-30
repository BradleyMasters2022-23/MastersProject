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
        Crystal newCrystal = new Crystal(p);

        for (int i = 0; i < 3; i++)
        {
            if (newCrystal.cost != newCrystal.par)
            {
                newCrystal.AddStat(stats[Random.Range(0, stats.Count)]);
            }
        }

        return newCrystal;
    }

    public void LoadCrystal(Crystal crystal)
    {
        crystal.EquipCrystal(player);
    }
}
