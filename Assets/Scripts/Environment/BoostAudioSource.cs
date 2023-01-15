using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    [RequireComponent(typeof(AudioSource))]
    public class BoostAudioSource : NetworkBehaviour
    {
        [SerializeField] protected AudioSource _ownAudio;

        [ClientRpc]
        public void RpcPlay()
        {
            _ownAudio.Play();
        }

        protected void Reset() 
        {
            TryGetComponent<AudioSource>(out _ownAudio);
        }
    }
}