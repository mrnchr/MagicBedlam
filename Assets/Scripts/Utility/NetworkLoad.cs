using UnityEngine;
using Mirror;

public class NetworkLoad : NetworkManager
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start() 
    {
        if(!NetworkClient.active)
            StartServer();
    }
}
