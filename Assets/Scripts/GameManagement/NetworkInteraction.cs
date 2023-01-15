using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Interaction between players over the network
    /// </summary>
    public class NetworkInteraction : NetworkManager
    {
        static public new NetworkInteraction singleton { get; protected set; }

        protected string _lastConnection;

        /// <summary>
        ///     Called when the new client connects or disconnects on the client
        /// </summary>
        public Action OnClientEvent;
        /// <summary>
        ///     Called when the new client connects or disconnects on the server
        /// </summary>
        public Action OnServerEvent;

        /// <summary>
        ///     Last connection to the server. The empty string by default
        /// </summary>
        public string LastConnection
        {
            get
            {
                return _lastConnection;
            }
        }

        public override void Awake()
        {
            base.Awake();

            singleton = Singleton.Create<NetworkInteraction>(this, singleton);

#if UNITY_EDITOR
            _lastConnection = "localhost";
#endif
        }

        /// <summary>
        /// Disconnect the host or the client
        /// </summary>
        public void Disconnect()
        {
            if (NetworkClient.isHostClient)
            {
                StopHost();
            }
            else
            {
                StopClient();
            }
        }

        public override void OnClientConnect()
        {
            Debug.Log("NetworkInteraction:OnClientConnect()");
            base.OnClientConnect();
            if (!NetworkClient.isHostClient)
                OnClientEvent?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            if (!NetworkClient.isHostClient)
            {
                _lastConnection = NetworkClient.serverIp;
                OnClientEvent?.Invoke();
            }

            base.OnClientDisconnect();

            Debug.Log("NetworkInteraction:OnClientDisconnect()");
        }

        public override void OnClientSceneChanged()
        {
            Debug.Log("NetworkInteraction:OnClientSceneChanged");
            if (SceneManager.GetActiveScene().name == "Castle")
            {
                if (!NetworkClient.ready) NetworkClient.Ready();
                if (NetworkClient.localPlayer == null)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }

        public override void OnStartServer()
        {
            _lastConnection = "";
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            GameObject player = Instantiate(playerPrefab);

            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log("NetworkInteraction:OnServerConnect()");
            base.OnServerConnect(conn);
            OnServerEvent?.Invoke();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            OnServerEvent?.Invoke();
            Debug.Log("NetworkInteraction:OnServerDisconnect()");
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == "Castle")
                maxConnections = numPlayers; // NOTE: not to connect new players 
        }

        public override void ServerChangeScene(string newSceneName)
        {
            base.ServerChangeScene(newSceneName);
        }
    }
}
