using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    public enum PlayerSound 
    {
        Score,
        Jump,
        Death,
        Pick,
        Throw
    }

    [RequireComponent(typeof(AudioSource))]
    public class PlayerAudioSource : NetworkBehaviour
    {
        [SerializeField] protected AudioSource _ownAudio;
        [SerializeField] protected AudioClip _scoreAudio;
        [SerializeField] protected AudioClip _jumpAudio;
        [SerializeField] protected AudioClip _deathAudio;
        [SerializeField] protected AudioClip _pickAudio;
        [SerializeField] protected AudioClip _throwAudio;

        [ClientRpc]
        public void RpcPlay(PlayerSound soundType) {
            _ownAudio.clip = null;
            switch (soundType)
            {
                case PlayerSound.Score : 
                    if(isLocalPlayer) 
                        _ownAudio.clip = _scoreAudio;
                break;
                case PlayerSound.Jump : _ownAudio.clip = _jumpAudio;
                    break;
                case PlayerSound.Death : 
                    if(isLocalPlayer)
                        _ownAudio.clip = _deathAudio;
                break;
                case PlayerSound.Pick : _ownAudio.clip = _pickAudio;
                    break;
                case PlayerSound.Throw : _ownAudio.clip = _throwAudio;
                    break;
            }

            _ownAudio.Play();
        }
        
        protected void Reset() 
        {
            TryGetComponent<AudioSource>(out _ownAudio);
        }
    }
}