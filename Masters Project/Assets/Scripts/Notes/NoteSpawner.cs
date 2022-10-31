/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Spawns fragment container.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour {
  private FragmentContainer fragmentContainer;
  public GameObject container;
  public Transform fragmentSpawnPoint;

  public void SpawnFragment() {
    GameObject obj = Instantiate(container, fragmentSpawnPoint.transform.position, fragmentSpawnPoint.rotation);
    obj.GetComponent<FragmentContainer>().SetUp(GetSpawnFragment());
    fragmentContainer = obj.GetComponent<FragmentContainer>();

    fragmentContainer.GetComponent<Collider>().enabled = true;
  }

  public Fragment GetSpawnFragment() {
    return AllNotesManager.instance.GetRandomLostNote().GetRandomLostFragment();
  }
}
