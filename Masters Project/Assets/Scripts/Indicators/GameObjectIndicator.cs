/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22nd, 2022
 * Last Edited - February 22nd, 2022 by Ben Schuster
 * Description - Concrete indicator implementation for simple gameobjects
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectIndicator : IIndicator
{
    [SerializeField] private GameObject indicator;

    private void Awake()
    {
        indicator.SetActive(false);
    }

    public override void Activate()
    {
        indicator.SetActive(true);
    }

    public override void Deactivate()
    {
        indicator.SetActive(false);
    }
}
