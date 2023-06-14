using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Masters.UI
{
    public static class UIAnimatorUtils
    {
        /// <summary>
        /// Check if the animator has a state of this name
        /// </summary>
        /// <param name="anim">Animator to check</param>
        /// <param name="stateName">String name of the state</param>
        /// <param name="Layer">Layer to check. Default 0</param>
        /// <returns>Whether the animator has this state</returns>
        public static bool HasStateStr(this Animator anim, string stateName, int Layer = 0)
        {
            int id = Animator.StringToHash(stateName);
            return anim.HasState(Layer, id);
        }

        public static void ResetPlay(this Animator anim)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }
}

