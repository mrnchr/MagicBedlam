using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class TableKeeper : MonoBehaviour
{
    #region Singleton
    static private TableKeeper _instance;

    static public TableKeeper Instance {
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

    [SerializeField] private TMP_Text[] _scores;
    [SerializeField] private TMP_Text[] _colors;

    private int _numberOfPlayers;

    private void Start() {
        for(int i = 0; i < _scores.Length; i++) {
            _scores[i].gameObject.SetActive(false);
            _colors[i].gameObject.SetActive(false);
        }
    }

    public void AddPlayerInfo(Color playerColor)
    {
        for(int i = 0; i < _numberOfPlayers; i++) {
            if(_scores[i].color == playerColor){
                return;
            }
        }

        _scores[_numberOfPlayers].gameObject.SetActive(true);
        _scores[_numberOfPlayers].color =  playerColor;
        _scores[_numberOfPlayers].text = "0";
        _colors[_numberOfPlayers].gameObject.SetActive(true);
        _colors[_numberOfPlayers].color = playerColor;
        _colors[_numberOfPlayers].text = GameData.Instance.GetColorName(playerColor);

        ++_numberOfPlayers;
    }

    public void ChangePlayerTable(Color color, int scores) {
        int oldIndex;
        int newIndex;

        for(oldIndex = 0; oldIndex < _numberOfPlayers; ++oldIndex) {
            if(_scores[oldIndex].color == color)
                break;
        }

        _scores[oldIndex].text = scores.ToString();

        // NOTE: you can not to swap positions objects because you have broken indexes
        // NOTE: also you need to change fields each other in loop not to break the order
        for(newIndex = oldIndex - 1; newIndex >= 0 && scores > int.Parse(_scores[newIndex].text); --newIndex) {
            SwapTableString(oldIndex, newIndex);
        }
    }    

    private void SwapTableString(int oldIndex, int newIndex) {
        SwapTableText(_scores[oldIndex], _scores[newIndex]);
        SwapTableText(_colors[oldIndex], _colors[newIndex]);
    }

    private void SwapTableText(TMP_Text a, TMP_Text b) {
        (a.color, b.color) = (b.color, a.color);
        (a.text, b.text) = (b.text, a.text);
    }
}
