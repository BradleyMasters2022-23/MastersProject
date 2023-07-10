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
    [SerializeField] protected GenericRadarPointer arrow;

    protected Queue<GenericRadarPointer> pointerQueue;

    [SerializeField] protected int startingPts;

    [SerializeField] protected int increaseIncrement;

    [SerializeField] protected Color pointerColor;

    /// <summary>
    /// List of active targets in the scene
    /// </summary>
    protected List<T> targetList;

    /// <summary>
    /// Reference to full display
    /// </summary>
    public RectTransform indicatorDisplay;

    /// <summary>
    /// Max dist. 
    /// </summary>
    public float maxDistance = Mathf.Infinity;

    protected void Awake()
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

    protected void Update()
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
        T[] targets = FindObjectsOfType<T>(false);

        // Check list for new targets. Add new pointers for each new target
        foreach (T t in targets)
        {
            if (!targetList.Contains(t))
            {
                targetList.Add(t);
                GenericRadarPointer ptr = GetPtr();
                ptr.SetTarget(t.transform, ChooseCol(t),
                    indicatorDisplay.sizeDelta.y / 2, maxDistance);
                ptr.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// By default, return the normal color
    /// </summary>
    /// <param name="data">data to load in</param>
    /// <returns>Color to us for the pointer</returns>
    protected virtual Color ChooseCol(T data)
    {
        return pointerColor;
    }

    protected GenericRadarPointer GetPtr()
    {
        if (pointerQueue.Count <= 0)
            return ExpandPool();
        else
            return pointerQueue.Dequeue();
    }

    protected GenericRadarPointer ExpandPool()
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
