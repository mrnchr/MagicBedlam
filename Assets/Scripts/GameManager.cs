using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct PlayerInfo {
    public int connId;
    public Color playerColor;
    public int scores;

    public PlayerInfo(int id, Color color, int scores) {
        connId = id;
        playerColor = color;
        this.scores = scores;
    }
}

public class GameManager : NetworkBehaviour {

    #region Singleton
    static private GameManager _instance;

    static public GameManager Instance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    [SerializeField] private Color[] _playerColors;
    [SerializeField] private float _gameTime;
    [SerializeField] private int _winScores;

    [SyncVar] private float _currentTime;
    private readonly SyncList<PlayerInfo> _players = new SyncList<PlayerInfo>();

    public override void OnStartServer()
    {
        List<Color> colors = new List<Color>(_playerColors);
        PlayerInfo info;
        int colorIndex;

        foreach(var conn in NetworkServer.connections) {
            colorIndex = Random.Range(0, colors.Count);

            // TODO: add check when client reconnects
            info = new PlayerInfo(conn.Key, colors[colorIndex], 0);
            _players.Add(info);

            colors.RemoveAt(colorIndex);
        }

        _currentTime = _gameTime;
    }

    [ServerCallback]
    private void FixedUpdate() {
        _currentTime -= Time.fixedDeltaTime;
    }

    public PlayerInfo GetPlayerInfo(int id) => _players.Find((match) => { return match.connId == id; });
    public int GetIndexPlayerInfo(int id) => _players.FindIndex((match) => { return match.connId == id; });

    [Server]
    public void PlayerDeath(Player killed, Player killer) {
        int killerIndex = GetIndexPlayerInfo(killer.connectionToClient.connectionId);
        PlayerInfo killerInfo = _players[killerIndex];
        ++killerInfo.scores;

        _players.RemoveAt(killerIndex);
        _players.Insert(killerIndex, killerInfo);

        Debug.Log($"The {_players[killerIndex].playerColor} player has {_players[killerIndex].scores} scores in total");

        killed.Dead();
    }

}
