using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    ///     Keep fire particles side up
    /// </summary>
    public class Fire : MonoBehaviour
    {
        [ServerCallback]
        protected void FixedUpdate()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}