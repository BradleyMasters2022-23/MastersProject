/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 18th, 2023
 * Last Edited - July 18th, 2023 by Ben Schuster
 * Description - Manager in charge of reading/writing to the collectable save data
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CollectableSaveManager : MonoBehaviour
{
    /// <summary>
    /// Collectable save manager instance. Middleman for save data reading and writing
    /// </summary>
    public static CollectableSaveManager instance;
    /// <summary>
    /// The current save data being used. Should only be read outside this class
    /// </summary>
    private CollectableSaveData data;
    /// <summary>
    /// string used for the file save data
    /// </summary>
    private const string saveFileName = "collectableSaveData";

    [Tooltip("All potential collectables that can be found by randomized collectable spawners")]
    [SerializeField] List<CollectableSO> randomDropPool;
    [Tooltip("When a spawn chance fails for a collectable, how much luck increases by")]
    [SerializeField] float bonusLuckOnFail;

    private InputAction clearDataCheat;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        // Get loading data
        data = DataManager.instance.Load<CollectableSaveData>(saveFileName);
        if (data == null)
        {
            data = new CollectableSaveData();
        }

        clearDataCheat = InputManager.Controls.PlayerGameplay.clearcollectiondata;
        clearDataCheat.performed += ClearDataCheat;
        clearDataCheat.Enable();
    }

    /// <summary>
    /// Get list of saved fragments from save data
    /// </summary>
    /// <param name="collectable">Collectable to check</param>
    /// <returns>List of ints representing collected fragment indexes. Null if none</returns>
    public List<int> GetSavedFragments(CollectableSO collectable)
    {
        return data.GetSavedFragments(collectable);
    }

    /// <summary>
    /// Save a new fragment to the save data 
    /// </summary>
    /// <param name="collectable">The collectable ID the fragment belongs to</param>
    /// <param name="fragmentID">The fragment index to save</param>
    public void SaveNewFragment(CollectableSO collectable, int fragmentID)
    {
        data.SaveNewFragment(collectable, fragmentID);
        SaveData();
    }

    /// <summary>
    /// Get a random collectable that has not been completed. Null if none
    /// </summary>
    /// <returns>Randomly selected collectable. Null if none available.</returns>
    public CollectableSO GetRandomCollectable()
    {
        // get copy of random pool, then filter out completed fragments
        List<CollectableSO> spawnPool = new List<CollectableSO>(randomDropPool);
        foreach(var opt in spawnPool.ToArray())
        {
            if(data.GetNumberOfFragmentsFound(opt) >= opt.GetFragmentCount())
            {
                spawnPool.Remove(opt);
            }
        }

        // make sure some options are remaining. Otherwise return null
        if(spawnPool.Count> 0)
        {
            int rng = Random.Range(0, spawnPool.Count);
            return spawnPool[rng];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get whether a fragment has been obtained
    /// </summary>
    /// <param name="collectable">ID of the collectable the fragment belongs to</param>
    /// <param name="fragmentID">fragment ID (index) to check</param>
    /// <returns>Whether that collectable's fragment is found</returns>
    public bool FragmentObtained(CollectableSO collectable, int fragmentID)
    {
        return data.FragmentObtained(collectable, fragmentID);
    }

    /// <summary>
    /// reset save data for collectable save data
    /// </summary>
    public void ClearSaveData()
    {
        data = new CollectableSaveData();
        SaveData();
    }

    /// <summary>
    /// Actually write the new data to save
    /// </summary>
    private void SaveData()
    {
        DataManager.instance.Save(saveFileName, data);
    }

    /// <summary>
    /// Validate a whitelist by removing options that are completed
    /// </summary>
    /// <param name="whitelist">Whitelist to validate</param>
    /// <returns>A valid whitelist with copies removed</returns>
    public List<CollectableSO> ValidateWhitelist(List<CollectableSO> whitelist)
    {
        foreach (var opt in whitelist.ToArray())
        {
            if (opt.GetFragmentCount()
                == data.GetNumberOfFragmentsFound(opt))
            {
                whitelist.Remove(opt);
            }
        }
        return whitelist;
    }

    /// <summary>
    /// Whether the given chance passes the RNG
    /// </summary>
    /// <param name="baseChance">base chance to succeed</param>
    /// <returns>Whether the spawn chance was successful</returns>
    public bool ShouldSpawn(float baseChance)
    {
        baseChance += data.savedBonusLuck;
        bool success = Random.Range(0.001f, 100f) <= baseChance;

        // reset luck on success, increment on fail.
        // TODO - move reset luck to pickup instead to not punish missing them
        if (success)
            data.ResetLuck();
        else
            data.AddLuck(bonusLuckOnFail);
        SaveData();

        return success;
    }

    public void ClearDataCheat(InputAction.CallbackContext c)
    {
        ClearSaveData();
    }
}
