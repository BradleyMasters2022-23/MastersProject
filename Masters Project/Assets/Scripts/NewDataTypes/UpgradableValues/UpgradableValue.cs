/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 17th, 2022
 * Last Edited - October 17th, 2022 by Ben Schuster
 * Description - Base structure for upgradable values.
 * ================================================================================================
 */
using UnityEngine;

namespace Masters.CoreUpgradeVariables
{
    /// <summary>
    /// A value prone to be modified through upgrades.
    /// </summary>
    /// <typeparam name="T">Numeric Type of this variable (float, int, etc).</typeparam>
    public abstract class UpgradableValue<T> where T : notnull
    {
        /// <summary>
        /// The current value
        /// </summary>
        protected T current;
        /// <summary>
        /// Get the current value
        /// </summary>
        public T Current
        {
            get { return current; }
        }

        [Tooltip("What does this value start at?")]
        [SerializeField] protected T startingValue;

        [Tooltip("What is this value's lower limit?")]
        [SerializeField] protected T lowerLimit;

        [Tooltip("What is this value's upper limit?")]
        [SerializeField] protected T upperLimit;

        /// <summary>
        /// Initialize the current value to the starting value
        /// TODO - Call via CSV system
        /// </summary>
        public void Initialize()
        {
            current = startingValue;
        }

        /// <summary>
        /// Change the current value to a new value. Automatically clamps based on limits.
        /// </summary>
        /// <param name="_newValue">New value to set to.</param>
        public abstract void ChangeVal(T _newValue);
    }
}

