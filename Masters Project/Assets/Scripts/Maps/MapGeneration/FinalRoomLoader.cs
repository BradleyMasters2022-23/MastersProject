using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoomLoader : SegmentLoader
{
    [SerializeField] List<GameObject> stuffToDisable;

    protected override IEnumerator UniquePoolInitialization()
    {
        // nothing to do yet

        initialized= true;

        yield return null;

    }

    protected override void UniqueActivate()
    {
        // this is prob where we initialize the boss

        // Tell the door to teleport to the return hub
        doorManager.SetReturnToHub();
    }

    protected override void UniqueDeactivate()
    {
        
    }

    public override void StartSegment()
    {
        Debug.Log("Starting segment");
        //MapLoader.instance.EndRoomEncounter();
        foreach(var obj in stuffToDisable)
        {
            Debug.Log($"Disabling {obj.name}");
            obj.SetActive(false);
        }

        return;
    }

    public void UnlockDoor()
    {
        MapLoader.instance.EndRoomEncounter();
    }
}
