/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 17th, 2023
 * Last Edited - April 17th, 2023 by Ben Schuster
 * Description - A generic radar display to manage any targets
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRadar<T> : MonoBehaviour where T : Component
{
    [Tooltip("Prefab of pointer to aim at")]
    [SerializeField] private GenericRadarPointer arrow;

    private Queue<GenericRadarPointer> pointerQueue;

    [SerializeField] private int startingPts;

    [SerializeField] private int increaseIncrement;

    [SerializeField] private Color pointerColor;

    /// <summary>
    /// List of active targets in the scene
    /// </summary>
    private List<T> targetList;

    /// <summary>
    /// Reference to full display
    /// </summary>
    public RectTransform indicatorDisplay;

    /// <summary>
    /// Max dist. 
    /// </summary>
    public float maxDistance = Mathf.Infinity;

    protected virtual void Awake()
    {
        targetList = new List<T>();
        pointerQueue = new Queue<GenericRadarPointer>();

        // Load in initial amount as requested
        for (int i = 0; i < startingPts; i++)
        {
            GenericRadarPointer newPtr = Instantiate(arrow.gameObject, transform).GetComponent<GenericRadarPointer>();
            newPtr.Init(Return);
            newPtr.gameObject.SetActive(false);
            pointerQueue.Enqueue(newPtr);
        }
    }

    private void Update()
    {
        // Check for null references or deactivated targets, remove them
        for (int i = targetList.Count - 1; i >= 0; i--)
        {
            if (targetList[i] == null || !targetList[i].gameObject.activeInHierarchy)
            {
                targetList.RemoveAt(i);
            }
        }
        targetList.TrimExcess();

        // get all targets
        T[] targets = FindObjectsOfType<T>();

        // Check list for new targets. Add new pointers for each new target
        foreach (T t in targets)
        {
            if (!targetList.Contains(t))
            {
                targetList.Add(t);
                GenericRadarPointer ptr = GetPtr();
                ptr.SetTarget(t.transform, pointerColor,
                    indicatorDisplay.sizeDelta.y / 2, maxDistance);
                ptr.gameObject.SetActive(true);
            }
        }
    }

    private GenericRadarPointer GetPtr()
    {
        if (pointerQueue.Count <= 0)
            return ExpandPool();
        else
            return pointerQueue.Dequeue();
    }

    private GenericRadarPointer ExpandPool()
    {
        // Load in initial amount as requested
        for (int i = 0; i < increaseIncrement; i++)
        {
            GenericRadarPointer newPtr = Instantiate(arrow.gameObject, transform).GetComponent<GenericRadarPointer>();
            newPtr.Init(Return);
            newPtr.gameObject.SetActive(false);
            pointerQueue.Enqueue(newPtr);
        }

        // return a new one
        return pointerQueue.Dequeue();
    }

    public void Return(GenericRadarPointer ptr)
    {
        ptr.gameObject.SetActive(false);
        ptr.Return();
        pointerQueue.Enqueue(ptr);
    }
}
