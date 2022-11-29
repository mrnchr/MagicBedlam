using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Spawner : NetworkManager
{
    #region Singleton
    private static Spawner _instance;

    public static Spawner Instance {
        get {
            return _instance;
        }
    }

    public override void Awake() {
        base.Awake();
        Debug.Log("Spawner:Awake()");
        
        if(_instance != null) return;
        _instance = this;
    }

    #endregion

    public Transform Camera {
        get {
            return _camera;
        }
    }

    public bool isPlaying { get; private set; }

    private Transform _spawnZone;
    private Transform _camera;

    private Queue<Color> _players;
    private Vector2 _spawnZoneX;
    private Vector2 _spawnZoneZ;

    public override void OnStartServer()
    {
        Debug.Log("Spawner:OnStartServer()");
        //_players = new Queue<Color>(_playerColors);
        //_spawnZone = GameObject.FindGameObjectWithTag("Spawner").transform;

        _spawnZoneX = Vector2.up * 10; //new Vector2(_spawnZone.position.x - _spawnZone.localScale.x / 2, _spawnZone.position.x + _spawnZone.localScale.x / 2);
        _spawnZoneZ = Vector2.up * 10; //new Vector2(_spawnZone.position.z - _spawnZone.localScale.z / 2, _spawnZone.position.z + _spawnZone.localScale.z / 2);
    }

    public override void OnClientConnect()
    {
        Debug.Log("Spawner:OnClientConnect()");
        
        base.OnClientConnect();
        if(!NetworkClient.isHostClient && !isPlaying) {
            Debug.Log("Try to change main menu");
            MainMenu.Instance.ChangeClientMenu(true);
        }
    }

    public override void OnStartClient()
    {
        Debug.Log("Spawner:OnStartClient()");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!isPlaying) {
            MainMenu.Instance.ChangeConnected();
        }
        Debug.Log("Spawner:OnServerConnect()");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Spawner:OnServerAddPlayer()");
        Vector3 startPos = new Vector3 (
        Random.Range(_spawnZoneX.x, _spawnZoneX.y),
        10, //_spawnZone.position.y,
        Random.Range(_spawnZoneZ.x, _spawnZoneZ.y)
        );

        GameObject player = Instantiate(playerPrefab, startPos, Quaternion.identity);

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
        Debug.Log("Spawner:OnClientDisconnect()");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if(!isPlaying) {
            MainMenu.Instance.ChangeConnected();
        }
        Debug.Log("Spawner:OnServerDisconnect()");
    }

    public override void OnClientSceneChanged()
    {
        Debug.Log("Spawner:OnClientSceneChanged");
        if(SceneManager.GetActiveScene().name == "Island") {
            isPlaying = true;

            if (!NetworkClient.ready) NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
                NetworkClient.AddPlayer();
            }
        }
        else {
            isPlaying = false;
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("Spawner:OnServerSceneChanged");
        if(sceneName == "Island") {
            Debug.Log($"The game is true");
        }
    }

    [Server]
    private Vector3 CalculateSpawnPos() {
        Vector3 startPos = new Vector3 (
        Random.Range(_spawnZoneX.x, _spawnZoneX.y),
        _spawnZone.position.y,
        Random.Range(_spawnZoneZ.x, _spawnZoneZ.y)
        );

        return startPos;
    }

    [Server]
    public void Respawn(Transform player) {
        player.position = CalculateSpawnPos();
    }

    [Server]
    public void AddColor(Color colorForGet) {
        Debug.Log("Spawner:AddColor()");
        _players.Enqueue(colorForGet);
    }

    [Server]
    public Color RemoveColor() {
        Debug.Log("Spawner:RemoveColor()");
        Color newColor = _players.Dequeue();
        return newColor;
    }
}
