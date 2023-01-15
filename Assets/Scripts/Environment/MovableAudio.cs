using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    public enum ObjectSound
    {
        Dull,
        Glass,
        Wood,
        LightMetal,
        HeavyMetal,
        Stone
    }

    [RequireComponent(typeof(AudioSource))]
    public class MovableAudio : NetworkBehaviour
    {
        static protected Queue<AudioSource> _audioPlayers;

        [SerializeField]
        protected AudioSource _ownAudio;
        [SerializeField]
        protected ObjectSound _soundType;

        public ObjectSound SoundType
        {
            get
            {
                return _soundType;
            }
        }

        protected void Awake() 
        {
            _audioPlayers = new Queue<AudioSource>();
        }

        protected void Start() 
        {
            StartCoroutine(WaitForUpdateSound());
        }

        protected void OnCollisionEnter(Collision col) 
        {
            if(col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Water") || _audioPlayers.Count == 16)
                return;

            if(!col.gameObject.CompareTag("MovableObject") || col.gameObject.GetComponent<MovableAudio>().SoundType > _soundType) {
                if(col.relativeVelocity.sqrMagnitude > Mathf.Pow(1, 2))
                    Play();
            }
        }

        protected void Play() {
            _ownAudio.Play();
            _audioPlayers.Enqueue(_ownAudio);
        }

        protected void Reset() 
        {
            TryGetComponent<AudioSource>(out _ownAudio);
        }

        protected IEnumerator WaitForUpdateSound() 
        {
            while (true)
            {
                while(_audioPlayers.Count > 0 && !_audioPlayers.Peek().isPlaying)
                {
                    _audioPlayers.Dequeue();
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}