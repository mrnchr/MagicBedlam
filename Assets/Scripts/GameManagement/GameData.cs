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

        _playerColor = new Dictionary<string, Color>(5);
        _playerColor["red"] = Color.red;
        _playerColor["green"] = Color.green;
        _playerColor["blue"] = Color.blue;
        _playerColor["yellow"] = Color.yellow;
        _playerColor["magenta"] = Color.magenta;
    }
    #endregion

    [SerializeField] private float _gameTime;
    [SerializeField] private int _winScores;
    Dictionary<string, Color> _playerColor;

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

    public Color[] GetAllColors() {
        Color[] colors = new Color[5];
        _playerColor.Values.CopyTo(colors, 0);
        return colors;
    }

    public Color GetColor(string name) {
        return _playerColor[name];
    }

    public string GetColorName(Color color) {
        foreach(var playercolor in _playerColor) {
            if(playercolor.Value == color) {
                return playercolor.Key;
            }
        }
        
        return null;
    }
}