using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationTimeScalar : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator= GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("time", TimeManager.WorldTimeScale);
    }
}
