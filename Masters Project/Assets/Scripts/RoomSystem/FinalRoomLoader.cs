using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoomLoader : SegmentLoader
{
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

    
}
