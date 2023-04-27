using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericField : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onEnter.Invoke();
            gameObject.SetActive(false);
        }
    }
}
