using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Representation the water object
    /// </summary>
    public class Water : MonoBehaviour
    {
        [Tooltip("A drag added to objects falling into the water")]
        [SerializeField]
        [Range(0, 20)]
        protected float _extraDrag;
        [Tooltip("The level of water which splashes spawn on")]
        [SerializeField] 
        protected Transform _levelOfWater;
        [Tooltip("The object of water splashes appeared when any object falls into the water")]
        [SerializeField]
        protected GameObject _waterSplashes;

        [ServerCallback]
        protected void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody && !other.isTrigger)
            {
                other.attachedRigidbody.drag += _extraDrag;
                other.attachedRigidbody.angularDrag += _extraDrag;

                Vector3 sprayPos = other.transform.position;
                sprayPos.y = _levelOfWater.position.y;
                NetworkServer.Spawn(Instantiate(_waterSplashes, sprayPos, Quaternion.identity));
            }
        }

        [ServerCallback]
        protected void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody && !other.isTrigger)
            {
                other.attachedRigidbody.drag -= _extraDrag;
                other.attachedRigidbody.angularDrag -= _extraDrag;
            }
        }
    }
}