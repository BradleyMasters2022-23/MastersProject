using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class SpinObject : MonoBehaviour
{
    public enum Axis
    {
        Xaxis,
        Zaxis,
        Yaxis
    }

    [Tooltip("Speed this rotates. Set negative to make spin the other way.")]
    [SerializeField] private float spinSpeed;
    [EnumPaging]
    [SerializeField] private Axis spinAxis = Axis.Yaxis;
    
    
    // Update is called once per frame
    void Update()
    {
        switch(spinAxis)
        {
            case Axis.Xaxis:
                {
                    transform.rotation *= Quaternion.Euler(spinSpeed * TimeManager.WorldTimeScale, 0, 0);
                    break;
                }
            case Axis.Zaxis:
                {
                    transform.rotation *= Quaternion.Euler(0, 0, spinSpeed * TimeManager.WorldTimeScale);
                    break;
                }
            case Axis.Yaxis:
                {
                    transform.rotation *= Quaternion.Euler(0, spinSpeed * TimeManager.WorldTimeScale, 0);
                    break;
                }
        }
        
    }
}
