/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 16th, 2023
 * Last Edited - June 16th, 2023
 * Description - Channel that uses control schemes to process data
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/ControlSchemeChannel", fileName = "New Control Scheme Channel")]
public class ChannelControlScheme : ChannelBase<InputManager.ControlScheme>
{
}
