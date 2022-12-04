using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : NetworkBehaviour
{
     [SyncVar] public bool isThrowing;
     [SyncVar] public GameObject owner;
    private MeshRenderer[] _meshs;

    public void Glow(bool highlighted) {
        foreach(var mesh in _meshs) {
            if(mesh) {
                foreach(var mat in mesh.materials) {
                    if(mat) {
                        if(highlighted && !owner) {
                            mat.EnableKeyword("_EMISSION");
                            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                            //mat.SetColor("_EmissionColor", WinTracker.Instance.GetPlayerInfoByConn(Spawner.Instance.SelfConnection).playerColor);
                        }
                        else 
                            mat.DisableKeyword("_EMISSION");
                    }
                }
            }
        }
    }

    // NOTE: it's called from here because we need to clean owner after death event
    [ServerCallback]
    private void OnTriggerEnter(Collider col) {
        if(isThrowing && col.gameObject != owner) {
            isThrowing = false;
            
            if(col.tag == "Player") {
                WinTracker.Instance.PlayerDeath(col.GetComponent<Player>(), owner.GetComponent<Player>());
            }

            owner = null;
        }
    }

    // TODO: always to set layer mask as MovableObject
    private void Awake() {
        MeshRenderer[] meshs = GetComponentsInChildren<MeshRenderer>();
        _meshs = new MeshRenderer[meshs.Length + 1];
        _meshs[0] = GetComponent<MeshRenderer>();
        meshs.CopyTo(_meshs, 1);
    }
}
