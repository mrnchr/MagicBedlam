using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ColorPicker : NetworkBehaviour 
{
    #region Singleton
    static private ColorPicker _instance;

    static public ColorPicker Instance {
        get {
            return _instance;
        }
    }
    #endregion

    private List<Color> _currentColor;

    private void Awake() {
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _currentColor = new List<Color>();
    }

    public override void OnStartServer() {
        Color[] colors = GameData.Instance.GetAllColors();
        colors.CopyTo<Color>(_currentColor);
    }

    public Color PickColor() {
        Color endColor = _currentColor[Random.Range(0, _currentColor.Count)];
        _currentColor.Remove(endColor);

        return endColor;
    }

}