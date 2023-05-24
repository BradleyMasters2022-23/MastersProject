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
        private float scaledTimeline = 0;

        /// <summary>
        /// The current internal timer, adjusted with timescale. 
        /// </summary>
        public float ScaledTimeline
        {
            get { return scaledTimeline; }
        }

        private float unscaledTimeline = 0;

        public float UnscaledTimeline
        {
            get { return unscaledTimeline; }
        }

        /// <summary>
        /// Prepare instance, initialize timer
        /// </summary>
        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

            CoreTimeline.instance = this;
            unscaledTimeline = 0;
            scaledTimeline = 0;
        }

        /// <summary>
        /// Update the timeline using the world's adjusted timeline
        /// </summary>
        private void FixedUpdate()
        {
            scaledTimeline += TimeManager.WorldDeltaTime;
            unscaledTimeline += Time.deltaTime;

            if (scaledTimeline == float.MaxValue)
                Debug.LogError("Timeline reached max float value, what the fuck???");
        }

        private void OnDisable()
        {
            instance = null;
        }
    }
}

