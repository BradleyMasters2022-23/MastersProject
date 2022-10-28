using Masters.CoreUpgradeVariables;
using UnityEngine;

[System.Serializable]
public class UpgradableInt : UpgradableValue<int>
{
    public override void ChangeVal(int _newValue)
    {
      if(lowerLimit == 0 && upperLimit == 0) {
          current = _newValue;
      } else {
        current = Mathf.Clamp(_newValue, lowerLimit, upperLimit);
      }
    }

    public bool AtMax() {
      if(current == upperLimit) {
        return true;
      }
      return false;
    }

    public void Increment(int incrementValue) {
      ChangeVal(current + incrementValue);
    }
}
