using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBoolRandomizer : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void OnEnable()
    {
        if(animator != null)
        {
            int r = Random.Range(0, 2);
            bool choice = (r == 0);
            animator.SetBool("Rand", choice);
        }
    }
}
