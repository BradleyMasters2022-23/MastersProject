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
    [SerializeField] private GameObject indicatorGraphic;
    private RectTransform indicatorRect;
    [SerializeField] private GameObject maxDistGraphic;
    private RectTransform maxDistRect;
    [SerializeField] private bool camViewDisable;
    private float maxDistance;
    private float radarRadius;
    [SerializeField] private bool useDistance;

    [SerializeField] private float radiusMod;

    private Vector2 distRect;

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
        //if (!indicatorGraphic.activeSelf)
        //{
        //    return;
        //}

        RotateToTargetFlat();
        CheckDistance();
        //RotateToTargetPop();
    }

    private void CheckDistance()
    {
        if (!useDistance)
            return;

        float currDist = Mathf.Abs(Vector3.Distance(player.position, target.position));

        float newPix =  Mathf.Clamp01((currDist / maxDistance)) * radarRadius;
        distRect.y = newPix;

        indicatorRect.anchoredPosition = distRect;
        maxDistRect.anchoredPosition = distRect;


        if (currDist >= maxDistance && !maxDistGraphic.activeSelf)
        {
            //Debug.Log("Toggling to max range arrow");
            maxDistGraphic.SetActive(true);
            indicatorGraphic.SetActive(false);
        }
        else if (currDist < maxDistance && !indicatorGraphic.activeSelf)
        {
            //Debug.Log("Toggling to normal dot");
            indicatorGraphic.SetActive(true);
            maxDistGraphic.SetActive(false);
        }
    }

    private void CheckEnemyVisibility()
    {
        if(camViewDisable)
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

        //Debug.Log("rotate to target called");
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
    public void SetTarget(GameObject _target, float rad = 0, float maxDist = 0)
    {
        target = _target.transform;
        //indicatorGraphic = transform.GetChild(0).gameObject;
        cameraRef = Camera.main.GetComponent<CameraShoot>();

        radarRadius = rad - radiusMod;
        maxDistance = maxDist;

        if(indicatorGraphic!= null)
            indicatorRect = indicatorGraphic.GetComponent<RectTransform>();
        if(maxDistGraphic != null)
            maxDistRect = maxDistGraphic.GetComponent<RectTransform>();

        distRect = indicatorRect.anchoredPosition;

        initialized = true;
    }
}
