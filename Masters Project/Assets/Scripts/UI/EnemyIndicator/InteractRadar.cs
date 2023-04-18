using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractRadar : GenericRadar<WaypointContainer>
{
    /// <summary>
    /// When pointing to a waypoint container, load in its
    /// container color for consistency
    /// </summary>
    /// <param name="data"></param>
    /// <returns>Color of container to use</returns>
    protected override Color ChooseCol(WaypointContainer data)
    {
        return data.GetData().displayColor;
    }
}
