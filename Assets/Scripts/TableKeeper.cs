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

    [SerializeField] private TMP_Text[] _infos;

    private int _numberOfPlayers;

    private void Start() {
        foreach(var info in _infos) {
            info.gameObject.SetActive(false);
        }
    }

    public void AddPlayerInfo(Color playerColor)
    {
        for(int i = 0; i < _numberOfPlayers; i++) {
            if(_infos[i].color == playerColor){
                return;
            }
        }

        _infos[_numberOfPlayers].gameObject.SetActive(true);
        _infos[_numberOfPlayers].color =  playerColor;
        _infos[_numberOfPlayers].text = "0";

        ++_numberOfPlayers;
    }

    public void ChangePlayerTable(Color color, int scores) {
        int oldIndex;
        int newIndex;

        for(oldIndex = 0; oldIndex < _numberOfPlayers; ++oldIndex) {
            if(_infos[oldIndex].color == color)
                break;
        }

        _infos[oldIndex].text = scores.ToString();

        // NOTE: you can not to swap positions objects because you have broken indexes
        // NOTE: also you need to change fields each other in loop not to break the order
        for(newIndex = oldIndex - 1; newIndex >= 0 && scores > int.Parse(_infos[newIndex].text); --newIndex) {
            (_infos[oldIndex].color, _infos[newIndex].color) = (_infos[newIndex].color, _infos[oldIndex].color);
            (_infos[oldIndex].text, _infos[newIndex].text) = (_infos[newIndex].text, _infos[oldIndex].text);
        }
    }    
}
