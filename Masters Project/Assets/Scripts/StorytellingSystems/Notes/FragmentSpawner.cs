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

    public void SpawnFragment(Transform parent)
    {
        if(FragShouldSpawn()) {
            GameObject obj = CreateFrag();

            obj.transform.parent = parent;
            if(obj != null ) 
                obj.GetComponent<Collider>().enabled = true;
            // Debug.Log(obj.GetComponent<FragmentInteract>().GetFragment().GetFragmentID());
        } 
    }

    private GameObject CreateFrag() {
        
        if(AllNotesManager.instance == null)
        {
            return null;
        }
        NoteObject temp = AllNotesManager.instance.GetRandomLostNote();
        Fragment tempNote;

        if (temp != null)
            tempNote = temp.GetRandomLostFragment();
        else
            return null;

        // error check
        if (temp == null)
            return null;

        GameObject obj = Instantiate(fragInteractable, fragSpawnPoint.transform.position, fragSpawnPoint.rotation);
        obj.GetComponent<FragmentInteract>().SetUp(tempNote);

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
