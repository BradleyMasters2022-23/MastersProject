/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 15th, 2023
 * Last Edited - June 15th, 2023 by Ben Schuster
 * Description - Single collectable instance that loads in-world objects 
 * based on completion of a collectable. On interact, this only sends 
 * a single instance of the prop, useful for collectables with repeat props
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCollectableInstanceSingle : LoadCollectableInstance
{
    public override void OnInteract()
    {
        CollectableViewUI ui = CollectableScreenInstance.instance.UI;
        ui.OpenUI(collectableData, 0);
    }
}
