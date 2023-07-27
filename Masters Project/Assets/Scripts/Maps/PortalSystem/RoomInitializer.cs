/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - May 26, 2023
 * Last Edited - May 26, 2023 by Ben Schuster
 * Description - Initializer for rooms using portal system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomInitializer : MonoBehaviour
{
    [Header("Core Loading")]

    [Tooltip("All objects that should be randomized on load")]
    [SerializeField] GameObject randomizedRoot;

    [SerializeField] bool automaticallyEnd;

    [SerializeField] private GenericWeightedList<PortalTrigger> exitPortalList;

    [SerializeField] private SecretPortalRandomizer corruptedProps;

    /// <summary>
    /// Initialize any objects in the world
    /// </summary>
    public virtual void Init(Cubemap nextCubemap)
    {
        if(randomizedRoot != null)
        {
            // get all props in the randomized root, randomize them
            IRandomizer[] randomizedObjs = randomizedRoot.GetComponentsInChildren<IRandomizer>(true);
            if (randomizedObjs != null)
            {
                foreach (var obj in randomizedObjs)
                {
                    obj.Randomize();
                }
            }
        }
        
        // Get all fragments, tell them to initialize
        FragmentInteract[] allFragments = FindObjectsOfType<FragmentInteract>();
        foreach (FragmentInteract fragment in allFragments)
        {
            // Debug.Log("Initializing obj of " + fragment.name);
            fragment.PrepareNote();
        }

        // Get all crystals, tell them to initialize
        CrystalInteract[] crystals = FindObjectsOfType<CrystalInteract>();
        foreach (CrystalInteract crystal in crystals)
        {
            crystal.RandomizeCrystal();
        }

        if(automaticallyEnd)
        {
            MapLoader.instance.ActivatePortal();
        }

        foreach(var p in exitPortalList.weightedList)
        {
            p.option.LoadNewCubemap(nextCubemap);
        }
    }

    /// <summary>
    /// Choose a random secret prop to initialize
    /// </summary>
    public void ChooseRandomSecretProp()
    {
        if(corruptedProps != null)
        {
            corruptedProps.Randomize();
        }
    }

    /// <summary>
    /// Get one of the exit portals
    /// </summary>
    /// <returns></returns>
    public PortalTrigger GetExitPortal()
    {
        return exitPortalList.Pull();
    }
}
