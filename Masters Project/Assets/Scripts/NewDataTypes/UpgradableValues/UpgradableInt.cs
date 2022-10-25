using Masters.CoreUpgradeVariables;
using UnityEngine;

[System.Serializable]
public class UpgradableInt : UpgradableValue<int>
{
    public override void ChangeVal(int _newValue)
    {
        current = Mathf.Clamp(_newValue, lowerLimit, upperLimit);
    }

    public bool AtMax() {
      if(current == upperLimit) {
        return true;
      }
      return false;
    }
}
