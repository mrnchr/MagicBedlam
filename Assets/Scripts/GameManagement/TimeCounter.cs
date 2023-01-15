using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;

namespace MagicBedlam
{
    /// <summary>
    /// Count and display session time    
    /// </summary>
    public class TimeCounter : NetworkBehaviour
    {
        [Tooltip("Own text field component")]
        [SerializeField]
        protected TMP_Text _ownText;

        [SyncVar] protected float _currentTime;
        
        [ClientRpc]
        protected void RpcChangeTime(int time)
        {
            _ownText.text = $"{time / 60}:{(time % 60).ToString("00")}";
        }

        [ServerCallback]
        protected void FixedUpdate()
        {
            _currentTime -= Time.fixedDeltaTime;

            if (_currentTime <= 0)
            {
                WinTracker.singleton.Win(true);
            }
        }

        public override void OnStartServer()
        {
            _currentTime = GameData.singleton.SessionTime;
            StartCoroutine(WaitForChangeText());
        }

        [Server]
        protected IEnumerator WaitForChangeText() {
            while (true)
            {
                RpcChangeTime(Mathf.RoundToInt(_currentTime));
                yield return new WaitForSeconds(0.5f);
            }
        }

        protected void Reset() {
            TryGetComponent<TMP_Text>(out _ownText);
        }
    }
}