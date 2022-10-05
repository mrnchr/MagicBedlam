using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    private Spawner _spawner;

    private Transform _camera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _spawner = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Spawner>();
        Debug.Log("NP:OnStartClient - I was born");
        if(isLocalPlayer) {
            Debug.Log("NP:OnStartClient - Camera is in place");
            _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            _camera.SetParent(NetworkClient.localPlayer.transform);
            _camera.localPosition = new Vector3(0, 0.7f, 0);
        }
    }
}
