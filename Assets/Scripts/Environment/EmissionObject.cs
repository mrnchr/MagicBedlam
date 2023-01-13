using UnityEngine;
using Mirror;

public class EmissionObject : MonoBehaviour {
    [SerializeField] private MeshRenderer[] _emissionMeshs;
    [SerializeField] private Color[] _emissionColors;

    private void Start() {
        _emissionColors = new Color[_emissionMeshs.Length];

        for (int i = 0; i < _emissionColors.Length; i++) {
            _emissionColors[i] = _emissionMeshs[i].material.GetColor("_EmissionColor");
        }
    }

    public bool TryGetColor(MeshRenderer match, out Color origin) {
        origin = Color.black;

        for (int i = 0; i < _emissionColors.Length; i++) {
            if(match == _emissionMeshs[i]) {
                origin = _emissionColors[i];
                return true;
            }
        }

        return false;
    }
}