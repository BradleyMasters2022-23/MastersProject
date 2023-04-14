/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - March 29 2023
 * Last Edited - March 30 2023
 * Description - framework for a stat
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public abstract class IStat : MonoBehaviour
{
    /// <summary>
    /// Defines stat groups. These determine a crystal's color.
    /// </summary>
    public enum StatGroup
    {
        HEALTH,
        GUN,
        GRENADE,
        MOVEMENT,
        TIMESTOP,
        SHIELD
    }
    [Tooltip("Group the stat belongs to. Determines color.")]
    [SerializeField] protected StatGroup group;

    [Tooltip("Name keyword for crystals with a + in this stat.")]
    [SerializeField] protected string plusKeyword;

    [Tooltip("Name keyword for crystals with a - in this stat.")]
    [SerializeField] protected string minusKeyword;

    [SerializeField] protected string statText;
    [SerializeField] protected Color color;
    [SerializeField] protected Sprite sprite;

    /// <summary>
    /// Loads the given stat to the player with the provided mod
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="mod">Stat modifier</param>
    public abstract void LoadStat(PlayerController player, int mod);

    /// <summary>
    /// Removes the given stat from the player with the provided mod
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="mod">Stat modifier</param>
    public abstract void UnloadStat(PlayerController player, int mod);

    public abstract float GetStatIncrease(int mod);

    /// <summary>
    /// Gets positive keyword 
    /// </summary>
    /// <returns>Positive name keyword of given stat</returns>
    public string GetPlusKeyword()
    {
        return plusKeyword;
    }

    /// <summary>
    /// Gets negative keyword
    /// </summary>
    /// <returns>Negative name keyword of given stat</returns>
    public string GetMinusKeyword()
    {
        return minusKeyword;
    }

    /// <summary>
    /// Gets stat group
    /// </summary>
    /// <returns>Group the stat belongs to</returns>
    public StatGroup GetGroup()
    {
        return group;
    }

    public string GetStatText()
    {
        return statText;
    }

    public Sprite GetIcon()
    {
        return sprite;
    }

    public Color GetColor()
    {
        return color;
    }
}
