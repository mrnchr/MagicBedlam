using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkInteraction : NetworkManager
{
    #region Singleton
    static private NetworkInteraction _instance;

    static public NetworkInteraction Instance {
        get {
            return _instance;
        }
    }

    #endregion

    [SerializeField] private bool _startHostAtOnce;
   
    public bool isPlaying { get; private set; }

    private string _lastConnection;

    public string LastConnection {
        get {
            return _lastConnection;
        }
    }

    private List<PlayerInfo> _playerInfos;

    public override void Awake() {
        base.Awake();
        
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;

        #if UNITY_EDITOR
            _lastConnection = "localhost";
        #endif
    }

    public override void Start() {
        Debug.Log("NetworkInteraction:Start");

        #if UNITY_EDITOR
            if(_startHostAtOnce) {
                MainMenu.Instance.ForHost();
                MainMenu.Instance.PlayGame();
            }
        #endif
    }

    public override void OnStartServer()
    {
        _lastConnection = "";
    }

    public override void OnClientConnect()
    {        
        base.OnClientConnect();
        if(!NetworkClient.isHostClient && !isPlaying) {
            MainMenu.Instance.ChangeClientMenu(true);
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!isPlaying) {
            MainMenu.Instance.ChangeConnected();
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab, Spawner.Instance.CalculateSpawnPosition(), Quaternion.identity);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnClientDisconnect()
    {
        //_camera.SetParent(null);
        if(!NetworkClient.isHostClient && !isPlaying) {
            MainMenu.Instance.ChangeClientMenu(false);
        }

        if(!NetworkClient.isHostClient) {
            _lastConnection = NetworkClient.serverIp;
        }

        base.OnClientDisconnect();
        Debug.Log("NetworkInteraction:OnClientDisconnect()");
    }

    public override void ServerChangeScene(string newSceneName)
    {
        #if UNITY_EDITOR
            base.ServerChangeScene(newSceneName);
        #else
            if(isPlaying || NetworkServer.connections.Count > 1) {
                base.ServerChangeScene(newSceneName);
            }
        #endif
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if(!isPlaying) {
            MainMenu.Instance.ChangeConnected();
        }
        Debug.Log("NetworkInteraction:OnServerDisconnect()");
    }

    public override void OnClientSceneChanged()
    {
        Debug.Log("NetworkInteraction:OnClientSceneChanged");
        if(SceneManager.GetActiveScene().name == "Island") {
            isPlaying = true;

            if (!NetworkClient.ready) NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }
        }
        else {
            isPlaying = false;
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName == "Island") {
            Debug.Log($"The game is true");
            maxConnections = NetworkServer.connections.Count; // NOTE: not to connect new players 
            _playerInfos = new List<PlayerInfo>(maxConnections);

            foreach(var conn in NetworkServer.connections) {
                _playerInfos.Add(new PlayerInfo(conn.Value.address, ColorPicker.Instance.PickColor()));
            }
        }
    }

    public void Disconnect() {
        if(NetworkClient.isHostClient) {
            StopHost();
        }
        else {
            StopClient();
        }
    }

    [Server]
    public Color GetColor(string ip) => _playerInfos.Find((match) => { return match.ip == ip; }).color;

    [Server]
    public int GetScores(string ip) => _playerInfos.Find((match) => { return match.ip == ip; }).scores;

    [Server]
    public void SaveInfo(string ip, in Color playerColor, in int playerScores) {
        _playerInfos.Remove(_playerInfos.Find((match) => { return match.ip == ip; }));
        _playerInfos.Add(new PlayerInfo(ip, playerColor, playerScores));

        Debug.Log($"Info about the player with ip {ip} was saved");
    }
}
