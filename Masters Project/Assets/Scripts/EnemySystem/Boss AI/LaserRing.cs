using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LaserRingSettings
{
    public string name;
    public int numberOfLasers;
    public float rotationSpeed;
}

public class LaserRing : MonoBehaviour
{
    [SerializeField] private LaserRingSettings[] settings;

    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserDuration;
    [SerializeField] float laserOffset;

    float rotationSpeed;
    private RangeAttack[] firedLasers;

    private bool active = false;

    /// <summary>
    /// Activate the laser ring
    /// </summary>
    /// <param name="num">number of lasers to fire</param>
    public void Activate(int dataIndex)
    {
        if (dataIndex >= settings.Length) return;

        firedLasers = new RangeAttack[settings[dataIndex].numberOfLasers];
        rotationSpeed= settings[dataIndex].rotationSpeed;
        float split = 360 / settings[dataIndex].numberOfLasers;

        for(int i = 0; i < settings[dataIndex].numberOfLasers; i++)
        {
            // fire and initialize
            GameObject l = Instantiate(laserPrefab, transform);
            firedLasers[i] = l.GetComponent<RangeAttack>();
            firedLasers[i].Initialize(1, 1, laserDuration, gameObject);

            // rotate and position appropriately
            l.transform.Rotate(l.transform.up, (split * i));
            l.transform.localPosition += l.transform.forward * (0.5f + laserOffset);
        }
        active = true;
    }

    public void Inturrupt()
    {
        active = false;

        transform.rotation = Quaternion.identity;

        foreach (RangeAttack laser in firedLasers)
            laser.Inturrupt();

        firedLasers = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
            transform.rotation *= Quaternion.Euler(0, rotationSpeed * TimeManager.WorldTimeScale, 0);
    }
}
