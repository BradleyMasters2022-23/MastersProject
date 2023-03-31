using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceVanish : MonoBehaviour
{
    [SerializeField] private RectTransform visualsRect;
    private Transform target;
    
    private float loadDistance = 30f;
    [SerializeField] private float distanceOffset;

    private void Start()
    {
        PlayerController tempPlayer = FindObjectOfType<PlayerController>();
        if (tempPlayer != null)
        {
            target= tempPlayer.transform;
        }

        PlayerGunController rangeTemp = FindObjectOfType<PlayerGunController>();
        if (rangeTemp != null)
            loadDistance = rangeTemp.GetMaxRange();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            PlayerController temp = FindObjectOfType<PlayerController>();
            
            if(temp != null)
                target = temp.transform;
            
            return;
        }

        // Debug.Log($"Target : {target == null} | Rect : {visualsRect == null}");
        float dist = Mathf.Abs(Vector3.Distance(target.position, visualsRect.position));

        if (dist <= loadDistance + distanceOffset && !visualsRect.gameObject.activeInHierarchy)
        {
            visualsRect.gameObject.SetActive(true);
        }
        else if(dist > loadDistance + distanceOffset && visualsRect.gameObject.activeInHierarchy)
        {
            visualsRect.gameObject.SetActive(false);
        }
    }
}
