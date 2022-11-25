using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private new void Awake() {
        _instance = this;
        Debug.Log("Spawner:Awake()");
    }

    #endregion

    public Transform Camera {
        get {
            return _camera;
        }
    }

    private Transform _spawnZone;
    [SerializeField] private Color[] _playerColors;
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

        //_camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //NetworkClient.AddPlayer();
    }

    public override void OnStartClient()
    {
        Debug.Log("Spawner:OnStartClient()");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        MainMenu.Instance.ChangeConnected();
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
        base.OnClientDisconnect();
        Debug.Log("Spawner:OnClientDisconnect()");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        MainMenu.Instance.ChangeConnected();
        Debug.Log("Spawner:OnServerDisconnect()");
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

    private new void Start() {
        Debug.Log("Spawner:Start()");
    }
}
