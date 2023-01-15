using UnityEngine;

namespace MagicBedlam
{
    /// <summary>
    /// Creating singleton pattern
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// Create singleton, if it doesn't exist else destroy current object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <param name="singleton"></param>
        /// <returns>Current if singleton doesn't exist else singleton</returns>
        public static T Create<T>(T current, T singleton) where T : MonoBehaviour
        {
            if (singleton != null && singleton != current)
            {
                Debug.LogError($"Object {typeof(T)} can exist in a single instance. The second one will be destroyed");
                GameObject.Destroy(current.gameObject);
                return singleton;
            }

            return current;
        }
    }
}