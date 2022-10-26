using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestUI : MonoBehaviour
{
    [SerializeField] string butts;
    [SerializeField] TextMeshProUGUI happiness;
    [SerializeField] private float health;
    [SerializeField] Slider toTheLeft;
    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        happiness.text = butts;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
    }


    void UpdateHealth()
    {
        currentHealth = toTheLeft.value;

        happiness.text = currentHealth.ToString();
    }
}
