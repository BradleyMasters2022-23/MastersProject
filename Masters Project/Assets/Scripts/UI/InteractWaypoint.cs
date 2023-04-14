using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractWaypoint : MonoBehaviour
{
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private Transform target;

    private float dist;
    [SerializeField] private Image t;

    float minX;
    float maxX;
    float minY;
    float maxY;

    private void Awake()
    {
        minX = t.GetPixelAdjustedRect().width / 2;
        maxX = Screen.width - minX;

        minY = t.GetPixelAdjustedRect().height/ 2;
        maxY = Screen.height - minY;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        if(Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
                pos.x = maxX;
            else
                pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxY);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        t.transform.position = pos;
        

    }
}
