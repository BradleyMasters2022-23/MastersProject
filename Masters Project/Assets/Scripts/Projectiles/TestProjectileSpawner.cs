using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectileSpawner : MonoBehaviour
{
    public float initialDelay;
    public float repeatDelay;

    public int numOfShotsPer;

    public GameObject projectile;
    public Transform shootPoint;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Shoot", initialDelay, repeatDelay);
    }

    public void Shoot()
    {
        for(int i = 0; i < numOfShotsPer; i++)
        {
            Quaternion lookRot = Quaternion.LookRotation(transform.forward, Vector3.up);
            Instantiate(projectile, shootPoint.position, lookRot);
        }
    }
}
