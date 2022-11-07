using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : NetworkBehaviour
{
    [HideInInspector] [SyncVar] public bool isThrowing;
    [HideInInspector] [SyncVar] public GameObject owner;
    private MeshRenderer[] _meshs;

    public void Glow(bool highlighted) {
        foreach(var mesh in _meshs) {
            if(mesh) {
                foreach(var mat in mesh.materials) {
                    if(mat) {
                        if(highlighted) {
                            mat.EnableKeyword("_EMISSION");
                            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        }
                        else 
                            mat.DisableKeyword("_EMISSION");
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col) {
        if(isServer) {
            if(isThrowing) {
                isThrowing = false;
                if(col.tag == "Player" && col.gameObject != owner) {
                    owner = null;
                    Respawn(col.transform);
                    Debug.Log("Detected player");
                }
            }
        }
    }

    [Command] 
    private void CmdRespawn(Transform player) {
        Spawner.Instance.Respawn(player);
    }

    private void Respawn(Transform player) { // call only the server
        Spawner.Instance.Respawn(player);
    }

    // TODO: always to set layer mask as MovableObject
    private void Awake() {
        MeshRenderer[] meshs = GetComponentsInChildren<MeshRenderer>();
        _meshs = new MeshRenderer[meshs.Length + 1];
        _meshs[0] = GetComponent<MeshRenderer>();
        meshs.CopyTo(_meshs, 1);
    }
}
