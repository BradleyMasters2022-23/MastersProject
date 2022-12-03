using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class LedgeData : MonoBehaviour
{
    public float traverseTime = 0.8f;
    public LayerMask groundLayers;

    private void Awake()
    {
        MeshRenderer[] indicators= GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer indicator in indicators) 
        {
            indicator.enabled = false;
        }
    }

    [Button]
    public void GroundNodes()
    {
        if(transform.childCount== 0)
        {
            Debug.Log("No children to ground!");
            return;
        }    

        float height = 0;
        Vector3[] points = new Vector3[transform.childCount];

        // Get grounded node positions
        for(int i = 0; i < transform.childCount; i++) 
        {
            Transform t = transform.GetChild(i);

            RaycastHit h;
            if (Physics.Raycast(t.position, Vector3.down, out h, Mathf.Infinity, groundLayers))
            {
                points[i] = h.point;
                height += h.point.y;
            }
            else
            {
                Debug.Log($"Node named {t.name} does not have any ground to be placed on! " +
                    $"Did you forget to set the floor to the ground layer?");
                return;
            }
        }

        // adjust its core position between the heights
        transform.position = new Vector3(transform.position.x, height / transform.childCount, transform.position.z);

        // Apply new positions
        for (int i = 0; i < points.Length; i++)
        {
            Transform t = transform.GetChild(i);
            t.position = points[i];
        }
    }

    [Button]
    public void ResetNodes()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            t.localPosition = new Vector3(t.localPosition.x, 0f, t.localPosition.z);
        }
    }

    // this it for now
    // later, maybe try auto calculate it?
}
