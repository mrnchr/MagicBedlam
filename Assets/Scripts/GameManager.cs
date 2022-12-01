using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct PlayerInfo : IComparable<PlayerInfo> {
    public int connId;
    public Color playerColor;
    public int scores;

    public PlayerInfo(int id, Color color, int scores) {
        connId = id;
        playerColor = color;
        this.scores = scores;
    }

    public int CompareTo(PlayerInfo compared) { 
        return -scores.CompareTo(compared.scores);
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

    [SyncVar(hook = nameof(WhenChangeCurrentTime))] private float _currentTime;
    private readonly SyncList<PlayerInfo> _players = new SyncList<PlayerInfo>();

    // NOTE: Will it be faster to call this function every second?
    public void WhenChangeCurrentTime(float oldTime, float newTime) {
        GameMenu.Instance.ChangeTime(Mathf.RoundToInt(newTime));
    }

    public override void OnStartServer()
    {
        List<Color> colors = new List<Color>(_playerColors);
        PlayerInfo info;
        int colorIndex;

        int index = 0;

        foreach(var conn in NetworkServer.connections) {
            colorIndex = UnityEngine.Random.Range(0, colors.Count);

            // TODO: add check when client reconnects
            info = new PlayerInfo(conn.Key, colors[colorIndex], 0);
            _players.Add(info);
            colors.RemoveAt(colorIndex);

            Debug.Log($"Player is added with {_players[index].connId}, {_players[index].playerColor}, {_players[index].scores}");
            index++;
        }

        _currentTime = _gameTime;
    }

    [ServerCallback]
    private void FixedUpdate() {
        _currentTime -= Time.fixedDeltaTime;

        if(_currentTime <= 0) {
            Win();
        }
    }

    public PlayerInfo GetPlayerInfoByIndex(int index) => _players[index];
    public PlayerInfo GetPlayerInfoByConn(int id) => _players.Find((match) => { return match.connId == id; });
    public int GetIndexPlayerInfo(int id) => _players.FindIndex((match) => { return match.connId == id; });
    public List<PlayerInfo> GetAllPlayerInfo() {
        List<PlayerInfo> players = new List<PlayerInfo>();
        _players.CopyTo<PlayerInfo>(players);

        return players;
    }

    [Server]
    public void PlayerDeath(Player killed, Player killer) {
        int killerIndex = GetIndexPlayerInfo(killer.connectionToClient.connectionId);
        PlayerInfo killerInfo = _players[killerIndex];
        ++killerInfo.scores;

        _players[killerIndex] = killerInfo;

        killed.Dead();

        GameMenu.Instance.RpcChangePlayerScores(killerIndex, killerInfo.scores);

        SortList();

        StartCoroutine(WaitToUpdateTable());

        Debug.Log($"The {_players[killerIndex].playerColor} player has {_players[killerIndex].scores} scores in total");

        if(_players[killerIndex].scores >= _winScores) {
            Win();
        }
    }

    [Server]
    IEnumerator WaitToUpdateTable() {
        yield return new WaitForSeconds(0.1f);
        GameMenu.Instance.RpcChangePlayerTable();
    }

    [Server]
    private void SortList() {   
        List<PlayerInfo> players = new List<PlayerInfo>();
        _players.CopyTo<PlayerInfo>(players);
        players.Sort();

        for(int i = 0; i < _players.Count; i++) {
            _players[i] = players[i];
        }
    }

    [Server]
    private void Win() {
        Debug.Log($"Winner is {_players[0].playerColor}. He has {_players[0].scores} scores");
        Spawner.Instance.StopHost();
    }

}
