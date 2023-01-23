using Cinemachine.Utility;
using Microsoft.Cci;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UIEnemyPointer : MonoBehaviour
{
    private CameraShoot cameraRef;
    private RectTransform t;
    private Transform target;
    private Transform player;
    private bool initialized;

    private GameObject indicatorGraphic; 



    // Start is called before the first frame update
    void Awake()
    {
        initialized = false;
        player = FindObjectOfType<PlayerController>(true).transform;
        t = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
            return;

        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Make visible only off screen
        CheckEnemyVisibility();

        // Dont bother with other functionality if the indicator isnt on
        if (!indicatorGraphic.activeSelf)
        {
            return;
        }

        RotateToTargetFlat();
        RotateToTargetPop();
    }

    private void CheckEnemyVisibility()
    {
        if (cameraRef != null)
        {
            if (cameraRef.InCamVision(target.position))
            {
                if (indicatorGraphic.activeSelf)
                {
                    indicatorGraphic.SetActive(false);
                }
            }
            else
            {
                if (!indicatorGraphic.activeSelf)
                {
                    indicatorGraphic.SetActive(true);
                }
            }
        }
    }

    private void RotateToTargetFlat()
    {
        Vector3 direction = target.position - player.position;

        Quaternion r = Quaternion.LookRotation(direction);
        r.z = -r.y;
        r.x = 0;
        r.y = 0;

        Vector3 north = new Vector3(0, 0, player.eulerAngles.y);
        t.rotation = r * Quaternion.Euler(north);
    }

    private void RotateToTargetPop()
    {
        //Plane plane = new Plane(cameraRef.transform.forward, cameraRef.transform.position);
        
        Vector3 direction = 
            Vector3.ProjectOnPlane(target.position-player.position, cameraRef.transform.forward);

        Debug.DrawRay(transform.position, direction, Color.red);

        /*
        Quaternion r = Quaternion.LookRotation(direction);
        r.z = -r.y;
        r.x = 0;
        r.y = 0;

        Vector3 north = new Vector3(0, 0, player.eulerAngles.y);
        t.rotation = r * Quaternion.Euler(north);
        */
    }

    /// <summary>
    /// Set the target enemy
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(GameObject _target)
    {
        target = _target.transform;
        indicatorGraphic = transform.GetChild(0).gameObject;
        cameraRef = Camera.main.GetComponent<CameraShoot>();
        initialized = true;
    }
}
