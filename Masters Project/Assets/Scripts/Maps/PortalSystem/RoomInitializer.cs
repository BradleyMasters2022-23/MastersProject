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
    [SerializeField] GameObject[] randomizedObjs;
    [Tooltip("Any other events that should happen on load")]
    [SerializeField] UnityEvent onInitializeEvents;

    [SerializeField] bool automaticallyEnd;

    [SerializeField] private GenericWeightedList<PortalTrigger> exitPortalList;

    [SerializeField] private GenericWeightedList<CorruptedProp> corruptedProps;

    public virtual void Init()
    {
        // Tell randomized objects to initiate randomization
        if(randomizedObjs != null)
        {
            foreach (GameObject obj in randomizedObjs)
            {
                IRandomizer t = obj.GetComponent<IRandomizer>();
                if (t != null)
                    t.Randomize();
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

        onInitializeEvents.Invoke();

        if(automaticallyEnd)
        {
            MapLoader.instance.ActivatePortal();
        }
    }

    /// <summary>
    /// Choose a random secret prop to initialize
    /// </summary>
    public void ChooseRandomSecretProp()
    {
        CorruptedProp prop = corruptedProps.Pull();
        if(prop != null)
        {
            prop.gameObject.SetActive(true);
            prop.Init();
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
