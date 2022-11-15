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
    #region Variables

    /// <summary>
    /// Parent door reference
    /// </summary>
    private Door parent;
    /// <summary>
    /// Whether the player is in range
    /// </summary>
    private bool inProximity;
    /// <summary>
    /// Whether the door is currently open
    /// </summary>
    private bool open;
    /// <summary>
    /// Refernece to check routine
    /// </summary>
    private Coroutine checkRoutine;

    #endregion

    #region Initialization

    // Start is called before the first frame update
    void Awake()
    {
        parent = GetComponentInParent<Door>();
    }
    /// <summary>
    /// On enable, start the coroutine
    /// </summary>
    private void OnEnable()
    {
        checkRoutine = StartCoroutine(RepeatCheck());
    }
    /// <summary>
    /// On disable, stop the coroutine
    /// </summary>
    private void OnDisable()
    {
        if(checkRoutine != null)
        {
            StopCoroutine(checkRoutine);
            checkRoutine = null;
        }
    }

    #endregion

    #region Door Checking

    /// <summary>
    /// Check the door to see if it should open
    /// </summary>
    private void CheckDoor()
    {
        if (!parent.Locked && inProximity && !open)
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

    /// <summary>
    /// Check if the door is unlocked, whether to open the door
    /// </summary>
    /// <returns></returns>
    private IEnumerator RepeatCheck()
    {
        WaitForSeconds stagger = new WaitForSeconds(1f);

        while (true)
        {
            yield return stagger;

            CheckDoor();
        }
    }

    #endregion

    #region TriggerField Check

    /// <summary>
    /// Detect if the player is in range. Open if able
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!inProximity && other.tag == "Player")
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

    #endregion
}
