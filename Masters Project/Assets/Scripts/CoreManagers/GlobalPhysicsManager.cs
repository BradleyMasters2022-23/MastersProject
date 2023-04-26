using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalPhysicsManager : MonoBehaviour
{
    [Tooltip("The global hit mask for whats ground or other physics objects")]
    [SerializeField] protected LayerMask globalWorldLayers;

    /// <summary>
    /// Get the layers this object has
    /// </summary>
    /// <returns>Layer mask for all ground objects</returns>
    public LayerMask GetWorldLayers() 
    { 
        return globalWorldLayers; 
    }
}
