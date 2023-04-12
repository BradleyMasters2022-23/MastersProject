/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - March 16 2023
 * Last Edited - March 30 2023
 * Description - holds and generates all crystals
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrystalManager : MonoBehaviour
{

    [Tooltip("Number of crystals we want the player to be able to have equipped at once.")]
    [SerializeField] private int crystalSlots;

    public static CrystalManager instance;

    /// <summary>
    /// All crystals the player currently has equipped. 
    /// </summary>
    private Crystal[] equippedCrystals;

    [Tooltip("List of possible stats. Should contain stat prefabs.")]
    [SerializeField] private IStat[] stats;

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

        equippedCrystals = new Crystal[crystalSlots];
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
                IStat stat = stats[Random.Range(0, stats.Length-1)];
                newCrystal.AddStat(stat);
            }
        }

        newCrystal.crystalName += "Crystal";
        return newCrystal;
    }

    /// <summary>
    /// Loads a crystal to the player
    /// </summary>
    /// <param name="crystal">Crystal to be loaded. Stored in the CrystalInteract object</param>
    public void LoadCrystal(Crystal crystal, int index)
    {
        if (equippedCrystals[index] != null)
        {
            equippedCrystals[index].DequipCrystal(player);
        }

        equippedCrystals[index] = crystal;
        crystal.EquipCrystal(player);

    }

    /// <summary>
    /// unloads a crystal
    /// </summary>
    /// <param name="crystal">crystal to be unloaded</param>
    /// <param name="store"> whether the crystal should be stored in inv or dropped</param>
    public void UnloadCrystal(Crystal crystal)
    {
        crystal.DequipCrystal(player);

    }

    public Crystal GetEquippedCrystal(int index)
    {

        return equippedCrystals[index];
    }

    public bool CrystalEquipped(int index)
    {
        if (equippedCrystals[index] != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Hub")
        {
            foreach(Crystal crystal in equippedCrystals)
            {
                UnloadCrystal(crystal);
            }
            DestroyCM();
            return;
        }

    }

    public void DestroyCM()
    {
        equippedCrystals = null;
        CrystalManager.instance = null;
        Destroy(gameObject);
    }

}
