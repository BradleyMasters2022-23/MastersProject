/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 25, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Defines a spawnable container of a single fragment.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentContainer : MonoBehaviour
{
    [Tooltip("The fragment this container represents.")]
    [SerializeField] private Fragment fragment;

    [Tooltip("Color the container should appear as.")]
    [SerializeField] private Color color;

    /// <summary>
    /// ensures that fragment is not null and calls SetUp
    /// </summary>
    private void Start()
    {
        if (fragment is null)
        {
            Destroy(this);
        }

        if(fragment != null)
        {
            SetUp(fragment);
        }
    }

    /// <summary>
    /// called exactly once, initializes container
    /// </summary>
    public void SetUp(Fragment obj)
    {
        fragment = obj;
        GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// called when player walks into the object. eventually change to a button?
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // TODO: trigger note display screen.
            PlayerNotesManager.instance.FindFragment(fragment);
            Destroy(this.gameObject);
        }
    }
}
