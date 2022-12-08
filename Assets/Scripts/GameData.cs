using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo {
    public string ip;
    public Color color;
    public int scores;

    public PlayerInfo(string ip, Color color, int scores = 0) {
        this.ip = ip;
        this.color = color;
        this.scores = scores;
    }
}

public class GameData : MonoBehaviour
{
    #region Singleton
    static private GameData _instance;

    static public GameData Instance {
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

    [SerializeField] private float _gameTime;
    [SerializeField] private int _winScores;
    [SerializeField] private Color[] _playerColor;

    public float GameTime {
        get {
            return _gameTime;
        }
    }
    
    public float WinScores {
        get {
            return _winScores;
        }
    }

    public Color[] PlayerColor {
        get {
            return _playerColor;
        }
    }
}