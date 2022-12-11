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

    private void Start()
    {
        max = 100/luck-1;
    }

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
        
        if(temp.spawned)
        {
            int i = 0;
            do
            {
                if(i > 100)
                {
                    Debug.Log("Couldn't find an unspawned fragment.");
                    break;
                }

                temp = AllNotesManager.instance.GetRandomLostNote().GetRandomLostFragment();
                i++;

            } while (temp.spawned);
        }
        GameObject obj = Instantiate(fragInteractable, fragSpawnPoint.transform.position, fragSpawnPoint.rotation);
        obj.GetComponent<FragmentInteract>().SetUp(temp);
        temp.spawned = true;

        return obj;
    }

    private bool FragShouldSpawn()
    {
        if(Random.Range(0, max) == 0 && AllNotesManager.instance.NoteFindable()) {
            return true;
        }

        return false;

    }

    public void SetSpawnPoint(Transform spawn) {
        fragSpawnPoint = spawn;
    }
}
