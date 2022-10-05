using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spawner : NetworkManager
{
    [SerializeField] private Color[] _playerColors;
    [SerializeField] private Transform _camera;

    [HideInInspector] [SerializeField] Queue<Color> _players;
    public Vector2 _spawnZoneX { private set; get; } 
    public Vector2 _spawnZoneZ { private set; get; }    
    public Transform _spawnZone { private set; get; }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _players = new Queue<Color>(_playerColors);

        _spawnZone = GameObject.FindGameObjectWithTag("Respawn").transform;

        _spawnZoneX = new Vector2(_spawnZone.position.x - _spawnZone.localScale.x / 2, _spawnZone.position.x + _spawnZone.localScale.x / 2);
        _spawnZoneZ = new Vector2(_spawnZone.position.z - _spawnZone.localScale.z / 2, _spawnZone.position.z + _spawnZone.localScale.z / 2);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.AddPlayer();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Vector3 endPos = new Vector3 (
        Random.Range(_spawnZoneX.x, _spawnZoneX.y),
        _spawnZone.position.y,
        Random.Range(_spawnZoneZ.x, _spawnZoneZ.y)
        );

        conn.identity.transform.position = endPos;
    }

    public override void OnClientDisconnect()
    {
        //CmdReturnColor(NetworkClient.localPlayer.GetComponent<MeshRenderer>().material.color);
        base.OnClientDisconnect();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
