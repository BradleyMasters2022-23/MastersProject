using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IStat : MonoBehaviour
{
    [SerializeField] protected Crystal.CrystalGroup group;
    [SerializeField] protected string plusKeyword;
    [SerializeField] protected string minusKeyword;

    public abstract void LoadStat(PlayerController player, int mod);

    public abstract void UnloadStat(PlayerController player, int mod);

    public string GetPlusKeyword()
    {
        return plusKeyword;
    }

    public string GetMinusKeyword()
    {
        return minusKeyword;
    }

    public Crystal.CrystalGroup GetGroup()
    {
        return group;
    }
}
