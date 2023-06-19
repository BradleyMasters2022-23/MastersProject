using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCaller : MonoBehaviour
{
    private CheckpointManager cp;

    /// <summary>
    /// Make sure the an updated reference is had
    /// </summary>
    private void OnEnable()
    {
        if(cp == null)
            cp = CheckpointManager.Instance;
    }

    /// <summary>
    /// Call checkpoint manager to teleport player
    /// </summary>
    public void CallCheckpointTeleport()
    {
        cp.SendPlayerToCheckpoint();
    }
}
