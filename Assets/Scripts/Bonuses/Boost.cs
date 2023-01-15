using System.Collections;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    ///     Representation a boost object
    /// </summary>
    [ExecuteInEditMode]
    public class Boost : MonoBehaviour
    {
        [Tooltip("The player jump force multiplier")]
        [SerializeField]
        [Range(1, 5)]
        protected float _jumpMultiplier;
        [SerializeField]
        protected BoostAudioSource _ownAudio;

        [Header("EditMode Data")]
        [Tooltip("Set the object to state when the player stay on it")]
        [SerializeField] 
        protected bool _isPrepared;

        protected float _delay = 0.5f;
        protected bool _isLastPrepared;
        protected int _numOfPlayers;
        protected Vector3 _startPos;

        protected void Awake() 
        {
            _startPos = transform.position;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                other.GetComponent<Mover>().JumpMultiplier = _jumpMultiplier;
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                other.GetComponent<Mover>().JumpMultiplier = 1;
                
                if(other.attachedRigidbody.velocity.y > 6)
                {
                    _ownAudio.RpcPlay();
                }
            }
        }

#if UNITY_EDITOR

        protected void Update()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (_isPrepared != _isLastPrepared)
                {
                    transform.position += Vector3.up * 0.19f * (_isPrepared ? -1 : 1);
                }
                _isLastPrepared = _isPrepared;
            }
        }

#endif
    }
}
