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
        PlayerInfo[] players = GameManager.Instance.GetAllPlayerInfo();

        for(int i = 0; i < players.Length; i++) {
            _infos[i].color =  players[i].playerColor;
            _infos[i].text = players[i].scores.ToString();
        }

        for(int i = players.Length; i < _infos.Length; i++) {
            _infos[i].gameObject.SetActive(false);
        }
    }

    public void SetColor() {
        Color self = GameManager.Instance.GetPlayerInfo(Spawner.Instance.SelfConnection).playerColor;
        _timeText.color = _cursor.color = self;
    }

    public void ChangeTime(int time) {
        _timeText.text = $"{time / 60}:{(time % 60).ToString("00")}";
    }
}
