/* ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 17th, 2022
 * Last Edited - October 17th, 2022 by Ben Schuster
 * Description - Holds all necessary values for an upgradable float value
 * ================================================================================================
 */
using Masters.CoreUpgradeVariables;
using UnityEngine;

[System.Serializable]
public class UpgradableFloat : UpgradableValue<float>
{
    /// <summary>
    /// Initialize variable with CSV solution
    /// </summary>
    public UpgradableFloat()
    {
        // TODO - Link to data organization CSV solution. 
        // Possibly override constructor with an ID value?
        // ID value points to the appropriate location?
    }

    public override void ChangeVal(float _newValue)
    {
        current = Mathf.Clamp(_newValue, lowerLimit, upperLimit);
    }
}
