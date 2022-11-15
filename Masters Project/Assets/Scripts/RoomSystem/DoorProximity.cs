/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 14, 2022
 * Last Edited - November 14, 2022 by Ben Schuster
 * Description - Controls opening and closing the player doors when player gets close
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorProximity : MonoBehaviour
{
    /// <summary>
    /// Parent door reference
    /// </summary>
    [SerializeField] private Door parent;
    /// <summary>
    /// Reference to collider
    /// </summary>
    private Collider col;

    /// <summary>
    /// Whether the door parent is locked
    /// </summary>
    //[SerializeField] private bool locked;
    /// <summary>
    /// Whether the player is in range
    /// </summary>
    [SerializeField] private bool inProximity;
    
    /// <summary>
    /// Whether the door is currently open
    /// </summary>
    [SerializeField] private bool open;
    /// <summary>
    /// Refernece to check routine
    /// </summary>
    private Coroutine checkRoutine;

    // Start is called before the first frame update
    void Awake()
    {
        parent = GetComponentInParent<Door>();
        col = GetComponent<Collider>();
    }
    /// <summary>
    /// On disable, stop the coroutine
    /// </summary>
    private void OnDisable()
    {
        //StopCoroutine(checkRoutine);
        checkRoutine = null;
    }
    /// <summary>
    /// On enable, start the coroutine
    /// </summary>
    private void OnEnable()
    {
        //checkRoutine = StartCoroutine(CheckDoor());
    }

    private void Update()
    {

        if(!parent.Locked && inProximity && !open)
        {
            open = true;
            parent.SetOpenStatus(true);
        }
        else if (open && (parent.Locked || !inProximity))
        {
            open = false;
            parent.SetOpenStatus(false);
        }
    }

    ///// <summary>
    ///// Check if the door is unlocked, whether to open the door
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator CheckDoor()
    //{
    //    WaitForSeconds stagger = new WaitForSeconds(1f);

    //    while(true)
    //    {
    //        yield return stagger;
            
    //        // Check if the lock state changed
    //        if(locked != parent.Locked)
    //        {
    //            locked = parent.Locked;

    //            // If it was set to locked and in prox, close door
    //            if(locked && inProximity)
    //            {
    //                open = false;
    //                parent.SetOpenStatus(false);
    //            }
    //            // If its unlocked and in range, open it
    //            else if(!locked && inProximity)
    //            {
    //                open = true;
    //                parent.SetOpenStatus(true);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Detect if the player is in range. Open if able
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(!inProximity && other.tag == "Player")
        {
            inProximity = true;
        }
    }
    /// <summary>
    /// Detect if the player is out of range. Close if needed
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (inProximity && other.tag == "Player")
        {
            inProximity = false;
        }
    }
}
