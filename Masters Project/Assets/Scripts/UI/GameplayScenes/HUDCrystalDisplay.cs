using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCrystalDisplay : MonoBehaviour
{
    [SerializeField] private CrystalUIDisplay[] displays;
    private Crystal[] loadedCrystals;
    private CrystalManager manager;


    private void OnEnable()
    {
        loadedCrystals = new Crystal[displays.Length];
        StartCoroutine(UpdateUI());
    }
    private IEnumerator UpdateUI()
    {
        while(manager == null)
        {
            manager = CrystalManager.instance;
            yield return null;
        }

        while(true)
        {
            for(int i = 0; i < displays.Length; i++)
            {
                if (i > manager.MaxSlots())
                    continue;

                if (loadedCrystals[i] != manager.GetEquippedCrystal(i))
                {
                    yield return StartCoroutine(SlotIn(i, manager.GetEquippedCrystal(i)));
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Slot in a new crystal. Coroutine so later we can animate it instead
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newCrystal"></param>
    /// <returns></returns>
    private IEnumerator SlotIn(int index, Crystal newCrystal)
    {
        displays[index].LoadCrystal(null, newCrystal, index);
        loadedCrystals[index] = newCrystal;
        yield return null;
    }

    private IEnumerator SlotOut(int index)
    {
        displays[index].LoadEmpty();
        loadedCrystals[index] = null;
        yield return null;
    }
}
