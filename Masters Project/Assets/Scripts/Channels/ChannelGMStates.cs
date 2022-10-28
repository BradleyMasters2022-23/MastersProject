/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 27th, 2022
 * Last Edited - October 27th, 2022 by Ben Schuster
 * Description - Concrete channel that takes the game manager state input
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Gamemanager State Channel", menuName = "Channels/Gamemanager State Channel")]
public class ChannelGMStates : ChannelBase<GameManager.States>
{
}
