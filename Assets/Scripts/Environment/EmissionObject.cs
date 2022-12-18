using UnityEngine;
using Mirror;

public class EmissionObject : NetworkBehaviour {
    [SerializeField] private MeshRenderer[] _emissionMeshs;

    public bool TryGetColor(MeshRenderer match, out Color origin) {
        origin = Color.black;

        foreach (var mat in _emissionMeshs) {
            if(match == mat) {
                origin = mat.material.GetColor("_EmissionColor");
                return true;
            }
        }

        return false;
    }
}