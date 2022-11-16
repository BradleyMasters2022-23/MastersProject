/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Spawns fragment container. Should be placed in each individual scene.
 * Eventually could possibly be merged with rewards manager or room manager.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    /// <summary>
    /// fragment container object
    /// </summary>
    private FragmentContainer fragmentContainer;

    [Tooltip("Should always be the same. It's in /Prefabs/Notes.")]
    public GameObject container;

    [Tooltip("Where should .")]
    public Transform fragmentSpawnPoint;

    /// <summary>
    /// spawns a fragment
    /// </summary>
    public void SpawnFragment()
    {
        GameObject obj = Instantiate(container, fragmentSpawnPoint.transform.position, fragmentSpawnPoint.rotation);
        obj.GetComponent<FragmentContainer>().SetUp(GetSpawnFragment());
        fragmentContainer = obj.GetComponent<FragmentContainer>();

        fragmentContainer.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// gets a random fragment from a random note to spawn
    /// </summary>
    public Fragment GetSpawnFragment()
    {
        return AllNotesManager.instance.GetRandomLostNote().GetRandomLostFragment();
    }
}
