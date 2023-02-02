using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedShotsManager : MonoBehaviour
{
    public static EnhancedShotsManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }
}
