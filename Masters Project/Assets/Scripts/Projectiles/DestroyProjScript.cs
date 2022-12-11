using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyProjScript : MonoBehaviour
{
    private RangeAttack parent;

    private void Start()
    {
        parent = GetComponentInParent<RangeAttack>(true);
    }

    public void DestroyProj()
    {
        parent.ScriptCallHit();
    }
}
