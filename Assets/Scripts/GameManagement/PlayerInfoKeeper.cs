using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    // NOTE: stores start information or after disconnecting the client. Non-synchronized  
    /// <summary>
    /// Global access point about information of players        
    /// </summary>
    public class PlayerInfoKeeper : NetworkBehaviour
    {
        public static PlayerInfoKeeper singleton { get; protected set; }

        protected List<PlayerData> _playerInfos;

        protected void Awake()
        {
            singleton = Singleton.Create<PlayerInfoKeeper>(this, singleton);
        }

        /// <summary>
        ///     Get the player color by his ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        [Server]
        public Color GetColor(string ip) => _playerInfos.Find((match) => { return match.ip == ip; }).color;

        /// <summary>
        ///     Get the player scores by his ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        [Server]
        public int GetScores(string ip) => _playerInfos.Find((match) => { return match.ip == ip; }).scores;

        public override void OnStartServer()
        {
            _playerInfos = new List<PlayerData>(NetworkServer.connections.Count);

            Color playerColor;
            var colorPicker = new ColorPicker();

            foreach (var conn in NetworkServer.connections)
            {
                playerColor = colorPicker.PickColor();
                _playerInfos.Add(new PlayerData(conn.Value.address, playerColor));
            }
        }

        /// <summary>
        ///     Save information about a player for the duration of the session
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="playerColor"></param>
        /// <param name="playerScores"></param>
        [Server]
        public void SaveInfo(string ip, in Color playerColor, in int playerScores)
        {
            _playerInfos.Remove(_playerInfos.Find((match) => { return match.ip == ip; }));
            _playerInfos.Add(new PlayerData(ip, playerColor, playerScores));

            Debug.Log($"Info about the player with ip {ip} was saved");
        }

        protected struct PlayerData
        {
            public Color color;
            public string ip;
            public int scores;

            public PlayerData(string ip, Color color, int scores = 0)
            {
                this.ip = ip;
                this.color = color;
                this.scores = scores;
            }
        }
    }
}