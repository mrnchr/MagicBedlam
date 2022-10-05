using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spawner : NetworkManager
{
    #region Singleton
    static private Spawner _instance;

    static public Spawner Instance {
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

    [SerializeField] private Transform _spawnZone;
    [SerializeField] private Color[] _playerColors;
    [SerializeField] private Transform _camera;

    private Queue<Color> _players;
    private Vector2 _spawnZoneX;
    private Vector2 _spawnZoneZ;   

    public override void OnStartServer()
    {
        _players = new Queue<Color>(_playerColors);

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
        Debug.Log("Spawner:OnClientDisconnect()");
        base.OnClientDisconnect();
    }

    public void GetColor(Color colorForGet) {
        _players.Enqueue(colorForGet);
    }

    public Color GiveColor() {
        return _players.Dequeue();
    }

    private new void Start() {
        Debug.Log("Spawner:Start()");
    }
}
