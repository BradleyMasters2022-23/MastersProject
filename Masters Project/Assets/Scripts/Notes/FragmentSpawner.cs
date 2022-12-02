using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSpawner : MonoBehaviour
{
    [Tooltip("% chance for a note to spawn.")]
    [SerializeField] private float luck;
    [Tooltip("Where the note should spawn.")]
    [SerializeField] private Transform noteSpawnPoint;
    [SerializeField] private GameObject fragInteractable;
    private float max;
    // Start is called before the first frame update
    private void Start()
    {
        max = 100/luck-1;
    }

    public void SpawnFragment()
    {
        if(FragShouldSpawn()) {
            Vector3 temp = noteSpawnPoint.transform.position;
            GameObject obj = Instantiate(fragInteractable, temp, noteSpawnPoint.rotation);
            obj.GetComponent<FragmentInteract>().SetUp(AllNotesManager.instance.GetRandomLostNote().GetRandomLostFragment());
            obj.GetComponent<Collider>().enabled = true;
        }
    }

    private bool FragShouldSpawn()
    {
        if(Random.Range(0, max) == 0) {
            return true;
        }

        return false;


    }
}
