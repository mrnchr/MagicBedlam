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

    public override void Awake() {
        base.Awake();
        Debug.Log("NetworkInteraction:Awake()");
        
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }
    public bool isPlaying { get; private set; }

    private int _selfConnection;

    public int SelfConnection {
        get {
            return _selfConnection;
        }
    }

    public void SetConnection(int conn) => _selfConnection = conn;

    public override void OnStartServer()
    {
        Debug.Log("NetworkInteraction:OnStartServer()");
    }

    public override void OnClientConnect()
    {
        Debug.Log("NetworkInteraction:OnClientConnect()");
        
        base.OnClientConnect();
        if(!NetworkClient.isHostClient && !isPlaying) {
            Debug.Log("Try to change main menu");
            MainMenu.Instance.ChangeClientMenu(true);
        }
    }

    public override void OnStartClient()
    {
        Debug.Log("NetworkInteraction:OnStartClient()");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!isPlaying) {
            MainMenu.Instance.ChangeConnected();
        }
        Debug.Log("NetworkInteraction:OnServerConnect()");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("NetworkInteraction:OnServerAddPlayer()");

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

        base.OnClientDisconnect();
        Debug.Log("NetworkInteraction:OnClientDisconnect()");
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
        Debug.Log("NetworkInteraction:OnServerSceneChanged");
        if(sceneName == "Island") {
            Debug.Log($"The game is true");
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
}
