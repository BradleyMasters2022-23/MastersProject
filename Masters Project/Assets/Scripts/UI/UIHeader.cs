using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeader : MonoBehaviour
{
    [SerializeField] GameObject[] sections;
    [SerializeField] int startingIndex = 0;
    private int currentIndex;

    private void OnEnable()
    {
        GoToSection(0);
    }

    /// <summary>
    /// Go to a section, disabling the rest
    /// </summary>
    /// <param name="idx">index to enable</param>
    public void GoToSection(int idx)
    {
        if (idx < 0 || idx >= sections.Length || currentIndex == idx) return;
        currentIndex = idx;

        for (int i = 0; i < sections.Length; i++)
        {
            sections[i].gameObject.SetActive(currentIndex == i);
        }
    }

    /// <summary>
    /// Tab left, looping around to right if overdoing it
    /// </summary>
    public void TabLeft()
    {
        if(currentIndex == 0)
            GoToSection(sections.Length - 1);
        else
            GoToSection(currentIndex - 1);
    }

    /// <summary>
    /// Tab right, looping around to left if overdoing it
    /// </summary>
    public void TabRight()
    {
        if (currentIndex+1 == sections.Length)
            GoToSection(0);
        else
            GoToSection(currentIndex + 1);
    }
}
