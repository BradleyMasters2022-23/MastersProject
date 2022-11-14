/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 13th, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Change the physics layer of the player when entering
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerFieldChanger : MonoBehaviour
{
    [Tooltip("What layer should the target be set to within this field?")]
    [SerializeField] private string targetLayerName;

    /// <summary>
    /// What is the original layer the target entered
    /// </summary>
    [SerializeField] private int originalLayer;
    /// <summary>
    /// What is the target layer to apply on targets who enter
    /// </summary>
    [SerializeField] private int targetLayer;

    /// <summary>
    /// Initialize, get the appropriate layer mask
    /// </summary>
    private void Awake()
    {
        targetLayer = LayerMask.NameToLayer(targetLayerName);
    }

    /// <summary>
    /// When entering field, change its physics layer
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject.layer != targetLayer)
        {
            originalLayer = other.gameObject.layer;
            other.gameObject.layer = targetLayer;
        }
    }

    /// <summary>
    /// When exiting field, change its physics layer back
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.layer = originalLayer;
        }
    }
}
