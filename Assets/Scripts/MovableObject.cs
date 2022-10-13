using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : NetworkBehaviour
{
    private Material _material;

    public void Glow(bool highlighted) {
        if(highlighted)
            _material.EnableKeyword("_EMISSION");
        else 
            _material.DisableKeyword("_EMISSION");
    }

    // TODO: always to set layer mask as MovableObject
    private void Awake() {
        _material = GetComponent<MeshRenderer>().material;
    }
}
