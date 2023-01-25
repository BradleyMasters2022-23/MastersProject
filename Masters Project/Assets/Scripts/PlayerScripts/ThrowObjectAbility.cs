using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectAbility : Ability
{
    protected override IEnumerator OnAbility()
    {
        Debug.Log("Throw object ability has been used!");
        yield return null;
    }

}
