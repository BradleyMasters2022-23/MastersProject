using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationIndicator : IIndicator
{
    [SerializeField] private Animator animator;
    [SerializeField] private string variableName;
    [SerializeField] private bool hardStop;

    public override void Activate()
    {
        animator.enabled = true;
        animator.SetTrigger(variableName);
    }

    public override void Deactivate()
    {
        if(hardStop)
        {
            animator.StopPlayback();
            animator.enabled= false;
        }
            

        animator.ResetTrigger(variableName);
    }
}
