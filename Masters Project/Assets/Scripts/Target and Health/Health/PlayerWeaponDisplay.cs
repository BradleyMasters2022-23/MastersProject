using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponDisplay : MonoBehaviour
{
    [Tooltip("Target image dispay to use")]
    [SerializeField] Image targetDisplay;
    [Tooltip("Sprites to utilize")]
    [SerializeField] Sprite[] mainImages;
    [Tooltip("The max amount for this threshold. Keep it organized (Highest values to lowest values) and ordered with Target Display." +
        "EX. If index 3 is set to 30, then index 3 will be selected if the player's health is AT or BELOW 30.")]
    [SerializeField] float[] healthThreshold;
    /// <summary>
    /// Player health manager
    /// </summary>
    private HealthManager playerHealth;
    /// <summary>
    /// Target  index for the main health
    /// </summary>
    private const int targetHealthIndex = 0;

    private void OnEnable()
    {
        StartCoroutine(WaitForPlayerLoad());    
    }

    private IEnumerator WaitForPlayerLoad()
    {
        yield return new WaitUntil(()=> PlayerTarget.p != null);

        playerHealth = PlayerTarget.p.GetComponent<PlayerHealthManager>();
        playerHealth.onDamagedEvent += UpdateImage;
        playerHealth.onHealEvent += UpdateImage;
        UpdateImage();
    }
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.onDamagedEvent -= UpdateImage;
            playerHealth.onHealEvent -= UpdateImage;
        }
    }

    /// <summary>
    /// update image on the gun based on player health thresholds
    /// </summary>
    private void UpdateImage()
    {
        // get ref to current health
        float currVal = playerHealth.CurrentHealth(targetHealthIndex);

        // loop through thresholds and find the relevant index to use
        int idx;
        for(idx = 0; idx < healthThreshold.Length; idx++)
        {
            // if the current value is greater than this threshold, break out
            if (currVal > healthThreshold[idx])
            {
                break;
            }
        }

        idx = Mathf.Clamp(idx - 1, 0, mainImages.Length - 1);
        // apply new image if its not already set
        if (targetDisplay.sprite != mainImages[idx])
        {
            targetDisplay.sprite = mainImages[idx];
        }
    }
}
