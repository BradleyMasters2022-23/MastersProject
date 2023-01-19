using Cinemachine.Utility;
using Microsoft.Cci;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UIEnemyPointer : MonoBehaviour
{
    [SerializeField] private CameraShoot cameraRef;
    private Transform target;
    private Transform player;
    private bool initialized;

    private GameObject indicatorGraphic; 



    // Start is called before the first frame update
    void Awake()
    {
        initialized = false;
        player = GetComponentInParent<PlayerController>().transform;
        //cameraRef = FindObjectOfType<CameraShoot>(true);
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

        RotateToTarget();
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

    private void RotateToTarget()
    {

        // TODO
        // Make point based on center vision of player. Essentially, project the position difference to a 2D plane
        // and rotate like that. Not sure how tf to do it but try it

        Plane test = new Plane(cameraRef.transform.forward, cameraRef.transform.position);
        Vector3 direction = target.position - player.position;
        
        direction = Vector3.ProjectOnPlane(direction, cameraRef.transform.forward);
        
        //transform.LookAt(target);
        Vector3 r = Quaternion.LookRotation(direction).eulerAngles;
        r.z = 0;
        transform.rotation = Quaternion.Euler(r);

        
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
