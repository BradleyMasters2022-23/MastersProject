/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 19th, 2022
 * Last Edited - October 19th, 2022 by Ben Schuster
 * Description - Manages the core timeline, taking into account player's time manipulation
 * ================================================================================================
 */
using UnityEngine;

namespace Masters.TimerSystem
{
    public class CoreTimeline : MonoBehaviour
    {
        /// <summary>
        /// Core instance of this object
        /// </summary>
        public static CoreTimeline instance;

        /// <summary>
        /// The current internal timer, adjusted with timescale. 
        /// </summary>
        private float scaledTimeline;

        /// <summary>
        /// The current internal timer, adjusted with timescale. 
        /// </summary>
        public float ScaledTimeline
        {
            get { return scaledTimeline; }
        }

        /// <summary>
        /// Prepare instance, initialize timer
        /// </summary>
        private void Awake()
        {
            if (CoreTimeline.instance != null && CoreTimeline.instance != this)
            {
                Destroy(this);
            }

            CoreTimeline.instance = this;
            DontDestroyOnLoad(this);

            scaledTimeline = 0;
        }

        /// <summary>
        /// Update the timeline using the world's adjusted timeline
        /// </summary>
        private void FixedUpdate()
        {
            scaledTimeline += TimeManager.WorldDeltaTime;
            
            if (scaledTimeline == float.MaxValue)
                Debug.LogError("Timeline reached max float value, what the fuck???");
        }
    }
}