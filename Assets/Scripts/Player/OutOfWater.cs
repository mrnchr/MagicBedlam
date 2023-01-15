using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Object controlled how high the player can jump out of the water
    /// </summary>
    public class OutOfWater : MonoBehaviour
    {
        [Tooltip("Own Mover component")]
        [SerializeField] 
        protected Mover _mover;

        [ServerCallback]
        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _mover.CanSwimJump = true;
            }
        }

        [ServerCallback]
        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _mover.CanSwimJump = false;
            }
        }

        protected void Reset() {
            TryGetComponent<Mover>(out _mover);
        }
    }
}
