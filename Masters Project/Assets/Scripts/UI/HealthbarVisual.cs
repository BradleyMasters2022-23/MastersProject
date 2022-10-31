/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Observe player health and update healthbar
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthbarVisual : MonoBehaviour
{
    [Tooltip("Each textfield for the object. Index is: 0 = left most number, 2 = right most number")]
    [SerializeField] private TextMeshProUGUI[] counter = new TextMeshProUGUI[3];

    /// <summary>
    /// Player to track health of
    /// </summary>
    private PlayerHealth health;
    /// <summary>
    /// Slider of healthbar
    /// </summary>
    private Slider healthbar;

    private void Awake()
    {
        health = FindObjectOfType<PlayerHealth>(true);
        healthbar = GetComponentInChildren<Slider>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // As health starts max
        healthbar.maxValue = health.MaxHealth();
        healthbar.value = health.CurrHealth;
    }
    private void Update()
    {
        healthbar.value = health.CurrHealth;

        if(healthbar.maxValue != health.MaxHealth())
        {
            healthbar.maxValue = health.MaxHealth();
        }

        ParseNum((int)healthbar.value);
    }

    /// <summary>
    /// Parse a number into a string across the 3 text objects
    /// </summary>
    /// <param name="_num">number to parse</param>
    private void ParseNum(int _num)
    {
        string temp = $"{_num:000}";

        counter[0].text = temp.Substring(0, 1);

        counter[1].text = temp.Substring(1, 1);

        counter[2].text = temp.Substring(2, 1);
    }
}
