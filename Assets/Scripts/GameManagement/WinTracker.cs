using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Keep track of win event
    /// </summary>
    public class WinTracker : NetworkBehaviour
    {
        public static WinTracker singleton { get; protected set; }

        [SerializeField] protected AudioSource _musicAudio;
        [SerializeField] protected AudioClip _loseAudio;
        [SerializeField] protected AudioClip _winAudio;

        // NOTE: used that win function by time wouldn't be called several times 
        [SyncVar] protected bool _endOfGame;

        public bool EndOfGame
        {
            get
            {
                return _endOfGame;
            }
        }

        protected void Awake()
        {
            singleton = Singleton.Create<WinTracker>(this, singleton);

            Debug.Log("WinTracker:Awake");
        }

        // NOTE: It's needed to process the order of operations and keep track of win event 
        /// <summary>
        ///     Give scores to the player and check win
        /// </summary>
        /// <param name="receiver"></param>
        [Server]
        public void GiveScores(Player receiver)
        {
            int newScores = receiver.IncrementScores();

            if (newScores >= GameData.singleton.WinScores)
            {
                Win(false);
            }
        }
        
        [ClientRpc]
        protected void RpcSetWin(bool isOutOfTime)
        {
            InputManager.singleton.LockInput();
            GameMenu.singleton?.SetWinMenu(isOutOfTime);

            PlayMusic();
        }

        protected void PlayMusic()
        {
            if(!_musicAudio)
                return;

            _musicAudio.Stop();
            _musicAudio.loop = false;
            _musicAudio.playOnAwake = false;
            _musicAudio.volume = 1;
            if(Player.localPlayer.OwnColor == TableKeeper.singleton.GetWinner()) 
            {
                _musicAudio.clip = _winAudio;
            }
            else 
            {
                _musicAudio.clip = _loseAudio;
            }

            _musicAudio.Play();
        }

        [Server]
        protected IEnumerator WaitForEndOfGame()
        {
            yield return new WaitForSeconds(7);
            NetworkInteraction.singleton.Disconnect();
        }

        /// <summary>
        ///     Call win menu on the clients and start the countdown to exit
        /// </summary>
        /// <param name="outOfTime">Whether game time is out of or not</param>
        [Server]
        public void Win(bool isOutOfTime)
        {
            if (_endOfGame)
                return;

            _endOfGame = true;
            RpcSetWin(isOutOfTime);
            StartCoroutine(WaitForEndOfGame());
        }
    }
}
