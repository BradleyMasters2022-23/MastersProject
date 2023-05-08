using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class SpinObject : TimeAffectedEntity
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
        float amt = spinSpeed * Timescale * Time.timeScale;

        switch(spinAxis)
        {
            case Axis.Xaxis:
                {
                    transform.rotation *= Quaternion.Euler(amt, 0, 0);
                    break;
                }
            case Axis.Zaxis:
                {
                    transform.rotation *= Quaternion.Euler(0, 0, amt);
                    break;
                }
            case Axis.Yaxis:
                {
                    transform.rotation *= Quaternion.Euler(0, amt, 0);
                    break;
                }
        }
        
    }
}
