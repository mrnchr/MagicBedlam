using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChanger : MonoBehaviour
{
    #region Singleton
    static private ColorChanger _instance;

    static public ColorChanger Instance {
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

    public void SetColor(Color menuColor) {
        _timeText.color = _cursor.color = menuColor;
    }
}
