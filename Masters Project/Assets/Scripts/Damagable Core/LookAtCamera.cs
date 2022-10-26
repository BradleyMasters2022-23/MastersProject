/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Make an object look at the camera. Useful for in-game UI
 * ================================================================================================
 */
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    /// <summary>
    /// Reference to the main camera
    /// </summary>
    private Camera mainCamera;

    // When enabled, look at the camera
    void OnEnable()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Makes the object look at camera 
    /// </summary>
    void Update()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
