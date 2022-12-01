using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class GameMenu : NetworkBehaviour
{
    #region Singleton
    static private GameMenu _instance;

    static public GameMenu Instance {
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

    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private Image _cursor;
    [SerializeField] private TMP_Text[] _infos;

    public override void OnStartClient()
    {
        List<PlayerInfo> players = GameManager.Instance.GetAllPlayerInfo();

        for(int i = 0; i < players.Count; i++) {
            _infos[i].color =  players[i].playerColor;
            _infos[i].text = players[i].scores.ToString();
        }

        for(int i = players.Count; i < _infos.Length; i++) {
            _infos[i].gameObject.SetActive(false);
        }
    }

    public void SetColor() {
        Color self = GameManager.Instance.GetPlayerInfoByConn(Spawner.Instance.SelfConnection).playerColor;
        _timeText.color = _cursor.color = self;
    }

    [ClientRpc]
    public void RpcChangePlayerTable() {
        List<PlayerInfo> infos = GameManager.Instance.GetAllPlayerInfo();

        Vector3[] columnPos = new Vector3[infos.Count];
        for(int i = 0; i < columnPos.Length; i++) {
            columnPos[i] = _infos[i].rectTransform.localPosition;
            Debug.Log($"{i} player: {infos[i].scores} scores");
        }

        int currentIndex;
        for(int i = 0; i < columnPos.Length; i++) {
            currentIndex = infos.FindIndex((match) => { return match.playerColor == _infos[i].color; });
            _infos[i].rectTransform.localPosition = columnPos[currentIndex];
        }
    }

    [ClientRpc]
    public void RpcChangePlayerScores(int index, int scores) {
        _infos[index].text = scores.ToString();
    }

    public void ChangeTime(int time) {
        _timeText.text = $"{time / 60}:{(time % 60).ToString("00")}";
    }
}
