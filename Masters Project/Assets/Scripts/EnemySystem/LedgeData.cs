using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class LedgeData : MonoBehaviour
{
    public float traverseTime = 0.8f;
    public LayerMask groundLayers;

    /// <summary>
    /// When game starts, hide the visual indicators
    /// </summary>
    private void Awake()
    {
        MeshRenderer[] indicators= GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer indicator in indicators) 
        {
            indicator.enabled = false;
        }
    }

    /// <summary>
    /// Ground the nodes to the first point of ground below them
    /// </summary>
    [Button]
    public void GroundNodes()
    {
        if(transform.childCount== 0)
        {
            Debug.Log("No children to ground!");
            return;
        }

        float xDist = 0;
        float yDist = 0;
        float zDist = 0;
        
        Vector3[] points = new Vector3[transform.childCount];

        // Get grounded node positions
        for(int i = 0; i < transform.childCount; i++) 
        {
            Transform t = transform.GetChild(i);

            // Add tiny upwards offset
            RaycastHit h;
            if (Physics.Raycast(t.position + Vector3.up * 0.1f, Vector3.down, out h, Mathf.Infinity, groundLayers))
            {
                points[i] = h.point;
                xDist += h.point.x;
                yDist += h.point.y;
                zDist += h.point.z;
            }
            else
            {
                Debug.Log($"Node named {t.name} does not have any ground to be placed on! " +
                    $"Did you forget to set the floor to the ground layer?");
                return;
            }
        }

        // adjust its core position between the yDists
        transform.position = new Vector3(xDist / transform.childCount, yDist / transform.childCount, zDist / transform.childCount);

        // Apply new positions
        for (int i = 0; i < points.Length; i++)
        {
            Transform t = transform.GetChild(i);
            t.position = points[i];
        }
    }

    /// <summary>
    /// Reset the node's Y heights to flat
    /// </summary>
    [Button]
    public void ResetNodes()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            t.localPosition = new Vector3(t.localPosition.x, 0f, t.localPosition.z);
        }
    }

    /// <summary>
    /// Draw lines under the nodes to make it easier to see where they will ground to
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        //Debug.Log("draw gizmos selected called");

        Gizmos.color = Color.yellow;

        // Get grounded node positions
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            RaycastHit h;
            if (Physics.Raycast(t.position + Vector3.up * 0.1f, Vector3.down, out h, Mathf.Infinity, groundLayers))
            {
                if(h.distance >= 0.3f)
                {
                    Gizmos.DrawLine(t.position, h.point);
                    Gizmos.DrawSphere(h.point, 0.1f);
                }
            }
        }
    }
}
