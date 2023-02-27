/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 27th, 2022
 * Last Edited - February 27th, 2022 by Ben Schuster
 * Description - Interface for any map randomizing systems that is called by map generators.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRandomizer 
{
    /// <summary>
    /// Perform any necessary randomization functions
    /// </summary>
    void Randomize();
}
