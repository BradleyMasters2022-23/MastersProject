using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSpawner : MonoBehaviour
{
    [Tooltip("% chance for a note to spawn.")]
    [SerializeField] private float luck;
    private Transform fragSpawnPoint;
    [SerializeField] private GameObject fragInteractable;
    private float max;

    public void SpawnFragment()
    {
        if(FragShouldSpawn()) {
            GameObject obj = CreateFrag();
            obj.GetComponent<Collider>().enabled = true;
            Debug.Log(obj.GetComponent<FragmentInteract>().GetFragment().GetFragmentID());
        } 
    }

    private GameObject CreateFrag() {
        Fragment temp = AllNotesManager.instance.GetRandomLostNote().GetRandomLostFragment();
        GameObject obj = Instantiate(fragInteractable, fragSpawnPoint.transform.position, fragSpawnPoint.rotation);
        obj.GetComponent<FragmentInteract>().SetUp(temp);

        return obj;
    }

    private bool FragShouldSpawn()
    {
        if(Random.Range(0f, 100f) <= luck && AllNotesManager.instance.NoteFindable()) {
            return true;
        }

        return false;

    }

    public void SetSpawnPoint(Transform spawn) {
        fragSpawnPoint = spawn;
    }
}
