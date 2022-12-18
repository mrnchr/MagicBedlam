using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : NetworkBehaviour
{
    [HideInInspector] [SyncVar] public bool isThrowing;
    [HideInInspector] [SyncVar] public GameObject owner;
    [SerializeField] private EmissionObject _selfEmission;
    private MeshRenderer[] _meshs;

    public void Glow(bool highlighted) {
        foreach(var mesh in _meshs) {
            foreach(var mat in mesh.materials) {
                if(mat) {
                    if(highlighted && !owner) {
                        mat.EnableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        mat.SetColor("_EmissionColor", Player.LocalPlayer.OwnColor);
                    }
                    else {
                        Color oldEmission;
                        if(_selfEmission && _selfEmission.TryGetColor(mat, out oldEmission)) {
                            mat.SetColor("_EmissionColor", oldEmission);
                        }
                        else {
                            mat.DisableKeyword("_EMISSION");
                        }
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
            
            if(col.CompareTag("Player")) {
                Debug.Log($"I catch {col.name}");
                WinTracker.Instance.PlayerDeath(col.GetComponent<Player>(), owner.GetComponent<Player>());
            }

            owner = null;
        }
    }

    // TODO: always to set layer mask as MovableObject
    private void Awake() {
        _meshs = GetComponentsInChildren<MeshRenderer>();
    }
}
