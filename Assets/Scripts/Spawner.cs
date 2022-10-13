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
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
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
        _players = new Queue<Color>(_playerColors);
        _spawnZone = GameObject.FindGameObjectWithTag("Spawner").transform;


        _spawnZoneX = new Vector2(_spawnZone.position.x - _spawnZone.localScale.x / 2, _spawnZone.position.x + _spawnZone.localScale.x / 2);
        _spawnZoneZ = new Vector2(_spawnZone.position.z - _spawnZone.localScale.z / 2, _spawnZone.position.z + _spawnZone.localScale.z / 2);
    }

    public override void OnClientConnect()
    {
        Debug.Log("Spawner:OnClientConnect()");
        base.OnClientConnect();
        NetworkClient.AddPlayer();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Spawner:OnServerAddPlayer()");
        Vector3 startPos = new Vector3 (
        Random.Range(_spawnZoneX.x, _spawnZoneX.y),
        _spawnZone.position.y,
        Random.Range(_spawnZoneZ.x, _spawnZoneZ.y)
        );

        GameObject player = Instantiate(playerPrefab, startPos, Quaternion.identity);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("Spawner:OnClientDisconnect()");
    }

    public void AddColor(Color colorForGet) {
        Debug.Log("Spawner:AddColor()");
        _players.Enqueue(colorForGet);
    }

    public Color RemoveColor() {
        Debug.Log("Spawner:RemoveColor()");
        Color newColor = _players.Dequeue();
        return newColor;
    }

    private new void Start() {
        Debug.Log("Spawner:Start()");
    }
}
