using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMenu : MonoBehaviour
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

    public void ChangeTime(int time) {
        _timeText.text = $"{time / 60}:{(time % 60).ToString("00")}";
    }
}
