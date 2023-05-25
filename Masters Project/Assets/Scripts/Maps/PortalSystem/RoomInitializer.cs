using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomInitializer : MonoBehaviour
{
    [Tooltip("All objects that should be randomized on load")]
    [SerializeField] GameObject[] randomizedObjs;

    [SerializeField] UnityEvent onInitializeEvents;

    private void Start()
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
            fragment.PrepareNote();
        }

        // Get all crystals, tell them to initialize
        CrystalInteract[] crystals = FindObjectsOfType<CrystalInteract>();
        foreach (CrystalInteract crystal in crystals)
        {
            crystal.RandomizeCrystal();
        }

        onInitializeEvents.Invoke();
    }
}
