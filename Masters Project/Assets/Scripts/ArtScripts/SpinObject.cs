using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [Tooltip("Speed this rotates. Set negative to make spin the other way.")]
    [SerializeField] private float spinSpeed;

    
    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, spinSpeed * TimeManager.WorldTimeScale, 0);
    }
}
