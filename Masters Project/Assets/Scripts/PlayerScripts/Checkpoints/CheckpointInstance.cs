using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointInstance : MonoBehaviour
{
    public void SubmitCheckpoint()
    {
        CheckpointManager.Instance.SetNewCheckpoint(transform);
    }
}
