using UnityEngine;
using Mirror;

public class EmissionObject : NetworkBehaviour {
    [SerializeField] private Material[] _emissionMats;

    public bool TryGetColor(Material match, out Color origin) {
        origin = Color.black;

        foreach (var mat in _emissionMats) {
            if(match == mat) {
                origin = mat.GetColor("_EmissionColor");
                return true;
            }
        }

        return false;
    }
}