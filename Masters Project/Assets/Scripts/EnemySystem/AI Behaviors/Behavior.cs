using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviorState
{
    Inactive,
    Active,
    Infinite
}

public abstract class Behavior : MonoBehaviour
{
    public abstract void StartBehavior();

    public abstract void UpdateBehavior();

    public abstract bool BehaviorFinished();
}
